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
   public class QuestionGroupController : ControllerBase
   {
      public QuestionGroupController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<PrivilegeController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._context = context;
         this._mobile = _mobile.Value;
         this._logger = logger;
         this._smtp = smtp.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      #region Group
      [HttpGet]
      public IActionResult Index()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }

         var model = this._context.QuestionGroups.OrderBy(c => c.Index);
         return View("QuestionGroup", model);
      }
      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = new QuestionGroup();
         model.Status = StatusType.Active;
         return View("QuestionGroupInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.QuestionGroups
                        .Where(w => w.ID == id).FirstOrDefault();

         return View("QuestionGroupInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(QuestionGroup model, IFormFile file, IFormFile video)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }

         if (ModelState.IsValid)
         {
            if (model.ID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;
               model.Index = 1;
               if (this._context.QuestionGroups.Count() > 0)
               {
                  var lastindex = this._context.QuestionGroups.Max(s => s.Index);
                  if (lastindex > 0)
                  {
                     model.Index = lastindex + 1;
                  }
               }

               this._context.QuestionGroups.Add(model);
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

         return View("QuestionGroupInfo", model);
      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         QuestionGroup model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.QuestionGroups.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var questions = this._context.Questions.Where(w => w.QuestionGroupID == model.ID);
               this._context.Questions.RemoveRange(questions);
               this._context.QuestionGroups.Remove(model);
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
      #endregion
   }
}