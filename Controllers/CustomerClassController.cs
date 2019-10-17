using Dhipaya.DAL;
using System.Linq;
using Dhipaya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Dhipaya.Services;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dhipaya.Extensions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using System.Net;
using OfficeOpenXml;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{
   

   public class CustomerClassController : ControllerBase
   {
      public CustomerClassController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<CustomerClassController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

         var model = this._context.CustomerClasses.OrderBy(c => c.ID);
         return View("CustomerClass", model);
      }
      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = new CustomerClass();
         model.Status = StatusType.Active;
         return View("CustomerClassInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.CustomerClasses
                        .Where(w => w.ID == id).FirstOrDefault();

         return View("CustomerClassInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(CustomerClass model)
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
               this._context.CustomerClasses.Add(model);
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

         return View("CustomerClassInfo", model);
      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         CustomerClass model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.CustomerClasses.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               this._context.CustomerClasses.Remove(model);
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }
      #endregion
   }
}