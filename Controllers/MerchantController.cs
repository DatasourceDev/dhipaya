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
   public class MerchantController : ControllerBase
   {
      private string[] extensionList = new string[] { ".jpg", ".gif", ".png", ".jpeg", ".bpm" };
      public MerchantController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<MerchantController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

      #region Merchant
      [HttpGet]
      public IActionResult Index(MerchantDTO model)
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


         model.Merchants = this._context.Merchants.OrderBy(c => c.MerchantName);
         if (model.CategoryID.HasValue)
            model.Merchants = model.Merchants.Where(w => w.CategoryID == model.CategoryID);

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower();
            model.Merchants = model.Merchants
               .Where(c => (!string.IsNullOrEmpty(c.MerchantName) && c.MerchantName.ToLower().Contains(text))
               );
         }

         ViewBag.ItemCount = model.Merchants.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 25);
         if (ViewBag.ItemCount % 25 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Merchants = model.Merchants.Skip(skipRows).Take(25);
         model.MerchantCategorys = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         return View("Merchant", model);
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = new Merchant();
         model.Status = StatusType.Active;
         ViewData["breadcrumb"] = "เพิ่มร้านค้า/บริการใหม่";

         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         ViewBag.ListProvinces = this._context.Provinces.OrderBy(b => b.ProvinceName);
         return View("MerchantInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.Merchants
                        .Include(i => i.User)
                        .Where(w => w.MerchantID == id).FirstOrDefault();
         if (model == null)
            return RedirectToAction("Index");

         if (model.User != null)
         {
            model.UserName = model.User.UserName;
            model.Password = DataEncryptor.Decrypt(model.User.Password);
         }

         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         ViewBag.ListProvinces = this._context.Provinces.OrderBy(b => b.ProvinceName);
         return View("MerchantInfo", model);
      }


      [HttpPost]
      public async Task<IActionResult> Modify(Merchant model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var uvali = new User() { UserName = model.UserName, ID = model.UserID.HasValue ? model.UserID.Value : 0 };
         if (this.isExistUserName(uvali))
            ModelState.AddModelError("UserName", "รหัสผู้ใช้งานซ้ำในระบบ");

         if (ModelState.IsValid)
         {
            var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
            if (model.MerchantID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;

               model.User = new User();
               model.User.Email = model.UserName;
               model.User.FirstName = model.MerchantName;
               model.User.LastName = "Admin";
               model.User.UserName = model.UserName;
               model.User.Password = DataEncryptor.Encrypt(model.Password);
               model.User.Status = UserStatusType.Active;
               model.User.Create_On = DateUtil.Now();
               model.User.Create_By = this.HttpContext.User.Identity.Name;
               model.User.Update_On = DateUtil.Now();
               model.User.Update_By = this.HttpContext.User.Identity.Name;
               var userrole = _context.UserRoles.Where(w => w.RoleName == RoleName.Merchant).FirstOrDefault();
               model.User.UserRoleID = userrole.UserRoleID;

               this._context.Merchants.Add(model);
               this._context.SaveChanges();

               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Merchant\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.MerchantID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
                  this._context.SaveChanges();
               }
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
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Merchant\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.MerchantID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.Url = filename;
               }
               model.Update_On = DateUtil.Now();
               model.Update_By = this.HttpContext.User.Identity.Name;

               if (!model.UserID.HasValue)
               {
                  model.User = new User();
                  model.User.Email = model.UserName;
                  model.User.FirstName = model.MerchantName;
                  model.User.LastName = "Admin";
                  model.User.UserName = model.UserName;
                  model.User.Password = DataEncryptor.Encrypt(model.Password);
                  model.User.Status = UserStatusType.Active;
                  model.User.Create_On = DateUtil.Now();
                  model.User.Create_By = this.HttpContext.User.Identity.Name;
                  model.User.Update_On = DateUtil.Now();
                  model.User.Update_By = this.HttpContext.User.Identity.Name;
                  var userrole = _context.UserRoles.Where(w => w.RoleName == RoleName.Merchant).FirstOrDefault();
                  model.User.UserRoleID = userrole.UserRoleID;
               }
               else
               {
                  var user = _context.Users.Where(w => w.ID == model.UserID).FirstOrDefault();
                  if (user != null)
                  {
                     user.UserName = model.UserName;
                     user.Password = DataEncryptor.Encrypt(model.Password);
                     this._context.Users.Attach(user);
                     this._context.Entry(user).Property(u => u.UserName).IsModified = true;
                     this._context.Entry(user).Property(u => u.Password).IsModified = true;
                  }


               }
               this._context.Update(model);
               this._context.SaveChanges();
               return RedirectToAction("Index");
            }
         }
         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         ViewBag.ListProvinces = this._context.Provinces.OrderBy(b => b.ProvinceName);
         return View("MerchantInfo", model);
      }


      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Merchant model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Merchants.Include(i => i.Privileges).Where(a => a.MerchantID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (model.Privileges.Count > 0)
               {
                  foreach (var item in model.Privileges)
                  {
                     var images = this._context.PrivilegeImages.Where(w => w.PrivilegeID == item.PrivilegeID);
                     if (images.Count() > 0)
                     {
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
                  }
                  this._context.Privileges.RemoveRange(model.Privileges);
               }
               var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               var filename = model.Url;

               this._context.Merchants.Remove(model);
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

      #region Report
      [HttpGet]
      public IActionResult Privilege(ReportDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
         if (user == null)
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
         int skipRows = (pageno - 1) * 100;

         model.Redeems = this._context.Redeems
            .Include(i => i.Customer)
            .Include(i => i.Customer.User)
            .Include(i => i.Customer.CustomerClass)
            .Include(i => i.Privilege)
            .Include(i => i.Privilege.Merchant)
            .Where(w=>w.Privilege.Merchant.UserID == user.ID);

         if (model.customerClassID.HasValue)
            model.Redeems = model.Redeems.Where(c => c.Customer.CustomerClassID == model.customerClassID);

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Redeems = model.Redeems
               .Where(c => (!string.IsNullOrEmpty(c.Customer.NameTh) && c.Customer.NameTh.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.SurNameTh) && c.Customer.SurNameTh.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.Email) && c.Customer.Email.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.User.UserName) && c.Customer.User.UserName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.MoblieNo) && c.Customer.MoblieNo.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.IDCard) && c.Customer.IDCard.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.NameEn) && c.Customer.NameEn.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.SurNameEn) && c.Customer.SurNameEn.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.RefCode) && c.Customer.RefCode.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Customer.FriendCode) && c.Customer.FriendCode.ToLower().Contains(text))
               );
         }
         if (model.search_privilege.HasValue)
         {
            model.Redeems = model.Redeems.Where(c => c.PrivilegeID == model.search_privilege);
         }
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Redeems = model.Redeems.Where(c => c.RedeemDate.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Redeems = model.Redeems.Where(c => c.RedeemDate.Value.Date <= edate.Value.Date);
         }
         model.Redeems = model.Redeems.OrderByDescending(c => c.RedeemDate);

         ViewBag.ItemCount = model.Redeems.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Redeems = model.Redeems.Skip(skipRows).Take(100);

         var privileges = this._context.Privileges
                                     .Include(s => s.Merchant)
                                     .Include(s => s.PrivilegeImages)
                                     .Include(s => s.MerchantCategory)
                                     .Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         ViewBag.ListPrivilege = privileges.OrderBy(o => o.Merchant.MerchantName);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("Privilege", model);
      }
      #endregion
   }
}