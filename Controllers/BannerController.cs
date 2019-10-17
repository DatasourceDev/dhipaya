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
   public class BannerController : ControllerBase
   {
      public BannerController(ICustomerRepository cusRepo, IReportRepository rptRepo,ChFrontContext context, IOptions<SystemConf> conf, ILogger<BannerController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
         var model = this._context.Banners.OrderBy(c => c.Index);

         return View("Banner", model);
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = new Banner();
         model.Status = StatusType.Active;

         return View("BannerInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.Banners.Where(w => w.ID == id).FirstOrDefault();
         if (model != null)
         {
            model.sDate = DateUtil.ToDisplayDate(model.StartDate);
            model.eDate = DateUtil.ToDisplayDate(model.EndDate);
         }
         return View("BannerInfo", model);
      }

      [HttpPost]
      public async Task<IActionResult> Modify(Banner model, IFormFile mobile, IFormFile web)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         if (ModelState.IsValid)
         {
            model.StartDate = DateUtil.ToDate(model.sDate);
            model.EndDate = DateUtil.ToDate(model.eDate);
            var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
            if (model.ID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;
               model.Index = 1;
               if (this._context.Banners.Count() > 0)
               {
                  var lastindex = this._context.Banners.Max(s => s.Index);
                  if (lastindex > 0)
                  {
                     model.Index = lastindex + 1;
                  }
               }
               this._context.Banners.Add(model);
               this._context.SaveChanges();

               if (mobile != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Banner\\";
                  string extension = Path.GetExtension(mobile.FileName);
                  var filename = webRoot + "mb" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await mobile.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.MobileUrl = filename;
               }
               if (web != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Banner\\";
                  string extension = Path.GetExtension(web.FileName);
                  var filename = webRoot + "web" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await web.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }

               this._context.SaveChanges();
               return RedirectToAction("Index");
            }
            else
            {
               if (mobile != null)
               {
                  if (!string.IsNullOrEmpty(model.MobileUrl))
                  {
                     var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
                     var mfilename = model.MobileUrl.Replace("~", mwebRoot);
                     mfilename = mfilename.Replace("/", "\\");
                     if (System.IO.File.Exists(mfilename))
                     {
                        System.IO.File.Delete(mfilename);
                     }
                  }
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Banner\\";
                  string extension = Path.GetExtension(mobile.FileName);
                  var filename = webRoot + "mb" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await mobile.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.MobileUrl = filename;
               }
               if (web != null)
               {
                  if (!string.IsNullOrEmpty(model.Url))
                  {
                     var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
                     var mfilename = model.Url.Replace("~", mwebRoot);
                     mfilename = mfilename.Replace("/", "\\");
                     if (System.IO.File.Exists(mfilename))
                     {
                        System.IO.File.Delete(mfilename);
                     }
                  }
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Banner\\";
                  string extension = Path.GetExtension(web.FileName);
                  var filename = webRoot + "web" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await web.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
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
         return View("BannersInfo", model);
      }

      public IActionResult MoveUp()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Banner model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Banners.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Banners.Where(w => w.Index < model.Index).OrderByDescending(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Banners.OrderBy(o => o.Index))
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
         Banner model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Banners.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Banners.Where(w => w.Index > model.Index).OrderBy(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Banners.OrderBy(o => o.Index))
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

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Banner model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Banners.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               var filename = model.Url;

               this._context.Banners.Remove(model);
               this._context.SaveChanges();

               if (!string.IsNullOrEmpty(filename))
               {
                  filename = filename.Replace("~", mwebRoot);
                  filename = filename.Replace("/", "\\");

                  if (System.IO.File.Exists(filename))
                  {
                     System.IO.File.Delete(filename);
                  }
               }
               var i = 1;
               foreach (var item in this._context.Banners.OrderBy(o => o.Index))
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