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
   public class UserController : ControllerBase
   {
      public UserController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<UserController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

      public IActionResult Index(UserDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         model.Users = this._context.Users
                .Include(u => u.UserRole)
                .Where(w => w.UserRole.RoleName != RoleName.Member & w.UserRole.RoleName != RoleName.Merchant)
                .OrderBy(r => new { r.UserRole.UserRoleID, r.UserName });

         ViewBag.ItemCount = model.Users.Count();
         return View(model);
      }

      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         User model = new User();
         model.Status = UserStatusType.Active;
         ViewBag.Roles = this._context.UserRoles.OrderBy(r => r.UserRoleID);
         ViewBag.UserRoles = this._context.UserRoles.Where(w => w.Status == StatusType.Active).OrderBy(r => r.UserRoleID);
         return View("UserInfo", model);
      }

      public IActionResult Update()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         string idParam = this.RouteData.Values["id"].ToString();
         User model = null;
         int recordId = -1;
         if (idParam != null && idParam != string.Empty)
         {
            recordId = Int32.Parse(idParam);
            model = this._context.Users
                .Where(c => c.ID == recordId).SingleOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลผู้ใช้งาน");
               return RedirectToAction("Index");
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลผู้ใช้งาน");
         }
         ViewBag.Roles = this._context.UserRoles.OrderBy(r => r.UserRoleID);
         ViewBag.UserRoles = this._context.UserRoles.Where(w => w.Status == StatusType.Active).OrderBy(r => r.UserRoleID);
         return View("UserInfo", model);
      }


      [HttpPost]
      public IActionResult Modify(User model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         if (this.isExistUserName(model))
            ModelState.AddModelError("UserName", "รหัสผู้ใช้ซ้ำในระบบ");

         if (ModelState.IsValid)
         {
            model.Update_On = DateUtil.Now();
            model.Update_By = this.HttpContext.User.Identity.Name;
            if (model.ID <= 0)
            {
               model.Password = DataEncryptor.Encrypt(model.Password);
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;

               this._context.Users.Add(model);
               this._context.SaveChanges();
            }
            else
            {
               this._context.Users.Attach(model);
               this._context.Entry(model).Property(u => u.Email).IsModified = true;
               this._context.Entry(model).Property(u => u.PhoneNumber).IsModified = true;
               this._context.Entry(model).Property(u => u.FirstName).IsModified = true;
               this._context.Entry(model).Property(u => u.LastName).IsModified = true;
               this._context.Entry(model).Property(u => u.Status).IsModified = true;
               this._context.Entry(model).Property(u => u.UserName).IsModified = true;
               this._context.Entry(model).Property(u => u.UserRoleID).IsModified = true;
               this._context.Entry(model).Property(u => u.Update_By).IsModified = true;
               this._context.Entry(model).Property(u => u.Update_On).IsModified = true;

               this._context.SaveChanges();
            }
            return RedirectToAction("Index");
         }
         ViewBag.Roles = this._context.UserRoles.OrderBy(r => r.UserRoleID);
         ViewBag.UserRoles = this._context.UserRoles.Where(w => w.Status == StatusType.Active).OrderBy(r => r.UserRoleID);
         return View("UserInfo", model);
      }

      private bool isSuperUser(string userName)
      {
         return userName.ToLower() == "admin";

      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         string idParam = this.RouteData.Values["id"].ToString();
         User model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Users.Where(b => b.ID == recordId).SingleOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (!this.isSuperUser(model.UserName))
               {
                  this._context.Users.Remove(model);
                  this._context.SaveChanges();
               }
               else
               {
                  ModelState.AddModelError("Error", "ไม่สามารถลบข้อมูล");
               }
            }
         }
         return RedirectToAction("Index");
      }

      #region  Reset Pwd      
      //public IActionResult Forgot()
      //{
      //   var model = new Forgot();
      //   return View(model);
      //}

      //[HttpPost]
      //public async Task<IActionResult> Forgot(Forgot model)
      //{
      //   if (ModelState.IsValid)
      //   {
      //      var customer = this._context.Customers.Include(i => i.User).Where(c => c.User.UserName == model.Email).FirstOrDefault();
      //      if (customer != null)
      //      {
      //         model.Customer = customer;
      //         var rg = new RijndaelCrypt();
      //         model.Url = Url.Action("ResetPwd", new { u = rg.Encrypt(customer.User.UserName) });
      //         var htmlToConvert = await RenderViewAsync("MailForgotPwd", model, true);
      //         var msg = EmailUtil.sendNotificationEmail(_smtp, customer.Email, "เปลี่ยนรหัสผ่าน", htmlToConvert.ToString());
      //         ViewData["Message"] = "ระบบกำลังส่่งการกำหนดรหัสผ่านใหม่ไปยังอีเมลของท่าน";
      //         return View(model);
      //      }
      //      ViewData["ErrorMessage"] = "ไม่พบอีเมลในระบบ";
      //   }
      //   else
      //      ViewData["ErrorMessage"] = "โปรดระบุอีเมล";

      //   return View(model);
      //}


      [HttpGet]
      public IActionResult ResetPwd()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
         }
         ResetPwdDTO model = new ResetPwdDTO();
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var user = this._context.Users.Where(c => c.ID == recordId).FirstOrDefault();
            if (user == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Index");
            }
            else
            {
               model.ID = user.ID;
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
            return RedirectToAction("Index", "Home");
         }
         return View("ResetPwd", model);
      }

      [HttpGet]
      public IActionResult ResetPwdO()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
         }
         ResetPwdDTO model = new ResetPwdDTO();
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var user = this._context.Users.Where(c => c.ID == recordId).FirstOrDefault();
            if (user == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Index");
            }
            else
            {
               model.ID = user.ID;
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
            return RedirectToAction("Index", "Home");
         }
         return View("ResetPwdO", model);
      }

      [HttpPost]
      public IActionResult ResetPwd(ResetPwdDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         if (ModelState.IsValid)
         {
            try
            {
               var user = this._context.Users.Where(w => w.ID == model.ID).FirstOrDefault();

               if (ModelState.IsValid)
               {
                  if (!string.IsNullOrEmpty(model.password))
                  {
                     user.Password = DataEncryptor.Encrypt(model.password);
                     user.Update_On = DateUtil.Now();
                     user.Update_By = this.HttpContext.User.Identity.Name;
                  }

                  this._context.Users.Attach(user);
                  this._context.Entry(user).Property(u => u.Password).IsModified = true;
                  this._context.Entry(user).Property(u => u.Update_On).IsModified = true;
                  this._context.Entry(user).Property(u => u.Update_By).IsModified = true;
                  this._context.SaveChanges();

                  return RedirectToAction("Update", new { ID = model.ID });
               }
            }
            catch
            {

            }
         }
         return View(model);
      }
      [HttpPost]
      public IActionResult ResetPwdO(ResetPwdDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
             return RedirectToAction("Login", "Accounts");

         if (ModelState.IsValid)
         {
            try
            {
               var user = this._context.Users.Where(w => w.ID == model.ID).FirstOrDefault();
               if (model.oldpassword == model.password)
               {
                  ModelState.AddModelError("oldpassword", "รหัสผ่านใหม่เหมือนกับรหัสผ่านเดิม");
                  ModelState.AddModelError("password", "รหัสผ่านใหม่เหมือนกับรหัสผ่านเดิม");
               }
               if (model.oldpassword != DataEncryptor.Decrypt(user.Password))
               {
                  ModelState.AddModelError("oldpassword", "รหัสผ่านเดิมไม่ถูกต้อง");
               }
               if (ModelState.IsValid)
               {
                  if (!string.IsNullOrEmpty(model.password))
                  {
                     user.Password = DataEncryptor.Encrypt(model.password);
                     user.Update_On = DateUtil.Now();
                     user.Update_By = this.HttpContext.User.Identity.Name;
                  }

                  this._context.Users.Attach(user);
                  this._context.Entry(user).Property(u => u.Password).IsModified = true;
                  this._context.Entry(user).Property(u => u.Update_On).IsModified = true;
                  this._context.Entry(user).Property(u => u.Update_By).IsModified = true;
                  this._context.SaveChanges();

                  return RedirectToAction("Update", new { ID = model.ID });
               }
            }
            catch
            {

            }
         }
         return View(model);
      }


      #endregion
   }
}