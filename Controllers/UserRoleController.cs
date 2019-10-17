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
using System.Collections.ObjectModel;
using Dhipaya.DTO.Accounts;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{

   public class UserRoleController : ControllerBase
   {
      public UserRoleController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<UserRoleController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._context = context;
         this._mobile = _mobile.Value;
         this._logger = logger;
         this._smtp = smtp.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      public IActionResult Index()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.UserRoles.OrderBy(r => r.UserRoleID);

         ViewBag.ItemCount = model.Count();
         return View(model);
      }

      public IActionResult Create()
      {
         UserRole model = new UserRole();
         model.Status = StatusType.Active;
         ViewBag.Pages = this._context.Pages.OrderBy(r => r.PageID);
         model.PageRoleList = new List<PageRole>();
         return View("UserRoleInfo", model);
      }

      public IActionResult Update()
      {
         string idParam = this.RouteData.Values["id"].ToString();
         UserRole model = null;
         int recordId = -1;
         if (idParam != null && idParam != string.Empty)
         {
            recordId = Int32.Parse(idParam);
            model = this._context.UserRoles.Include(s => s.PageRoles).Where(c => c.UserRoleID == recordId).SingleOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลผู้ใช้งาน");
               return RedirectToAction("Index");
            }

            if (model.PageRoleList.Count <= 0)
               model.PageRoles = new Collection<PageRole>();
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลผู้ใช้งาน");
         }
         ViewBag.Pages = this._context.Pages.OrderBy(r => r.PageID);
         return View("UserRoleInfo", model);
      }


      [HttpPost]
      public IActionResult Modify(UserRole model)
      {
         if (ModelState.IsValid)
         {
            using (var dbContextTransaction = this._context.Database.BeginTransaction())
            {
               try
               {
                  model.Update_On = DateUtil.Now();
                  model.Update_By = this.HttpContext.User.Identity.Name;
                  if (model.UserRoleID <= 0)
                  {
                     model.Create_On = DateUtil.Now();
                     model.Create_By = this.HttpContext.User.Identity.Name;
                     var proles = new List<PageRole>();
                     foreach (var item in model.PageRoleList.Where(w => w.PageID > 0))
                     {
                        proles.Add(item);
                     }
                     model.PageRoles = proles;
                     this._context.UserRoles.Add(model);
                     this._context.SaveChanges();
                     dbContextTransaction.Commit();
                     return RedirectToAction("Index");

                  }
                  else
                  {
                     this._context.Update(model);
                     /*Product*/
                     IEnumerable<int> itemIDs = Enumerable.Empty<int>();
                     if (model.PageRoleList != null)
                     {
                        itemIDs = model.PageRoleList.Where(w => w.ID > 0 & w.PageID > 0).Select(s => s.ID);
                     }
                     foreach (var item in this._context.PageRoles.Where(w => w.UserRoleID == model.UserRoleID))
                     {
                        if (!itemIDs.Contains(item.ID))
                        {
                           this._context.PageRoles.Remove(item);
                           var tmpitem = model.PageRoles.Where(w => w.ID == item.ID).FirstOrDefault();
                           if (tmpitem != null)
                              model.PageRoles.Remove(tmpitem);
                        }
                     }

                     foreach (var item in model.PageRoleList)
                     {
                        item.UserRoleID = model.UserRoleID;
                        if (item.ID <= 0)
                           this._context.PageRoles.Add(item);

                     }
                     this._context.SaveChanges();

                     this._context.PageRoles.RemoveRange(this._context.PageRoles.Where(w => w.PageID <= 0));
                     this._context.SaveChanges();
                     dbContextTransaction.Commit();
                     return RedirectToAction("Index");

                  }
               }
               catch (Exception ex)
               {
                  Console.WriteLine(ex.ToString());
                  dbContextTransaction.Rollback();
                  ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล");
               }

            }

         }
         ViewBag.Pages = this._context.Pages.OrderBy(r => r.PageID);

         return View("UserRoleInfo", model);
      }


      public IActionResult Delete()
      {
         string idParam = this.RouteData.Values["id"].ToString();
         UserRole model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.UserRoles.Include(i => i.PageRoles).Where(b => b.UserRoleID == recordId).SingleOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               this._context.PageRoles.RemoveRange(model.PageRoles);
               this._context.UserRoles.Remove(model);
               this._context.SaveChanges();

            }
         }
         return RedirectToAction("Index");
      }


   }
}