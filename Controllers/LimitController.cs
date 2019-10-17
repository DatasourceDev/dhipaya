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
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers.Admin
{
   public class LimitController : ControllerBase
   {
      public LimitController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<LimitController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
         var model = this._context.PointLimits.FirstOrDefault();
         if (model == null)
         {
            model = new PointLimit();
         }
         return View("LimitInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(PointLimit model)
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
               this._context.PointLimits.Add(model);
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
         else
         {
            ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล");
         }
         return View("LimitInfo", model);
      }




   }
}