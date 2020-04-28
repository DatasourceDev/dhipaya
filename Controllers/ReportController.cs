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
using OfficeOpenXml;
using Dhipaya.ModelsDapper;
using System.Net;
using Microsoft.Extensions.FileProviders;

namespace Dhipaya.Controllers.Admin
{
   public class ReportController : ControllerBase
   {

      public ReportController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<ReportController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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

      public async Task<IActionResult> CalPoint(int id)
      {
         return Json(new { point = await _cusRepo.GetPoint(id) });
      }

      [HttpGet]
      public async Task<IActionResult> Customer(CustomersDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         model.search_user_type = UserLevelType.Member;
         model.orderby = "ID desc";
         var customers = await _cusRepo.List(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Customers = customers;
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("Customer", model);

      }
      [HttpGet]
      public async Task<IActionResult> ExcelCustomer()
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";

         var filename = Directory.GetFiles(webRoot, "*.xlsx").Where(w => w.Contains("รายชื่อสมาชิก")).OrderByDescending(f => f).FirstOrDefault();
         if (filename != null)
         {
            //filename = filename.Replace(webRoot, "");
            IFileProvider provider = new PhysicalFileProvider(webRoot);
            if (System.IO.File.Exists(filename))
            {
               var memory = new MemoryStream();
               using (var stream = new FileStream(filename, FileMode.Open))
               {
                  await stream.CopyToAsync(memory);
               }
               memory.Position = 0;
               var mimeType = "application/vnd.ms-excel";
               return File(memory, mimeType, Path.GetFileName(filename));
            }
         }

         return null;
      }
      //[HttpGet]
      //public async Task<IActionResult> ExcelCustomer(CustomersDTO model)
      //{
      //   model.search_user_type = UserLevelType.Member;
      //   model.orderby = "ID desc";
      //   model.Customers = await _cusRepo.ListAll(model);

      //   var comlumHeadrs = new string[]
      //     {
      //         "หมายเลขสมาชิก",
      //         "ชื่อ",
      //         "นามสกุล",
      //         "อีเมล (รหัสผู้ใช้งาน)",
      //         "หมายเลขโทรศัพท์มือถือ",
      //         "หมายเลขบัตรประชาชน",
      //         "ประเภทสมาชิก",
      //         "ช่องทางการสมัคร",
      //         "ที่อยู่",
      //         "วันที่สมัคร",
      //         //"คะแนนสะสม"
      //     };

      //   byte[] result;

      //   using (var package = new ExcelPackage())
      //   {
      //      // add a new worksheet to the empty workbook
      //      var worksheet = package.Workbook.Worksheets.Add("รายชื่อสมาชิก"); //Worksheet name
      //      using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
      //      {
      //         cells.Style.Font.Bold = true;
      //      }

      //      //First add the headers
      //      for (var i = 0; i < comlumHeadrs.Count(); i++)
      //      {
      //         worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
      //      }

      //      //Add values
      //      var j = 2;
      //      foreach (var customer in model.Customers)
      //      {
      //         worksheet.Cells["A" + j].Value = customer.RefCode;
      //         worksheet.Cells["B" + j].Value = customer.NameTh;
      //         worksheet.Cells["C" + j].Value = customer.SurNameTh;
      //         worksheet.Cells["D" + j].Value = customer.User.UserName;
      //         worksheet.Cells["E" + j].Value = customer.MoblieNo;
      //         worksheet.Cells["F" + j].Value = customer.IDCard;
      //         worksheet.Cells["G" + j].Value = customer.CustomerClass != null ? customer.CustomerClass.Name : "TIP Silver";
      //         worksheet.Cells["H" + j].Value = customer.Channel.toName();
      //         worksheet.Cells["I" + j].Value = CustomerBinding.GetCustomerAddress(customer, this._context);
      //         worksheet.Cells["J" + j].Value = DateUtil.ToDisplayDateTime(customer.Create_On);
      //         //worksheet.Cells["K" + j].Value = customer.Point + customer.RedeemPoint;
      //         j++;
      //      }
      //      result = package.GetAsByteArray();
      //   }

      //   return File(result, "application/ms-excel", $"รายชื่อสมาชิก " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      //}

