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
   public class NewsActivityController : ControllerBase
   {
      public NewsActivityController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<NewsActivityController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

         var model = this._context.NewsActivities.OrderBy(c => c.Index);
         return View("NewsActivity", model);
      }
      public IActionResult Favorite()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         NewsActivity model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.NewsActivities.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               model.IsFavorite = !model.IsFavorite;
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }

      public IActionResult MoveUp()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         NewsActivity model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.NewsActivities.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.NewsActivities.Where(w => w.Index < model.Index).OrderByDescending(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.NewsActivities.OrderBy(o => o.Index))
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
         NewsActivity model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.NewsActivities.Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.NewsActivities.Where(w => w.Index > model.Index).OrderBy(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.NewsActivities.OrderBy(o => o.Index))
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
         var model = new NewsActivity();
         model.Status = StatusType.Active;
         ViewBag.ListGroup = this._context.NewsActivityGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         return View("NewsActivityInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id, bool imgfocus)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.NewsActivities
                        .Include(i => i.NewsActivityImages)
                        .Where(w => w.ID == id).FirstOrDefault();
         if (model != null)
         {
            model.sDate = DateUtil.ToDisplayDate(model.StartDate);
            model.eDate = DateUtil.ToDisplayDate(model.EndDate);
         }
         ViewBag.imgfocus = imgfocus;
         ViewBag.ListGroup = this._context.NewsActivityGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         return View("NewsActivityInfo", model);
      }

      [HttpPost]
      public async Task<IActionResult> Modify(NewsActivity model, IFormFile file, IFormFile video)
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
               if (this._context.NewsActivities.Count() > 0)
               {
                  var lastindex = this._context.NewsActivities.Max(s => s.Index);
                  if (lastindex > 0)
                  {
                     model.Index = lastindex + 1;
                  }
               }
               this._context.NewsActivities.Add(model);
               this._context.SaveChanges();

               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivity\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.ID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.ImgUrl = filename;
               }
               if (video != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivity\\";
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
               return RedirectToAction("Update", new { id = model.ID, imgfocus = true });

            }
            else
            {
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;
               if (file != null)
               {
                  if (!string.IsNullOrEmpty(model.ImgUrl))
                  {
                     var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
                     var mfilename = model.ImgUrl.Replace("~", mwebRoot);
                     mfilename = mfilename.Replace("/", "\\");
                     if (System.IO.File.Exists(mfilename))
                     {
                        System.IO.File.Delete(mfilename);
                     }
                  }
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivity\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.ID + dateformat+ extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.ImgUrl = filename;
               }
               if (video != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivity\\";
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
         ViewBag.ListGroup = this._context.NewsActivityGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         return View("NewsActivityInfo", model);
      }


      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         NewsActivity model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.NewsActivities.Include(i => i.NewsActivityImages).Where(a => a.ID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (model.NewsActivityImages.Count() > 0)
               {
                  this._context.NewsActivityImages.RemoveRange(model.NewsActivityImages);
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivityGallery\\" + model.ID + "\\";
                  if (Directory.Exists(webRoot))
                  {
                     DirectoryInfo dir = new DirectoryInfo(webRoot);
                     foreach (FileInfo fi in dir.GetFiles())
                     {
                        fi.IsReadOnly = false;
                        fi.Delete();
                     }
                     Directory.Delete(webRoot);
                  }
               }

               this._context.NewsActivities.Remove(model);
               this._context.SaveChanges();
               var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               var filename = model.ImgUrl;
               if (!string.IsNullOrEmpty(filename))
               {
                  filename = filename.Replace("~", mwebRoot);
                  filename = filename.Replace("/", "\\");

                  if (System.IO.File.Exists(filename))
                  {
                     System.IO.File.Delete(filename);
                  }
               }
               filename = model.VideoUrl;
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
               foreach (var item in this._context.NewsActivities.OrderBy(o => o.Index))
               {
                  item.Index = i;
                  i++;
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }

      [HttpPost]
      public async Task<JsonResult> Image(NewsActivity model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return Json(new
            {
               responseCode = "-403",
               responseDesc = "คุณไม่มีสิทธิ์เข้าถึงระบบ",
            });
         }
         if (file != null && model.ID > 0)
         {
            var news = this._context.NewsActivities.Where(w => w.ID == model.ID).FirstOrDefault();
            if (news != null)
            {
               var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
               var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\NewsActivityGallery\\" + model.ID + "\\";
               if (!Directory.Exists(webRoot))
               {
                  Directory.CreateDirectory(webRoot);
               }
               if (file.Length > 0)
               {
                  var img = new NewsActivityImage()
                  {
                     NewsActivityID = model.ID,
                  };
                  this._context.NewsActivityImages.Add(img);
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
         }

         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }
      [HttpGet]
      public bool ImageDelete(int id)
      {
         if (id > 0)
         {
            var img = this._context.NewsActivityImages.Where(w => w.ID == id).FirstOrDefault();
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
               this._context.NewsActivityImages.Remove(img);
               this._context.SaveChanges();
               return true;

            }
         }
         return false;
      }


   }

}