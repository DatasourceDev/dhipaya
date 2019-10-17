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
   public class CategoryController : ControllerBase
   {
      private string[] extensionList = new string[] { ".jpg", ".gif", ".png", ".jpeg", ".bpm" };
      public CategoryController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<CategoryController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
      #region Category
      [HttpGet]
      public IActionResult Index(CategoryDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         int pageno = 1;
         if (this.RouteData.Values["pno"] != null)
         {
            pageno = NumUtil.ParseInteger(this.RouteData.Values["pno"].ToString());
            if (pageno == 0)
               pageno = 1;
         }
         int skipRows = (pageno - 1) * 25;


         model.MerchantCategories = this._context.MerchantCategories.OrderBy(c => c.Index);

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower();
            model.MerchantCategories = model.MerchantCategories
               .Where(c => (!string.IsNullOrEmpty(c.CategoryName) && c.CategoryName.ToLower().Contains(text))
               );
         }

         ViewBag.ItemCount = model.MerchantCategories.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 25);
         if (ViewBag.ItemCount % 25 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.MerchantCategories = model.MerchantCategories.Skip(skipRows).Take(25);
         return View("Category", model);
      }

      public IActionResult MoveUp()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         MerchantCategory model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.MerchantCategories.Where(a => a.CategoryID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.MerchantCategories.Where(w => w.Index < model.Index).OrderByDescending(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.MerchantCategories.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.CategoryID == item.CategoryID)
                  {
                     latestindex.Index = i + 1;
                  }
                  else if (latestindex != null && model.CategoryID == item.CategoryID)
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
         MerchantCategory model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.MerchantCategories.Where(a => a.CategoryID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.MerchantCategories.Where(w => w.Index > model.Index).OrderBy(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.MerchantCategories.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.CategoryID == item.CategoryID)
                  {
                     latestindex.Index = i;
                     i += 2;
                  }
                  else if (latestindex != null && model.CategoryID == item.CategoryID)
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
         var model = new MerchantCategory();
         model.Status = StatusType.Active;

         return View("CategoryInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.MerchantCategories
                        .Where(w => w.CategoryID == id).FirstOrDefault();
         if (model != null)
         {
            ViewData["breadcrumb"] = model.CategoryName;
         }

         return View("CategoryInfo", model);
      }

      [HttpPost]
      public async Task<IActionResult> Modify(MerchantCategory model, IFormFile file, IFormFile logo)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         if (ModelState.IsValid)
         {
            if (model.CategoryID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Index = 1;
               if (this._context.MerchantCategories.Count() > 0)
               {
                  var lastindex = this._context.MerchantCategories.Max(s => s.Index);
                  if (lastindex > 0)
                  {
                     model.Index = lastindex + 1;
                  }
               }
               this._context.MerchantCategories.Add(model);
               this._context.SaveChanges();

               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Category\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.CategoryID + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
               if (logo != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Category\\";
                  string extension = Path.GetExtension(logo.FileName);
                  var filename = webRoot + "icon-" + model.CategoryID + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await logo.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Logo = filename;
               }
               this._context.SaveChanges();

               return RedirectToAction("Index");

            }
            else
            {
               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Category\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.CategoryID + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
               if (logo != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Category\\";
                  string extension = Path.GetExtension(logo.FileName);
                  var filename = webRoot + "icon-" + model.CategoryID + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await logo.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Logo = filename;
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
         return View("CategoryInfo", model);
      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         MerchantCategory model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.MerchantCategories.Where(a => a.CategoryID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var privileges = this._context.Privileges.Where(w => w.CategoryID == model.CategoryID);
               if (privileges.Count() > 0)
               {
                  foreach (var item in privileges)
                  {
                     var images = this._context.PrivilegeImages.Where(w => w.PrivilegeID == item.PrivilegeID);
                     if (images.Count() > 0)
                     {

                     }

                     this._context.PrivilegeImages.RemoveRange(images);

                     var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\" + item.PrivilegeID + "\\";
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
                  this._context.Privileges.RemoveRange(privileges);
               }
               var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               var filename = model.Url;

               this._context.MerchantCategories.Remove(model);
               this._context.SaveChanges();
               var i = 1;
               foreach (var item in this._context.MerchantCategories.OrderBy(o => o.Index))
               {
                  item.Index = i;
                  i++;
               }
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

            }
         }
         return RedirectToAction("Index");
      }
      #endregion
   }
}