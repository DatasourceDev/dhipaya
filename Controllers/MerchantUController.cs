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
   public class MerchantUController : ControllerBase
   {
      private string[] extensionList = new string[] { ".jpg", ".gif", ".png", ".jpeg", ".bpm" };
      public MerchantUController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<MerchantUController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

      #region Report
      [HttpGet]
      public IActionResult Index(ReportDTO model)
      {
         var user = this._context.Users.Include(i => i.UserRole).Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
         if (user == null)
            return RedirectToAction("Login", "Accounts");

         if (user.UserRole.RoleName != RoleName.Merchant)
            return RedirectToAction("Login", "Accounts");

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
            .Where(w => w.Privilege.Merchant.UserID == user.ID);

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

         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         var merchant = _context.Merchants.Where(w => w.UserID == user.ID).FirstOrDefault();
         if(merchant != null)
            ViewBag.MerchantName = merchant.MerchantName;
         return View("Index", model);
      }

      [HttpGet]
      public IActionResult Confirm(int id)
      {
         var model = this._context.Redeems.Where(w => w.RedeemID == id).FirstOrDefault();
         if (model != null)
         {
            model.Confirmed = true;
            _context.SaveChanges();
           return Json(new
            {
               result = 1
            });
         }
         return Json(new { error = "ไม่พบข้อมูลสิทธิพิเศษ", result = -1 });
      }
      #endregion
   }
}