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
   public class GalleryController : ControllerBase
   {
      public GalleryController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<GalleryController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
         var model = this._context.Galleries.OrderByDescending(c => c.ID);

         return View("Gallery", model);
      }
      [HttpGet]
      public bool ImageDelete(int id)
      {
         if (id > 0)
         {
            var img = this._context.Galleries.Where(w => w.ID == id).FirstOrDefault();
            if (img != null)
            {
               var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               if (!string.IsNullOrEmpty(img.Url))
               {
                  var filename = img.Url.Replace("~", webRoot);
                  filename = filename.Replace("/", "\\");
                  if (System.IO.File.Exists(filename))
                  {
                     System.IO.File.Delete(filename);
                  }
               }
               this._context.Galleries.Remove(img);
               this._context.SaveChanges();
               return true;

            }
         }
         return false;
      }

      [HttpPost]
      public async Task<JsonResult> Image(Gallery model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return Json(new
            {
               responseCode = "-403",
               responseDesc = "คุณไม่มีสิทธิ์เข้าถึงระบบ",
            });
         }
         if (file != null)
         {
            var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
            var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Gallery\\";
            if (!Directory.Exists(webRoot))
            {
               Directory.CreateDirectory(webRoot);
            }
            if (file.Length > 0)
            {
               var img = new Gallery()
               {
                  ID = model.ID,
               };
               this._context.Galleries.Add(img);
               this._context.SaveChanges();
               string extension = Path.GetExtension(file.FileName);
               var filename = webRoot + img.ID + dateformat + extension;
               using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
               {
                  await file.CopyToAsync(fileStream);
               }
               filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
               filename = filename.Replace("\\", "/");
               img.Url = filename;
               this._context.SaveChanges();

            }

         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }
   }
}