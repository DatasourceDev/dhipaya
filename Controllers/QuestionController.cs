using System;
using System.Collections.Generic;
using System.Linq;
using Dhipaya.DAL;
using Dhipaya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Dhipaya.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dhipaya.Services;
using System.Threading.Tasks;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers.Admin
{
   public class QuestionController : ControllerBase
   {
      public QuestionController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<QuestionController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._context = context;
         this._mobile = _mobile.Value;
         this._logger = logger;
         this._smtp = smtp.Value;
         this._conf = conf.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      [HttpGet]
      public IActionResult Index()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }

         var model = this._context.Questions.OrderBy(c => c.Index);
         return View("Question", model);
      }


      public IActionResult MoveUp()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Question model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Questions.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Questions.Where(w => w.Index < model.Index).OrderByDescending(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Questions.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.ID == item.ID)
                  {
                     latestindex.Index = i + 1;
                  }
                  else if (latestindex != null && model.ID == item.ID)
                  {
                     item.Index = i;
                     i += 2;
                  }
                  else
                  {
                     item.Index = i;
                     i++;
                  }
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }
      public IActionResult MoveDown()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Question model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Questions.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Questions.Where(w => w.Index > model.Index).OrderBy(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Questions.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.ID == item.ID)
                  {
                     latestindex.Index = i;
                     i += 2;
                  }
                  else if (latestindex != null && model.ID == item.ID)
                  {
                     item.Index = i + 1;
                  }
                  else
                  {
                     item.Index = i;
                     i++;
                  }
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = new Question();
         model.Status = StatusType.Active;
         ViewBag.ListGroup = this._context.QuestionGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         return View("QuestionInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.Questions
                        .Where(w => w.ID == id).FirstOrDefault();
         if (model != null)
         {
            model.sDate = DateUtil.ToDisplayDate(model.StartDate);
            model.eDate = DateUtil.ToDisplayDate(model.EndDate);
         }
         ViewBag.ListGroup = this._context.QuestionGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         return View("QuestionInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(Question model, IFormFile file, IFormFile video)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }

         if (ModelState.IsValid)
         {
            model.StartDate = DateUtil.ToDate(model.sDate);
            model.EndDate = DateUtil.ToDate(model.eDate);

            if (model.ID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;
               model.Index = 1;
               if (this._context.Questions.Count() > 0)
               {
                  var lastindex = this._context.Questions.Max(s => s.Index);
                  if (lastindex > 0)
                  {
                     model.Index = lastindex + 1;
                  }
               }

               this._context.Questions.Add(model);
               this._context.SaveChanges();
               return RedirectToAction("Index");

            }
            else
            {
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;

               this._context.Update(model);
               this._context.SaveChanges();

               return RedirectToAction("Index");
            }

         }
         ViewBag.ListGroup = this._context.QuestionGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         return View("QuestionInfo", model);
      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Question model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Questions.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               this._context.Questions.Remove(model);
               this._context.SaveChanges();
               var i = 1;
               foreach (var item in this._context.Questions.OrderBy(o => o.Index))
               {
                  item.Index = i;
                  i++;
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }


   }

}