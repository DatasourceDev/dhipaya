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
   public class ProductController : ControllerBase
   {
      public ProductController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<ProductController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

         var model = this._context.Products.OrderBy(c => c.ProductCode);
         return View("Product", model);
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = new Product();
         model.Status = StatusType.Active;

         return View("ProductInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.Products
                        .Where(w => w.ProductID == id).FirstOrDefault();
         if (model != null)
         {
            ViewData["breadcrumb"] = model.ProductName;
         }

         return View("ProductInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(Product model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }

         if (!string.IsNullOrEmpty(model.ProductCode))
         {
            var query = this._context.Products.Where(c => c.ProductCode == model.ProductCode  & (model.ProductID > 0 ? c.ProductID != model.ProductID : true));
            if (!string.IsNullOrEmpty(model.SubProductCode))
               query = query.Where(w => w.SubProductCode == model.SubProductCode);

            if (query.FirstOrDefault() != null){
               ModelState.AddModelError("ProductCode", "Insurance Class ซ้ำในระบบ");
               if (!string.IsNullOrEmpty(model.SubProductCode))
                  ModelState.AddModelError("SubProductCode", "Sub Class ซ้ำในระบบ");
            }
               
         }


         if (ModelState.IsValid)
         {
            if (model.ProductID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;
               this._context.Products.Add(model);
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
         return View("ProductInfo", model);
      }


      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Product model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Products.Where(a => a.ProductID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               this._context.Products.Remove(model);
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }


   }
}