      [HttpGet]
      public async Task<IActionResult> CustomerClass(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         var cChanges = await _rptRepo.ListClassChange(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.CustomerClassChanges = cChanges;

         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         return View("CustomerClass", model);
      }

      [HttpGet]
      public IActionResult Invite(CustomersDTO model)
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


         model.Customers = this._context.Customers.Include(i => i.User).Include(i => i.CustomerClass)
             .Where(c => c.UserLevel == (int)UserLevelType.Member & !string.IsNullOrEmpty(c.FriendCode));

         if (model.customerClassID.HasValue)
            model.Customers = model.Customers.Where(c => c.CustomerClassID == model.customerClassID);

         if (model.customer_chanal.HasValue)
            model.Customers = model.Customers.Where(c => c.Channel == model.customer_chanal);
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Customers = model.Customers.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Customers = model.Customers.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Customers = model.Customers
               .Where(c => (!string.IsNullOrEmpty(c.NameTh) && c.NameTh.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.SurNameTh) && c.SurNameTh.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.User.UserName) && c.User.UserName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.MoblieNo) && c.MoblieNo.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.IDCard) && c.IDCard.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.NameEn) && c.NameEn.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.SurNameEn) && c.SurNameEn.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.RefCode) && c.RefCode.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.FriendCode) && c.FriendCode.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.NameTh) && !string.IsNullOrEmpty(c.SurNameTh) && (c.NameTh + c.SurNameTh).ToLower().Trim().Contains(text.Replace(" ", "")))

               );

         }
         int skipRows = (pageno - 1) * 100;

         model.Customers = model.Customers.OrderByDescending(c => c.Create_On);

         ViewBag.ItemCount = model.Customers.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Customers = model.Customers.Skip(skipRows).Take(100);

         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("Invite", model);

      }

      [HttpGet]
      public IActionResult ExcelInvite(CustomersDTO model)
      {
         model.Customers = this._context.Customers.Include(i => i.User).Include(i => i.CustomerClass)
                    .Where(c => c.UserLevel == (int)UserLevelType.Member & !string.IsNullOrEmpty(c.FriendCode));

         if (model.customerClassID.HasValue)
            model.Customers = model.Customers.Where(c => c.CustomerClassID == model.customerClassID);

         if (model.customer_chanal.HasValue)
            model.Customers = model.Customers.Where(c => c.Channel == model.customer_chanal);
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Customers = model.Customers.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Customers = model.Customers.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Customers = model.Customers
               .Where(c => (!string.IsNullOrEmpty(c.NameTh) && c.NameTh.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.SurNameTh) && c.SurNameTh.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.User.UserName) && c.User.UserName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.MoblieNo) && c.MoblieNo.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.IDCard) && c.IDCard.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.NameEn) && c.NameEn.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.SurNameEn) && c.SurNameEn.ToLower().Trim().Contains(text))
                  | (!string.IsNullOrEmpty(c.RefCode) && c.RefCode.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.FriendCode) && c.FriendCode.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.NameTh) && !string.IsNullOrEmpty(c.SurNameTh) && (c.NameTh + c.SurNameTh).ToLower().Trim().Contains(text.Replace(" ", "")))

               );

         }
         model.Customers = model.Customers.OrderByDescending(c => c.Create_On);

         var comlumHeadrs = new string[]
           {
               "หมายเลขสมาชิก",
               "Friend Code",
               "ชื่อ",
               "นามสกุล",
               "อีเมล (รหัสผู้ใช้งาน)",
               "หมายเลขโทรศัพท์มือถือ",
               "หมายเลขบัตรประชาชน",
               "ประเภทสมาชิก",
               "ช่องทางการสมัคร",
               "ที่อยู่",
               "วันที่สมัคร"
           };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("รายชื่อสมาชิก"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var customer in model.Customers)
            {
               worksheet.Cells["A" + j].Value = customer.RefCode;
               worksheet.Cells["B" + j].Value = customer.FriendCode;
               worksheet.Cells["C" + j].Value = customer.NameTh;
               worksheet.Cells["D" + j].Value = customer.SurNameTh;
               worksheet.Cells["E" + j].Value = customer.User.UserName;
               worksheet.Cells["F" + j].Value = customer.MoblieNo;
               worksheet.Cells["G" + j].Value = customer.IDCard;
               worksheet.Cells["H" + j].Value = customer.CustomerClass != null ? customer.CustomerClass.Name : "TIP Silver";
               worksheet.Cells["I" + j].Value = customer.Channel.toName();
               worksheet.Cells["J" + j].Value = CustomerBinding.GetCustomerAddress(customer, this._context);
               worksheet.Cells["K" + j].Value = DateUtil.ToDisplayDateTime(customer.Create_On);
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"รายชื่อสมาชิก " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public async Task<IActionResult> Privilege(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         var redeems = await _rptRepo.ListRedeem(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Redeems = redeems;


         var privileges = this._context.Privileges
                                     .Include(s => s.Merchant)
                                     .Include(s => s.PrivilegeImages)
                                     .Include(s => s.MerchantCategory)
                                     .Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         ViewBag.ListPrivilege = privileges.OrderBy(o => o.Merchant.MerchantName);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         ViewBag.ListCategorys = this._context.MerchantCategories.OrderBy(o => o.Index);

         return View("Privilege", model);
      }

      [HttpGet]
      public async Task<IActionResult> ExcelPrivilege(ReportDTO model)
      {
         model.Redeems = await _rptRepo.ListRedeemAll(model);

         var comlumHeadrs = new string[]
           {
               "หมายเลขสมาชิก",
               "ชื่อสมาชิก",
               "ประเภทสมาชิก",
               "รหัสรับสิทธิพิเศษ",
               "วันเวลาที่ใช้สิทธิพิเศษ",
               "ร้านค้า/บริการ",
               "สิทธิพิเศษ",
               "คะแนน",
           };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("รายการสิทธิพิเศษ"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var redeem in model.Redeems)
            {
               worksheet.Cells["A" + j].Value = redeem.Customer.RefCode;
               worksheet.Cells["B" + j].Value = redeem.Customer.NameTh + " " + redeem.Customer.SurNameTh;
               worksheet.Cells["C" + j].Value = redeem.CustomerClassName;
               worksheet.Cells["D" + j].Value = redeem.RedeemCode;
               worksheet.Cells["E" + j].Value = DateUtil.ToDisplayDateTime(redeem.RedeemDate);
               worksheet.Cells["F" + j].Value = redeem.MerchantName;
               worksheet.Cells["G" + j].Value = redeem.PrivilegeName;
               worksheet.Cells["H" + j].Value = redeem.Point;
               j++;
            }
            result = package.GetAsByteArray();
         }
         return File(result, "application/ms-excel", $"รายการสิทธิพิเศษ " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public async Task<IActionResult> PrivilegeDelivery(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         model.search_redeemtype = RedeemType.Delivery;
         var redeems = await _rptRepo.ListRedeem(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Redeems = redeems;


         var privileges = this._context.Privileges
                                     .Include(s => s.Merchant)
                                     .Include(s => s.PrivilegeImages)
                                     .Include(s => s.MerchantCategory)
                                     .Where(w => w.RedeemType == RedeemType.Delivery & w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         ViewBag.ListPrivilege = privileges.OrderBy(o => o.Merchant.MerchantName);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("PrivilegeDelivery", model);
      }

      [HttpGet]
      public async Task<IActionResult> ExcelPrivilegeDelivery(ReportDTO model)
      {
         model.search_redeemtype = RedeemType.Delivery;
         model.Redeems = await _rptRepo.ListRedeemAll(model);

         var comlumHeadrs = new string[]
           {
 "หมายเลขสมาชิก",
               "ชื่อสมาชิก",
               "ประเภทสมาชิก",
               "ที่อยู่จัดส่ง",
               "วันเวลาที่ใช้สิทธิพิเศษ",
               "ร้านค้า/บริการ",
               "สิทธิพิเศษ",
               "คะแนน",

           };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("รายการสิทธิพิเศษ"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var redeem in model.Redeems)
            {
               worksheet.Cells["A" + j].Value = redeem.Customer.RefCode;
               worksheet.Cells["B" + j].Value = redeem.Customer.NameTh + " " + redeem.Customer.SurNameTh;
               worksheet.Cells["C" + j].Value = redeem.CustomerClassName;
               worksheet.Cells["D" + j].Value = redeem.Address;
               worksheet.Cells["E" + j].Value = DateUtil.ToDisplayDateTime(redeem.RedeemDate);
               worksheet.Cells["F" + j].Value = redeem.MerchantName;
               worksheet.Cells["G" + j].Value = redeem.PrivilegeName;
               worksheet.Cells["H" + j].Value = redeem.Point;
               j++;
            }
            result = package.GetAsByteArray();
         }
         return File(result, "application/ms-excel", $"จัดส่งสิทธิพิเศษ" + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public async Task<IActionResult> RedeemRank(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         var ranks = await _rptRepo.ListRedeemRank(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;

         model.RedeemRanks = ranks;
         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         return View("RedeemRank", model);
      }

      [HttpGet]
      public async Task<IActionResult> InviteRank(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         var ranks = await _rptRepo.ListInviteRank(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;

         model.InviteRanks = ranks;
         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         return View("InviteRank", model);
      }
      [HttpGet]
      public IActionResult Contact(ReportDTO model)
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
         int skipRows = (pageno - 1) * 100;

         model.Contacts = this._context.Contacts;


         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Contacts = model.Contacts
               .Where(c => (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Title) && c.Title.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Information) && c.Information.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.ContactNo) && c.ContactNo.ToLower().Contains(text))
               );
         }
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Contacts = model.Contacts.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Contacts = model.Contacts.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }
         model.Contacts = model.Contacts.OrderByDescending(c => c.Create_On);

         ViewBag.ItemCount = model.Contacts.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Contacts = model.Contacts.Skip(skipRows).Take(100);

         return View("Contact", model);
      }

      [HttpGet]
      public IActionResult ExcelContact(ReportDTO model)
      {
         model.Contacts = this._context.Contacts;


         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Contacts = model.Contacts
               .Where(c => (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Title) && c.Title.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Information) && c.Information.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.ContactNo) && c.ContactNo.ToLower().Contains(text))
               );
         }
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Contacts = model.Contacts.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Contacts = model.Contacts.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }
         model.Contacts = model.Contacts.OrderByDescending(c => c.Create_On);
         var comlumHeadrs = new string[]
                  {
               "ชื่อ",
               "อีเมล",
               "เบอร์ติดต่อ",
               "วันเวลาที่ติดต่อ",
               "หัวข้อติดต่อ",
               "รายละเอียด",
                  };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("รายชื่อผู้ติดต่อ"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var contact in model.Contacts)
            {
               worksheet.Cells["A" + j].Value = contact.Name;
               worksheet.Cells["B" + j].Value = contact.Email;
               worksheet.Cells["C" + j].Value = contact.ContactNo;
               worksheet.Cells["D" + j].Value = DateUtil.ToDisplayDateTime(contact.Create_On);
               worksheet.Cells["E" + j].Value = contact.Title;
               worksheet.Cells["F" + j].Value = contact.Information;
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"รายชื่อผู้ติดต่อ " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public IActionResult Subscriber(ReportDTO model)
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
         int skipRows = (pageno - 1) * 100;

         model.Subscribers = this._context.Subscribers;


         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Subscribers = model.Subscribers.Where(c => (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text)));
         }
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Subscribers = model.Subscribers.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Subscribers = model.Subscribers.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }
         model.Subscribers = model.Subscribers.OrderByDescending(c => c.Create_On);

         ViewBag.ItemCount = model.Subscribers.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Subscribers = model.Subscribers.Skip(skipRows).Take(100);

         return View("Subscriber", model);
      }

      [HttpGet]
      public IActionResult ExcelSubscriber(ReportDTO model)
      {
         model.Subscribers = this._context.Subscribers;
         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower().Trim();
            model.Subscribers = model.Subscribers.Where(c => (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(text)));
         }
         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.Subscribers = model.Subscribers.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.Subscribers = model.Subscribers.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }
         model.Subscribers = model.Subscribers.OrderByDescending(c => c.Create_On);
         var comlumHeadrs = new string[]
                  {
               "อีเมล",
               "วันเวลาที่สมัคร",
                  };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("ผู้สมัครรับข่าวสาร"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var contact in model.Subscribers)
            {
               worksheet.Cells["A" + j].Value = contact.Email;
               worksheet.Cells["B" + j].Value = DateUtil.ToDisplayDateTime(contact.Create_On);
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"ผู้สมัครรับข่าวสาร " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public async Task<IActionResult> Point(ReportDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         var points = await _rptRepo.ListCustomerPoint(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.CustomerPoints = points;

         ViewBag.ListProduct = this._context.Products.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ProductCode);
         ViewBag.ListTransacionType = this._context.PointTransacionTypes.OrderBy(o => o.TransacionTypeID);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("Point", model);
      }

      [HttpGet]
      public async Task<IActionResult> ExcelPoint(ReportDTO model)
      {
         model.CustomerPoints = await _rptRepo.ListCustomerPointAll(model);

         var comlumHeadrs = new string[]
                  {
                     "หมายเลขสมาชิก",
                     "ชื่อสมาชิก",
                     "รหัส",
                     "Order No.",
                     "Policy No.",
                     "วันเวลาที่ได้รับคะแนน",
                     "วันเริ่มคุ้มครอง",
                     "วันที่สิ้นสุด",
                     "ราคาประกันภัย",
                     "ช่องทาง",
                     "รายละเอียด",
                     "คะแนน",
                  };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("คะแนนสะสม"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var point in model.CustomerPoints)
            {
               worksheet.Cells["A" + j].Value = point.Customer.RefCode;
               worksheet.Cells["B" + j].Value = point.Customer.NameTh + " " + point.Customer.SurNameTh;
               worksheet.Cells["C" + j].Value = point.Code;
               worksheet.Cells["D" + j].Value = point.OrderNo;
               worksheet.Cells["E" + j].Value = point.PolicyNo;
               worksheet.Cells["F" + j].Value = DateUtil.ToDisplayDateTime(point.Create_On);
               worksheet.Cells["G" + j].Value = DateUtil.ToDisplayDate(point.EffectiveDate);
               worksheet.Cells["H" + j].Value = DateUtil.ToDisplayDate(point.ExpiryDate);
               worksheet.Cells["I" + j].Value = point.PurchaseAmt;
               worksheet.Cells["J" + j].Value = point.Source;
               worksheet.Cells["K" + j].Value = point.Name;
               worksheet.Cells["L" + j].Value = point.Point;
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"คะแนนสะสม " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }

      [HttpGet]
      public IActionResult PrivilegeRank(ReportDTO model)
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
         int skipRows = (pageno - 1) * 100;

         model.PrivilegeRanks = this._context.Privileges
                      .Include(s => s.Merchant)
                      .Include(s => s.MerchantCategory)
                      .Include(s => s.Redeems);


         if (model.search_category_id.HasValue)
            model.PrivilegeRanks = model.PrivilegeRanks.Where(w => w.CategoryID == model.search_category_id);

         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower();
            model.PrivilegeRanks = model.PrivilegeRanks
               .Where(c => (!string.IsNullOrEmpty(c.PrivilegeName) && c.PrivilegeName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Merchant.MerchantName) && c.Merchant.MerchantName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Allowable_Outlet) && c.Allowable_Outlet.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeCondition) && c.PrivilegeCondition.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeDesc) && c.PrivilegeDesc.ToLower().Contains(text))
               );
         }


         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Redeems.Any(a => a.RedeemDate.Value.Date >= sdate.Value.Date));
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Redeems.Any(a => a.RedeemDate.Value.Date <= edate.Value.Date));
         }
         model.PrivilegeRanks = model.PrivilegeRanks.OrderByDescending(c => c.Redeems.Count());

         ViewBag.ItemCount = model.PrivilegeRanks.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.PrivilegeRanks = model.PrivilegeRanks.Skip(skipRows).Take(100);
         ViewBag.ListCategory = this._context.MerchantCategories.OrderBy(o => o.Index);

         return View("PrivilegeRank", model);
      }

      [HttpGet]
      public IActionResult ExcelPrivilegeRank(ReportDTO model)
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
         int skipRows = (pageno - 1) * 100;

         model.PrivilegeRanks = this._context.Privileges
                      .Include(s => s.Merchant)
                      .Include(s => s.MerchantCategory)
                      .Include(s => s.Redeems);


         if (model.search_category_id.HasValue)
            model.PrivilegeRanks = model.PrivilegeRanks.Where(w => w.CategoryID == model.search_category_id);

         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Create_On.Value.Date >= sdate.Value.Date);
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Create_On.Value.Date <= edate.Value.Date);
         }

         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower();
            model.PrivilegeRanks = model.PrivilegeRanks
               .Where(c => (!string.IsNullOrEmpty(c.PrivilegeName) && c.PrivilegeName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Merchant.MerchantName) && c.Merchant.MerchantName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Allowable_Outlet) && c.Allowable_Outlet.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeCondition) && c.PrivilegeCondition.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeDesc) && c.PrivilegeDesc.ToLower().Contains(text))
               );
         }


         if (!string.IsNullOrEmpty(model.search_sdate))
         {
            var sdate = DateUtil.ToDate(model.search_sdate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Redeems.Any(a => a.RedeemDate.Value.Date >= sdate.Value.Date));
         }
         if (!string.IsNullOrEmpty(model.search_edate))
         {
            var edate = DateUtil.ToDate(model.search_edate);
            model.PrivilegeRanks = model.PrivilegeRanks.Where(c => c.Redeems.Any(a => a.RedeemDate.Value.Date <= edate.Value.Date));
         }
         model.PrivilegeRanks = model.PrivilegeRanks.OrderByDescending(c => c.Redeems.Count());

         var comlumHeadrs = new string[]
                  {
                     "ลำดับ",
                     "ร้านค้า",
                     "สิทธิพิเศษ",
                     "ใช้สิทธิ์	",
                  };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("การแลกสิทธิพิเศษของร้านค้า / บริการ"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var item in model.PrivilegeRanks)
            {
               worksheet.Cells["A" + j].Value = j - 1;
               worksheet.Cells["B" + j].Value = item.Merchant.MerchantName;
               worksheet.Cells["C" + j].Value = item.PrivilegeName;
               worksheet.Cells["D" + j].Value = item.Redeems.Count;
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"การแลกสิทธิพิเศษของร้านค้าบริการ " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");

      }


      [HttpGet]
      public async Task<IActionResult> IIA(CustomersDTO model)
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
         model.pno = pageno;
         model.pmax = 100;
         model.search_user_type = UserLevelType.Member;
         model.orderby = "ID desc";
         var customers = await _cusRepo.List(model);

         ViewBag.ItemCount = model.totalrow;
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Customers = customers;
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("IIA", model);

      }


      [HttpPost]
      public JsonResult IIACheck(string name, string surname, string idc)
      {
         if (string.IsNullOrEmpty(name))
         {
            return Json(new
            {
               responseCode = "-1",
               responseresult = "Name is not found"
            });
         }
         if (string.IsNullOrEmpty(surname))
         {
            return Json(new
            {
               responseCode = "-1",
               responseresult = "Surname is not found"
            });
         }
         if (string.IsNullOrEmpty(idc))
         {
            return Json(new
            {
               responseCode = "-1",
               responseresult = "ID Card is not found"
            });
         }
         if (_conf.Environment == "Dev")
         {
            return Json(GetRequestPolicyActiveDev(idc, name, surname));
         }
         else
         {
            return Json(GetRequestPolicyActive(_iia, idc, name, surname));
         }

      }


      private List<IIAExpireDTO> GetIIAExpireData(List<IIAExpireDTO> datas, string filename)
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";

         var cnt = 1;
         using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Open))
         using (StreamReader reader = new StreamReader(fileStream))
         {
            string text = reader.ReadToEnd();
            var lines = text.Split("\n");
            foreach (var line in lines)
            {
               if (cnt >= 2)
               {
                  if (!string.IsNullOrEmpty(line))
                  {
                     var i = 0;
                     var raws = line.Split("|");
                     if (raws.Length == 34)
                     {
                        var data = new IIAExpireDTO();
                        data.typereport = raws[i]; i++;
                        data.transdate = DateUtil.ToDisplayDate(DateUtil.ToDate(raws[i], monthfirst: true)); i++;
                        data.insuranceclass = raws[i]; i++;
                        data.subclass = raws[i]; i++;
                        data.effectivedate = DateUtil.ToDisplayDate(DateUtil.ToDate(raws[i], monthfirst: true)); i++;
                        data.expirydate = DateUtil.ToDisplayDate(DateUtil.ToDate(raws[i], monthfirst: true)); i++;
                        data.newpolicyno = raws[i]; i++;
                        data.oldpolicyno = raws[i]; i++;
                        data.outletcode = raws[i]; i++;
                        data.invoiceno = raws[i]; i++;
                        data.newnetgprem = raws[i]; i++;
                        data.newduty = raws[i]; i++;
                        data.tax = raws[i]; i++;
                        data.totalamount = raws[i]; i++;
                        data.paid = raws[i]; i++;
                        data.insuredname = raws[i]; i++;
                        data.t_title = raws[i]; i++;
                        data.t_firstname = raws[i]; i++;
                        data.t_lastname = raws[i]; i++;
                        data.idcardno = raws[i]; i++;
                        data.insuredaddress = raws[i]; i++;
                        data.contactaddress = raws[i]; i++;
                        data.projcode = raws[i]; i++;
                        data.projectdesc = raws[i]; i++;
                        data.customergroupid = raws[i]; i++;
                        data.custgroupname = raws[i]; i++;
                        data.packageid = raws[i]; i++;
                        data.packagename = raws[i]; i++;
                        data.dfbookno = raws[i]; i++;
                        data.dfcdno = raws[i]; i++;
                        data.departmentid = raws[i]; i++;
                        data.depname = raws[i]; i++;
                        data.licenseno = raws[i]; i++;
                        data.email = raws[i]; i++;
                        datas.Add(data);
                     }

                  }
               }
               cnt++;
            }
         }
         return datas;
      }

      [HttpGet]
      public IActionResult IIAExpire(ReportDTO model)
      {
         model.IIAExpires = new List<IIAExpireDTO>();
         if (!string.IsNullOrEmpty(model.search_sdate) | !string.IsNullOrEmpty(model.search_edate))
         {
            if (string.IsNullOrEmpty(model.search_edate))
               model.search_edate = model.search_sdate;

            if (string.IsNullOrEmpty(model.search_sdate))
               model.search_sdate = model.search_edate;

            var sdate = DateUtil.ToDate(model.search_sdate);
            var edate = DateUtil.ToDate(model.search_edate);
            var datas = new List<IIAExpireDTO>();
            var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
            var files = Directory.GetFiles(webRoot, "*.*", SearchOption.AllDirectories);

            while (sdate <= edate)
            {
               foreach (var filename in files.Where(w => w.Contains(DateUtil.ToInternalDate3(sdate))))
               {
                  datas = GetIIAExpireData(datas, filename);
                  break;
               }
               sdate = sdate.Value.AddDays(1);
            }
            if (!string.IsNullOrEmpty(model.search_text))
            {
               var stext = model.search_text.ToLower().Trim();

               datas = datas.Where(c => (!string.IsNullOrEmpty(c.t_firstname) && c.t_firstname.ToLower().Trim().Contains(stext))
                        | (!string.IsNullOrEmpty(c.t_lastname) && c.t_lastname.ToLower().Trim().Contains(stext))
                        | (!string.IsNullOrEmpty(c.email) && c.email.ToLower().Contains(stext))
                        | (!string.IsNullOrEmpty(c.t_firstname) && !string.IsNullOrEmpty(c.t_lastname) && (c.t_firstname + c.t_lastname).ToLower().Trim().Contains(stext.Replace(" ", "")))
                     ).ToList();
            }

            model.IIAExpires = datas.OrderByDescending(o => o.transdate).ThenBy(o2 => o2.t_firstname);
         }
         return View("IIAExpire", model);
      }


      [HttpGet]
      public IActionResult ExcelIIAExpire(ReportDTO model)
      {
         model.IIAExpires = new List<IIAExpireDTO>();
         if (!string.IsNullOrEmpty(model.search_sdate) | !string.IsNullOrEmpty(model.search_edate))
         {
            if (string.IsNullOrEmpty(model.search_edate))
               model.search_edate = model.search_sdate;

            if (string.IsNullOrEmpty(model.search_sdate))
               model.search_sdate = model.search_edate;

            var sdate = DateUtil.ToDate(model.search_sdate);
            var edate = DateUtil.ToDate(model.search_edate);
            var datas = new List<IIAExpireDTO>();

            //var url = "192.168.110.13";
            //var user = "TIPSociety";
            //var password = "T1234567*";
            //var ftpServerUrl = string.Concat(url, "");
            //var request = (FtpWebRequest)WebRequest.Create(ftpServerUrl);
            //request.Method = WebRequestMethods.Ftp.DownloadFile;
            //request.Credentials = new NetworkCredential(user, password);
            //using (var response = (FtpWebResponse)request.GetResponse())
            //using (var responseStream = response.GetResponseStream())
            //using (var memoryStream = new MemoryStream())
            //{
            //   responseStream?.CopyTo(memoryStream);
            //   //return memoryStream.ToArray();
            //}

            var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
            var files = Directory.GetFiles(webRoot, "*.*", SearchOption.AllDirectories);

            while (sdate <= edate)
            {
               foreach (var filename in files.Where(w => w.Contains(DateUtil.ToInternalDate3(sdate))))
               {
                  datas = GetIIAExpireData(datas, filename);
                  break;
               }
               sdate = sdate.Value.AddDays(1);
            }
            if (!string.IsNullOrEmpty(model.search_text))
            {
               var stext = model.search_text.ToLower().Trim();

               datas = datas.Where(c => (!string.IsNullOrEmpty(c.t_firstname) && c.t_firstname.ToLower().Trim().Contains(stext))
                        | (!string.IsNullOrEmpty(c.t_lastname) && c.t_lastname.ToLower().Trim().Contains(stext))
                        | (!string.IsNullOrEmpty(c.email) && c.email.ToLower().Contains(stext))
                        | (!string.IsNullOrEmpty(c.t_firstname) && !string.IsNullOrEmpty(c.t_lastname) && (c.t_firstname + c.t_lastname).ToLower().Trim().Contains(stext.Replace(" ", "")))
                     ).ToList();
            }

            model.IIAExpires = datas.OrderByDescending(o => o.transdate).ThenBy(o2 => o2.t_firstname);
         }
         var comlumHeadrs = new string[]
     {
               "ชื่อ",
               "นามสกุล",
               "รหัสบัตรประชาชน",
               "ประเภท",
               "วันที่ทำรายการ",
               "กรมธรรม์",
               "วันที่เริ่ม",
               "วันที่สิ้นสุด",
               "policy no.",
               "policy no.เดิม",
               "ช่องทางการขาย",
               "email"
     };

         byte[] result;

         using (var package = new ExcelPackage())
         {
            // add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.Add("สมาชิกหมดอายุ/ต่ออายุ"); //Worksheet name
            using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count()]) //(1,1) (1,5)
            {
               cells.Style.Font.Bold = true;
            }

            //First add the headers
            for (var i = 0; i < comlumHeadrs.Count(); i++)
            {
               worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
            }

            //Add values
            var j = 2;
            foreach (var item in model.IIAExpires)
            {
               worksheet.Cells["A" + j].Value = item.t_firstname;
               worksheet.Cells["B" + j].Value = item.t_lastname;
               worksheet.Cells["C" + j].Value = item.idcardno;
               worksheet.Cells["D" + j].Value = item.typereport;
               worksheet.Cells["E" + j].Value = item.transdate;
               worksheet.Cells["F" + j].Value = item.insuranceclass + " [" + item.subclass + "]";
               worksheet.Cells["G" + j].Value = item.effectivedate;
               worksheet.Cells["H" + j].Value = item.expirydate;
               worksheet.Cells["I" + j].Value = item.newpolicyno;
               worksheet.Cells["J" + j].Value = item.oldpolicyno;
               worksheet.Cells["K" + j].Value = item.outletcode;
               worksheet.Cells["L" + j].Value = item.email;
               j++;
            }
            result = package.GetAsByteArray();
         }

         return File(result, "application/ms-excel", $"สมาชิกหมดอายุ/ต่ออายุ " + DateUtil.ToDisplayFullDateTime(DateUtil.Now()) + ".xlsx");
      }
   }
}