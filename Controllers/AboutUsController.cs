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
   public class AboutUsController : ControllerBase
   {
      public AboutUsController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<AboutUsController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

         var model = this._context.AboutUss.FirstOrDefault();
         if (model == null)
            model = new AboutUs();
         return View("AboutUs", model);
      }

      [HttpPost]
      public async Task<IActionResult> Modify(AboutUs model, IFormFile file, IFormFile video)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         if (ModelState.IsValid)
         {
            model.Update_On = DateUtil.Now();
            model.Update_By = this.HttpContext.User.Identity.Name;
            var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());

            if (model.ID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;

               this._context.AboutUss.Add(model);
               this._context.SaveChanges();

               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\AboutUs\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
               if (video != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\AboutUs\\";
                  string extension = Path.GetExtension(video.FileName);
                  var filename = webRoot + "video" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await video.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.VideoUrl = filename;
               }
               this._context.SaveChanges();
               return RedirectToAction("Index");

            }
            else
            {
               if (file != null)
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

                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\AboutUs\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
               if (video != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\AboutUs\\";
                  if (!string.IsNullOrEmpty(model.VideoUrl))
                  {
                     var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
                     var mfilename = model.VideoUrl.Replace("~", mwebRoot);
                     mfilename = mfilename.Replace("/", "\\");
                     if (System.IO.File.Exists(mfilename))
                     {
                        System.IO.File.Delete(mfilename);
                     }
                  }
                  string extension = Path.GetExtension(video.FileName);
                  var filename = webRoot + "video" + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await video.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.VideoUrl = filename;
               }
               this._context.Update(model);
               this._context.SaveChanges();
               return RedirectToAction("Index");
            }

         }
         else
         {
            ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล");
         }
         return View("AboutUs", model);
      }
   }
}