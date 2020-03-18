using Dhipaya.DAL;
using System.Linq;
using Dhipaya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Dhipaya.Services;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dhipaya.Extensions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using System.Net;
using OfficeOpenXml;
using Dhipaya.ModelsDapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Dhipaya.Controllers
{
   public class CustomerController : ControllerBase
   {

      public CustomerController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<CustomerController> logger, IOptions<Smtp> smtp, IOptions<IIA> _iia, IOptions<TIPMobile> _mobile, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._context = context;
         this._logger = logger;
         this._smtp = smtp.Value;
         this._iia = _iia.Value;
         this._mobile = _mobile.Value;
         this._conf = conf.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      [HttpGet]
      public IActionResult Import(CustomersImportDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }

         return View("Import", model);

      }

      [HttpPost]
      public async Task<IActionResult> Import(IFormFile excelfile, CustomersImportDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }

         if (!model.dosave)
         {
            model.valid = true;
            model.Imports = new List<CustomerImport>();
            var imports = new List<CustomerImport>();
            var errors = new List<CustomerImport>();

            if (excelfile == null)
               ModelState.AddModelError("Error", "ไม่พบข้อมูลไฟล์นำเข้า");
            else
            {
               using (var memoryStream = new MemoryStream())
               {
                  await excelfile.CopyToAsync(memoryStream).ConfigureAwait(false);
                  using (var package = new ExcelPackage(memoryStream))
                  {
                     var workbook = package.Workbook;
                     var worksheet = workbook.Worksheets.First();

                     var rowCount = worksheet.Dimension?.Rows;
                     var colCount = worksheet.Dimension?.Columns;
                     if (rowCount.HasValue && colCount.HasValue)
                     {
                        for (int row = 2; row <= rowCount.Value; row++)
                        {
                           try
                           {
                              var msg = new StringBuilder();

                              var rowno = worksheet.Cells["A" + row].Value != null ? worksheet.Cells["A" + row].Value.ToString().Trim() : "";
                              var title = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Value.ToString().Trim() : "";
                              var firstname = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Value.ToString().Trim() : "";
                              var lastname = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Value.ToString().Trim() : "";
                              var idcard = worksheet.Cells["E" + row].Value != null ? worksheet.Cells["E" + row].Value.ToString().Trim() : "";
                              var passport = worksheet.Cells["F" + row].Value != null ? worksheet.Cells["F" + row].Value.ToString().Trim() : "";
                              var tel = worksheet.Cells["G" + row].Value != null ? worksheet.Cells["G" + row].Value.ToString().Trim() : "";
                              var email = worksheet.Cells["H" + row].Value != null ? worksheet.Cells["H" + row].Value.ToString().Trim() : "";

                              var haverow = false;
                              for (int col = 1; col <= colCount.Value; col++)
                              {
                                 if (worksheet.Cells[row, col].Value != null && !string.IsNullOrEmpty(worksheet.Cells[row, col].Value.ToString()))
                                 {
                                    haverow = true;
                                    break;
                                 }
                              }
                              if (!haverow)
                                 continue;

                              var cus = new CustomerImport();
                              cus.No = rowno;
                              cus.row = row + 1;
                              cus.NameTh = firstname;
                              cus.SurNameTh = lastname;
                              cus.IDCard = idcard;
                              cus.MoblieNo = tel;
                              cus.Email = email;
                              cus.Passport = passport;
                              cus.succeed = true;

                              if (!string.IsNullOrEmpty(cus.MoblieNo))
                              {
                                 if (cus.MoblieNo.Substring(0, 1) != "0")
                                    cus.MoblieNo = "0" + cus.MoblieNo;
                              }

                              if (string.IsNullOrEmpty(rowno))
                                 msg.AppendLine("ไม่พบข้อมูล row no.");

                              if (string.IsNullOrEmpty(cus.NameTh))
                                 msg.AppendLine("ไม่พบข้อมูลชื่อ");
                              else
                                 cus.NameTh = cus.NameTh.Trim();

                              if (string.IsNullOrEmpty(cus.SurNameTh))
                                 msg.AppendLine("ไม่พบข้อมูลนามสกุล");
                              else
                                 cus.SurNameTh = cus.SurNameTh.Trim();

                              if (string.IsNullOrEmpty(email))
                                 msg.AppendLine("ไม่พบข้อมูลอีเมล");

                              var prefix = _context.CustomerPrefixs.Where(w => w.Name.ToLower() == title.ToLower() | (!string.IsNullOrEmpty(w.NameEng) ? w.NameEng.ToLower() : "") == title.ToLower()).FirstOrDefault();
                              if (prefix == null)
                              {
                                 prefix = _context.CustomerPrefixs.Where(w => w.Name == "นาย").FirstOrDefault();
                              }
                              cus.PrefixTh = prefix.Name;
                              cus.PrefixID = prefix.ID;

                              if (!EmailUtil.EmailIsValid(cus.Email))
                                 msg.AppendLine("รูปแบบอีเมลไม่ถูกต้อง");

                              var dup = imports.Where(w => w.Email == email).FirstOrDefault();
                              if (dup != null)
                                 continue;


                              if (!string.IsNullOrEmpty(idcard))
                              {
                                 var dup3 = imports.Where(w => w.IDCard == idcard).FirstOrDefault();
                                 if (dup3 != null)
                                    continue;
                              }

                              if (msg != null && !string.IsNullOrEmpty(msg.ToString()))
                              {
                                 model.valid = false;
                                 cus.succeed = false;
                                 errors.Add(cus);
                                 cus.Msg = msg.ToString();
                              }
                              else
                                 imports.Add(cus);
                           }
                           catch (Exception ex)
                           {
                              ModelState.AddModelError("Error", ex.Message);
                           }

                        }
                        if (errors.Count() > 0)
                           model.Imports = errors;
                        else
                           model.Imports = imports;
                        if (model.valid)
                        {
                           _context.CustomerImpts.RemoveRange(_context.CustomerImpts);
                           _context.CustomerImpts.AddRange(model.Imports.Select(s => new CustomerImpt()
                           {
                              Email = s.Email,
                              IDCard = s.IDCard,
                              MoblieNo = s.MoblieNo,
                              NameTh = s.NameTh,
                              SurNameTh = s.SurNameTh,
                              PrefixTh = s.PrefixTh,
                              PrefixID = s.PrefixID,
                              No = s.No,
                              Passport = s.Passport
                           }));
                           _context.SaveChanges();
                        }
                     }
                     else
                     {
                        ModelState.AddModelError("Error", "รูปแบบไฟล์ไม่ถูกต้องหรือไม่มีข้อมูล");
                     }
                  }
               }

            }
         }
         else
         {
            model.result = ImportCustomers();
            return RedirectToAction("Import", new CustomersImportDTO { result = model.result });
         }

         return View("Import", model);
      }

      private bool ImportCustomers()
      {
         try
         {
            var List = new List<Customer>();
            foreach (var imp in _context.CustomerImpts.Where(w => w.Imported == false & w.Msg == null).OrderBy(o => o.ID))
            {
               imp.Imported = true;
               var dup1 = _context.Users.Where(w => w.UserName == imp.Email).FirstOrDefault();
               if (dup1 != null)
                  continue;

               if (!string.IsNullOrEmpty(imp.IDCard))
               {
                  var dup4 = _context.Customers.Where(w => w.IDCard == imp.IDCard).FirstOrDefault();
                  if (dup4 != null)
                     continue;
               }

               var cus = new Customer()
               {
                  Create_By = "Migrate",
                  Update_By = "Migrate",
                  Email = imp.Email,
                  IDCard = imp.IDCard,
                  Passport = imp.Passport,
                  MoblieNo = imp.MoblieNo,
                  UserLevel = UserLevelType.Member,
                  Status = UserStatusType.Active,
                  PrefixTh = imp.PrefixID,
                  NameTh = imp.NameTh,
                  SurNameTh = imp.SurNameTh,
                  Create_On = DateUtil.Now(),
                  Update_On = DateUtil.Now(),
                  Channel = CustomerChanal.DhiMemberImport,
                  Syned = true,
                  DoSendReisterEmail = true,
                  SentReisterEmail = false,
                  CustomerClassID = 1,
                  IIASyned = false,
               };

               cus.User = new User();
               cus.User.Email = imp.Email;
               cus.User.PhoneNumber = imp.MoblieNo;
               cus.User.UserName = imp.Email;
               cus.User.UserRoleID = 2;
               cus.User.Status = UserStatusType.Active;
               cus.User.Create_On = cus.Create_On;
               cus.User.Create_By = cus.Create_By;
               cus.User.Update_On = cus.Update_On;
               cus.User.Update_By = cus.Update_By;
               if (string.IsNullOrEmpty(cus.User.Password))
                  cus.User.Password = DataEncryptor.Encrypt("Admin1234!");

               if (string.IsNullOrEmpty(cus.User.Email))
               {
                  cus.Email = imp.Email;
                  cus.User.Email = imp.Email;
               }
               List.Add(cus);
            }
            List.ForEach(s => _context.Customers.Add(s));
            _context.SaveChanges();

            foreach (var item in List.Where(w => string.IsNullOrEmpty(w.RefCode)))
            {
               item.RefCode = CustomerBinding.GetRefCode(item);
               item.User.Password = DataEncryptor.Encrypt(item.ID.ToString("00000000"));
            }
            _context.SaveChanges();
         }
         catch (Exception ex)
         {
            return false;
         }
         return true;
      }


      [HttpGet]
      public async Task<IActionResult> TemplateCustomer()
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
         var filename = webRoot + "customer_template.xlsx";

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
         return null;
      }

      [HttpGet]
      public async Task<IActionResult> Index(CustomersDTO model)
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

      private string RedeemCode(int? ID)
      {
         Random rnd = new Random();
         var code = "18";
         code += ID;
         for (var i = 0; code.Length < 9; i++)
         {
            code += rnd.Next(0, 9);
         }
         return code;
      }

      [HttpPost]
      public async Task<IActionResult> Redeem(int? pID, string popaddress)
      {
         var customer = this._context.Customers.Include(i => i.CustomerClass).Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
         if (customer != null)
         {
            var model = this._context.Privileges.Include(i => i.Merchant).Where(w => w.PrivilegeID == pID).FirstOrDefault();
            if (model != null)
            {
               if (customer.Status == UserStatusType.BlockReward)
               {
                  return Json(new { result = 4 });
               }
               var point = this._context.CustomerPoints.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);
               var redeempoint = this._context.Redeems.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);

               var totalPoint = point - redeempoint;
               if (model.CreditPoint > 0 && (totalPoint < 0 | totalPoint - model.CreditPoint.Value < 0))
               {
                  return Json(new { result = 3 });
               }

               if (model.PerPrivilegePeriod != Period.None)
               {
                  var redeemed = this._context.Redeems.Where(w => w.PrivilegeID == model.PrivilegeID);

                  if (model.PerPrivilegePeriod == Period.Day)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddDays(-1));
                     if (redeemed.Count() >= model.PerPrivilegeLimitedDay)
                     {
                        return Json(new { result = 5 });
                     }
                  }
                  else if (model.PerPrivilegePeriod == Period.Week)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddDays(-7));
                     if (redeemed.Count() >= model.PerPrivilegeLimitedWeek)
                     {
                        return Json(new { result = 5 });
                     }
                  }
                  else if (model.PerPrivilegePeriod == Period.Month)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddMonths(-1));
                     if (redeemed.Count() >= model.PerPrivilegeLimitedMonth)
                     {
                        return Json(new { result = 5 });
                     }
                  }

               }

               if (model.PerPersonPeriod != Period.None)
               {
                  var redeemed = this._context.Redeems.Where(w => w.PrivilegeID == model.PrivilegeID & w.CustomerID == customer.ID);
                  if (model.PerPersonPeriod == Period.Once)
                  {
                     if (redeemed.Count() >= 1)
                     {
                        return Json(new { result = 2 });
                     }
                  }
                  else if (model.PerPersonPeriod == Period.Day)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddDays(-1));
                     if (redeemed.Count() >= model.PerPersonLimitedDay)
                     {
                        return Json(new { result = 2 });
                     }
                  }
                  else if (model.PerPersonPeriod == Period.Week)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddDays(-7));
                     if (redeemed.Count() >= model.PerPersonLimitedWeek)
                     {
                        return Json(new { result = 2 });
                     }
                  }
                  else if (model.PerPersonPeriod == Period.Month)
                  {
                     redeemed = redeemed.Where(w => w.RedeemDate >= DateUtil.Now().AddMonths(-1));
                     if (redeemed.Count() >= model.PerPersonLimitedMonth)
                     {
                        return Json(new { result = 2 });
                     }
                  }

               }
               var redeem = new Redeem();

               var prefix = "S";
               if (customer.CustomerClass != null)
               {
                  prefix = customer.CustomerClass.Prefix;
                  redeem.CustomerClassName = customer.CustomerClass.Name;
               }
               if (model.PrivilegeCodeType == PrivilegeCodeType.Custom)
               {
                  var codes = _context.PrivilegeCodes.Where(w => w.PrivilegeID == model.PrivilegeID & w.Status == StatusType.Active);
                  codes = codes.Where(w => w.EffectiveDate.HasValue ? w.EffectiveDate.Value.Date <= DateUtil.Now() : true);
                  codes = codes.Where(w => w.ExpiryDate.HasValue ? w.ExpiryDate.Value.AddDays(1).Date >= DateUtil.Now() : true);

                  foreach (var code in codes)
                  {
                     if (!code.MaxUse.HasValue)
                     {
                        redeem.RedeemCode = code.Code;
                        break;
                     }
                     else
                     {
                        var cnt = _context.Redeems.Where(w => w.RedeemCode == code.Code & w.PrivilegeID == model.PrivilegeID).Count();
                        if (cnt < code.MaxUse)
                        {
                           redeem.RedeemCode = code.Code;
                           code.Used += 1;
                           break;
                        }
                     }

                  }

               }
               else
               {
                  if (model.Quantity > 0)
                  {
                     var redeemcnt = this._context.Redeems.Where(w => w.PrivilegeID == model.PrivilegeID).Count();
                     if (redeemcnt + 1 > model.Quantity.Value)
                     {
                        return Json(new { result = 5 });
                     }
                  }

                  redeem.RedeemCode = prefix + RedeemCode(customer.ID);
                  while (this._context.Redeems.Where(w => w.RedeemCode == redeem.RedeemCode).FirstOrDefault() != null)
                  {
                     redeem.RedeemCode = prefix + RedeemCode(customer.ID);
                  }
               }
               if (string.IsNullOrEmpty(redeem.RedeemCode))
               {
                  return Json(new { result = 5 });
               }
               redeem.Address = popaddress;
               redeem.RedeemType = model.RedeemType;
               redeem.PrivilegeCodeType = model.PrivilegeCodeType;
               redeem.PrivilegeID = model.PrivilegeID;
               redeem.CustomerID = customer.ID;
               redeem.MerchantName = model.Merchant.MerchantName;
               redeem.PrivilegeName = model.PrivilegeName;
               redeem.Point = model.CreditPoint.HasValue ? model.CreditPoint.Value : 0;
               redeem.RedeemDate = DateUtil.Now();
               this._context.Redeems.Add(redeem);
               this._context.SaveChanges();
               if (!string.IsNullOrEmpty(redeem.Address))
               {
                  /*Send mail to admin*/
                  await MailRedeemSendAddrestoAdmin(customer, redeem);
               }
               var points = _context.CustomerPoints.Where(w => w.CustomerID == customer.ID);
               var redeems = this._context.Redeems.Where(w => w.CustomerID == customer.ID);
               return Json(new { result = 1, redeemcode = redeem.RedeemCode, date = DateUtil.ToDisplayDateTime(redeem.RedeemDate), totalpoint = NumUtil.FormatCurrency(points.Sum(s => s.Point) - redeems.Sum(s => s.Point)) });
            }
         }
         return Json(new { result = -1 });
      }

      [HttpPut]
      public JsonResult Reward(TransacionTypeID tID)
      {
         var customer = this._context.Customers.Include(i => i.User).Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
         if (customer == null)
         {
            return Json(new
            {
               responseCode = "-1",
            });
         }

         var totalpoint = 0M;
         var regs = this.GetPointCondition(customer, tID);
         if (regs != null)
         {
            foreach (var item in regs)
            {
               var p = this.GetPoint(item, customer);
               if (p > 0)
               {
                  var point = this.GetCustomerPoint(item, customer, p, (int)tID, CustomerChanal.TIP, "tipsociety-fbshare");
                  point.CustomerID = customer.ID;
                  point.Source = "tipsociety-fbshare";
                  point.PurchaseAmt = 0;
                  this._context.Add(point);
                  totalpoint += p;
               }
            }
            this._context.SaveChanges();
            return Json(new
            {
               responseCode = "1",
               responseDesc = "SUCCESS",
            });
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = new CustomerDTO();
         model.channelInt = CustomerChanal.TIP;
         model.status = "Active";
         ViewBag.ListProvince = this.ListProvince();
         ViewBag.ListProvinceEn = this.ListProvinceEn();
         ViewBag.ListPrefix = this.ListPrefix();
         ViewBag.ListPrefixEn = this.ListPrefixEn();
         return View("CustomerEdit", model);
      }

      [HttpGet]
      public IActionResult Info()
      {
         ViewData["FBAppID"] = this._conf.FBAppID;
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
               var cus = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
               if (cus != null)
                  idParam = cus.ID.ToString();
            }
         }
         CustomerDTO model = null;
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Include(i => i.User).Include(i => i.CustomerClass).Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Login", "Accounts");
            }
            else
            {
               GetCustomerClass(customer, true);
               model = CustomerBinding.Binding(customer);
               if (customer.PrefixTh.HasValue)
               {
                  var prefix = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixTh).FirstOrDefault();
                  if (prefix != null)
                     model.prefix = prefix.Name;
               }
               if (customer.PrefixEn.HasValue)
               {
                  var prefixEn = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixEn).FirstOrDefault();
                  if (prefixEn != null)
                     model.prefixEn = prefixEn.NameEng;
               }
               var province = this._context.Provinces.Where(w => w.ProvinceID == model.provinceId).FirstOrDefault();
               if (province != null)
                  model.provinceName = province.ProvinceName;

               var district = this._context.Aumphurs.Where(w => w.AumphurID == model.districtId).FirstOrDefault();
               if (district != null)
                  model.districtName = district.AumphurName;

               var subdistrict = this._context.Tumbons.Where(w => w.TumbonID == model.subDistrictId).FirstOrDefault();
               if (subdistrict != null)
                  model.subdistrictName = subdistrict.TumbonName;



               ViewData["breadcrumb"] = model.firstName + " " + model.lastName;
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
            return RedirectToAction("Login", "Accounts");
         }
         var randomizer = new Random();
         var privileges = this._context.Privileges
                                 .Include(s => s.Merchant)
                                 .Include(s => s.PrivilegeImages)
                                 .Include(s => s.PrivilegeCustomerClasses)
                                 .Include(s => s.MerchantCategory)
                                 .Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         if (model.customerClassID.HasValue)
            privileges = privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == model.customerClassID));

         ViewBag.ListPrivilege = privileges.OrderByDescending(o => o.Update_On).Take(3);

         return View("CustomerInfo", model);
      }

      [HttpGet]
      public IActionResult Update()
      {
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
               idParam = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault().ID.ToString();
            }
         }
         CustomerDTO model = null;
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Login", "Accounts");
            }
            else
            {
               GetCustomerClass(customer, true);
               model = CustomerBinding.Binding(customer);
               ViewData["breadcrumb"] = model.firstName + " " + model.lastName;
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
            return RedirectToAction("Login", "Accounts");
         }
         ViewBag.ListProvince = this.ListProvince();
         ViewBag.ListProvinceEn = this.ListProvinceEn();
         ViewBag.ListPrefix = this.ListPrefix();
         ViewBag.ListPrefixEn = this.ListPrefixEn();
         return View("CustomerEdit", model);
      }

      [HttpPost]
      public async Task<IActionResult> Modify(CustomerDTO model)
      {
         if (ModelState.IsValid)
         {
            var rg = new RijndaelCrypt();

            if (string.IsNullOrEmpty(model.username))
               model.username = model.email;
            if (this.isExistEmail(model))
               ModelState.AddModelError("email", "อีเมลซ้ำในระบบ");
            if (this.isExistUserName(model))
               ModelState.AddModelError("email", "รหัสผู้ใช้งานซ้ำในระบบ");
            if (model.ID <= 0)
            {
               if (this.isExistIDCard(model))
               {
                  model.ShowIdcardDupPopup = true;
                  var ducus = this._context.Customers.Include(i => i.User).Where(c => c.IDCard == model.citizenId & (model.ID > 0 ? c.ID != model.ID : true));
                  model.dupEmail = new List<string>();
                  model.dupFBID = new List<string>();
                  foreach (var cus in ducus)
                  {
                     if (string.IsNullOrEmpty(cus.FacebookID))
                        model.dupEmail.Add(cus.User.UserName);
                     else
                        model.dupFBID.Add(cus.User.UserName);

                     model.dupIdcard = model.citizenId;
                  }
                  ModelState.AddModelError("citizenId", "หมายเลขบัตรประชาชนซ้ำในระบบ");
               }

               if (this.isExistMobileNo(model))
                  ModelState.AddModelError("moblieNo", "เบอร์โทรศัพท์ซ้ำในระบบ");
               if (this.isExistName(model))
               {
                  ModelState.AddModelError("firstName", "ชื่อนามสกุลซ้ำในระบบ");
                  ModelState.AddModelError("lastName", "ชื่อนามสกุลซ้ำในระบบ");
               }
            }
            else
            {
               if (string.IsNullOrEmpty(model.citizenIdInit))
               {
                  if (this.isExistIDCard(model))
                  {
                     model.ShowIdcardDupPopup = true;
                     var ducus = this._context.Customers.Include(i => i.User).Where(c => c.IDCard == model.citizenId & (model.ID > 0 ? c.ID != model.ID : true));
                     model.dupEmail = new List<string>();
                     model.dupFBID = new List<string>();
                     foreach (var cus in ducus)
                     {
                        if (string.IsNullOrEmpty(cus.FacebookID))
                           model.dupEmail.Add(cus.User.UserName);
                        else
                           model.dupFBID.Add(cus.User.UserName);

                        model.dupIdcard = model.citizenId;
                     }
                     ModelState.AddModelError("citizenId", "หมายเลขบัตรประชาชนซ้ำในระบบ");
                  }
               }
            }
            if (ModelState.IsValid)
            {
               if (model.ID <= 0)
               {
                  /*new customer*/
                  var customer = new Customer();
                  customer.Create_On = DateUtil.Now();
                  customer.ChannelUpdate = CustomerChanal.TIP;
                  customer = CustomerBinding.Binding(customer, model);
                  GetCustomerClass(customer);
                  customer.Create_On = DateUtil.Now();
                  customer.Create_By = this.HttpContext.User.Identity.Name;
                  customer.Update_On = DateUtil.Now();
                  customer.Update_By = this.HttpContext.User.Identity.Name;

                  var regs = this.GetPointCondition(customer, TransacionTypeID.Register);
                  foreach (var item in regs)
                  {
                     var p = this.GetPoint(item, customer);
                     if (p > 0)
                     {
                        var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Register, CustomerChanal.TIP, "tipsociety-register");
                        customer.CustomerPoints.Add(point);
                     }
                  }
                  if (!string.IsNullOrEmpty(customer.FriendCode))
                  {
                     var cons = this.GetPointCondition(customer, TransacionTypeID.InviteFriend);
                     foreach (var item in cons)
                     {
                        var p = this.GetPoint(item, customer);
                        if (p > 0)
                        {
                           var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.InviteFriend, CustomerChanal.TIP, "tipsociety-register");
                           var friend = this._context.Customers.Where(w => w.RefCode == customer.FriendCode).FirstOrDefault();
                           if (friend != null)
                           {
                              point.CustomerID = friend.ID;
                              this._context.CustomerPoints.Add(point);
                           }
                        }
                     }
                  }
                  if (!customer.UpdatedAllRequired)
                  {
                     if (!string.IsNullOrEmpty(customer.NameTh) & !string.IsNullOrEmpty(customer.SurNameTh) & !string.IsNullOrEmpty(customer.MoblieNo) & !string.IsNullOrEmpty(customer.IDCard) & customer.DOB.HasValue & !string.IsNullOrEmpty(customer.CUR_HouseNo) & customer.CUR_Province.HasValue & customer.CUR_Tumbon.HasValue & customer.CUR_Aumper.HasValue & !string.IsNullOrEmpty(customer.CUR_ZipCode))
                     {
                        var cons = this.GetPointCondition(customer, TransacionTypeID.Update);
                        foreach (var item in cons)
                        {
                           var p = this.GetPoint(item, customer);
                           if (p > 0)
                           {
                              var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Update, CustomerChanal.TIP, "tipsociety-register");
                              customer.CustomerPoints.Add(point);
                              customer.UpdatedAllRequired = true;
                           }
                        }
                     }
                  }
                  this._context.Customers.Add(customer);
                  this._context.SaveChanges();
                  this._context.Entry(customer).GetDatabaseValues();
                  customer.RefCode = CustomerBinding.GetRefCode(customer);
                  customer.Success = false;

                  model.customerNo = customer.CustomerNo;
                  model.userID = customer.UserID;
                  model.ID = customer.ID;
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  _logger.LogWarning(DateUtil.Now() + "");
                  _logger.LogWarning("/rewardpoint/customerprofile/register");
                  _logger.LogWarning(JsonConvert.SerializeObject(model));

                  /*create customer imobile*/
                  using (var client = new HttpClient())
                  {
                     client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile/register");
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                     model.username = rg.Encrypt(model.username);
                     model.password = rg.Encrypt(model.password);
                     model.status = customer.Status.toStatusNameEn();
                     if (customer.PrefixTh.HasValue)
                     {
                        var prefix = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixTh).FirstOrDefault();
                        if (prefix != null)
                           model.prefix = prefix.Name;
                     }
                     if (customer.PrefixEn.HasValue)
                     {
                        var prefixEn = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixEn).FirstOrDefault();
                        if (prefixEn != null)
                           model.prefixEn = prefixEn.NameEng;
                     }

                     StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                     HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
                     if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                     {
                        customer.Success = true;
                        this._context.SaveChanges();
                     }
                  }
               }
               else
               {
                  /*update customer*/
                  //var customer = await _cusRepo.GetByID(model.ID);
                  var customer = this._context.Customers.Where(w => w.ID == model.ID).FirstOrDefault();
                  var user = this._context.Users.Where(w => w.ID == model.userID).FirstOrDefault();
                  customer.User = user;
                  customer.ChannelUpdate = CustomerChanal.TIP;
                  customer = CustomerBinding.Binding(customer, model);
                  customer.Success = false;

                  customer.Update_On = DateUtil.Now();
                  customer.Update_By = this.HttpContext.User.Identity.Name;

                  if (!customer.UpdatedAllRequired)
                  {
                     if (!string.IsNullOrEmpty(customer.NameTh) & !string.IsNullOrEmpty(customer.SurNameTh) & !string.IsNullOrEmpty(customer.MoblieNo) & !string.IsNullOrEmpty(customer.IDCard) & customer.DOB.HasValue & !string.IsNullOrEmpty(customer.CUR_HouseNo) & customer.CUR_Province.HasValue & customer.CUR_Tumbon.HasValue & customer.CUR_Aumper.HasValue & !string.IsNullOrEmpty(customer.CUR_ZipCode))
                     {
                        var cons = this.GetPointCondition(customer, TransacionTypeID.Update);
                        foreach (var item in cons)
                        {
                           var p = this.GetPoint(item, customer);
                           if (p > 0)
                           {
                              var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Update, CustomerChanal.TIP, "tipsociety-update");
                              point.CustomerID = customer.ID;
                              this._context.CustomerPoints.Add(point);
                              customer.UpdatedAllRequired = true;

                           }
                        }
                     }
                  }
                  GetCustomerClass(customer);

                  customer.RefCode = CustomerBinding.GetRefCode(customer);
                  this._context.Users.Attach(customer.User);
                  this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
                  this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  _logger.LogWarning(DateUtil.Now() + "");
                  _logger.LogWarning("/rewardpoint/customerprofile");
                  _logger.LogWarning(JsonConvert.SerializeObject(model));
                  /*update customer imobile*/
                  using (var client = new HttpClient())
                  {
                     client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     model.username = rg.Encrypt(model.username);
                     model.password = null;
                     model.status = customer.Status.toStatusNameEn();
                     if (customer.PrefixTh.HasValue)
                     {
                        var prefix = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixTh).FirstOrDefault();
                        if (prefix != null)
                           model.prefix = prefix.Name;
                     }
                     if (customer.PrefixEn.HasValue)
                     {
                        var prefixEn = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixEn).FirstOrDefault();
                        if (prefixEn != null)
                           model.prefixEn = prefixEn.NameEng;
                     }
                     StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                     HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                     if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                     {
                        customer.Success = true;
                        this._context.SaveChanges();
                     }
                  }
               }

               var loginServices = new Dhipaya.Services.LoginServices(this.HttpContext);
               if (loginServices.isInRoles(new string[] { RoleName.Member }))
                  return RedirectToAction("Info");
               else
                  return RedirectToAction("Index");

            }
         }
         ViewBag.ListProvince = this.ListProvince();
         ViewBag.ListProvinceEn = this.ListProvinceEn();
         ViewBag.ListPrefix = this.ListPrefix();
         ViewBag.ListPrefixEn = this.ListPrefixEn();
         return View("CustomerEdit", model);
      }

      [HttpGet]
      public IActionResult ResetPwd()
      {
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
               idParam = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault().ID.ToString();
         }
         ResetPwdDTO model = new ResetPwdDTO();
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Index");
            }
            else
            {
               model.ID = customer.ID;
               model.UserID = customer.UserID;
               ViewData["breadcrumb"] = customer.NameTh + " " + customer.SurNameTh;
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
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
               idParam = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault().ID.ToString();
         }
         ResetPwdDTO model = new ResetPwdDTO();
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Index");
            }
            else
            {
               model.ID = customer.ID;
               model.UserID = customer.UserID;
               ViewData["breadcrumb"] = customer.NameTh + " " + customer.SurNameTh;
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
      public async Task<IActionResult> ResetPwd(ResetPwdDTO model)
      {
         if (ModelState.IsValid)
         {
            try
            {
               var customer = this._context.Customers.Where(w => w.ID == model.ID).FirstOrDefault();
               var user = this._context.Users.Where(w => w.ID == model.UserID).FirstOrDefault();
               customer.User = user;


               if (ModelState.IsValid)
               {
                  if (!string.IsNullOrEmpty(model.password))
                  {
                     customer.FirstLogedIn = true;
                     customer.Syned = true;
                     customer.User.Password = DataEncryptor.Encrypt(model.password);
                     customer.Update_On = DateUtil.Now();
                     customer.BCryptPwd = BCrypt.Net.BCrypt.HashPassword(model.password);
                  }


                  this._context.Users.Attach(customer.User);
                  this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
                  this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  /*update customer imobile*/
                  using (var client = new HttpClient())
                  {
                     client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     var rg = new RijndaelCrypt();
                     model.username = rg.Encrypt(customer.User.UserName);
                     model.password = rg.Encrypt(model.password); ;
                     model.confirmPassword = rg.Encrypt(model.confirmPassword); ;

                     StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                     _logger.LogWarning(JsonConvert.SerializeObject(model));
                     // HTTP POST
                     HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                     if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                     {
                        customer.Success = true;
                        this._context.SaveChanges();
                     }
                     else
                     {
                        _logger.LogWarning(JsonConvert.SerializeObject(model));
                        _logger.LogWarning(await response.Content.ReadAsStringAsync());
                     }
                  }
                  return RedirectToAction("Info", new { ID = model.ID });
               }
            }
            catch
            {

            }
         }
         return View(model);
      }
      [HttpPost]
      public async Task<IActionResult> ResetPwdO(ResetPwdDTO model)
      {
         if (ModelState.IsValid)
         {
            try
            {
               var customer = this._context.Customers.Where(w => w.ID == model.ID).FirstOrDefault();
               var user = this._context.Users.Where(w => w.ID == model.UserID).FirstOrDefault();
               customer.User = user;
               if (model.oldpassword == model.password)
               {
                  ModelState.AddModelError("oldpassword", "รหัสผ่านใหม่เหมือนกับรหัสผ่านเดิม");
                  ModelState.AddModelError("password", "รหัสผ่านใหม่เหมือนกับรหัสผ่านเดิม");
               }
               if (model.oldpassword != DataEncryptor.Decrypt(user.Password))
               {
                  if (!string.IsNullOrEmpty(customer.BCryptPwd))
                  {
                     string paintTextPassword = model.oldpassword;
                     string passworeInDB = customer.BCryptPwd;
                     if (!BCrypt.Net.BCrypt.Verify(paintTextPassword, passworeInDB))
                     {
                        ModelState.AddModelError("oldpassword", "รหัสผ่านเดิมไม่ถูกต้อง");
                     }
                  }
                  else
                  {
                     ModelState.AddModelError("oldpassword", "รหัสผ่านเดิมไม่ถูกต้อง");
                  }
               }
               if (ModelState.IsValid)
               {
                  if (!string.IsNullOrEmpty(model.password))
                  {
                     customer.Syned = true;
                     customer.User.Password = DataEncryptor.Encrypt(model.password);
                     customer.Update_On = DateUtil.Now();
                     customer.BCryptPwd = BCrypt.Net.BCrypt.HashPassword(model.password);
                  }


                  this._context.Users.Attach(customer.User);
                  this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
                  this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  /*update customer imobile*/
                  using (var client = new HttpClient())
                  {
                     client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     var rg = new RijndaelCrypt();
                     model.username = rg.Encrypt(customer.User.UserName);
                     model.password = rg.Encrypt(model.password); ;
                     model.confirmPassword = rg.Encrypt(model.confirmPassword); ;

                     StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                     _logger.LogWarning(JsonConvert.SerializeObject(model));
                     // HTTP POST
                     HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                     if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                     {
                        customer.Success = true;
                        this._context.SaveChanges();
                     }
                     else
                     {
                        _logger.LogWarning(JsonConvert.SerializeObject(model));
                        _logger.LogWarning(await response.Content.ReadAsStringAsync());
                     }
                  }
                  return RedirectToAction("Info", new { ID = model.ID });
               }
            }
            catch
            {

            }
         }
         return View(model);
      }

      private bool canDelete(int Id)
      {
         bool retVal = true;
         //var result = this._context.CustomerBalances.Where(c => c.CustomerID == Id);
         //if (result.Count() > 0)
         //{
         //    retVal = false;
         //}
         //else
         //{
         //    var result2 = this._context.StoreTransactions.Where(s => s.CustomerID == Id);
         //    if (result2.Count() > 0)
         //    {
         //        retVal = false;
         //    }
         //}
         return retVal;
      }

      public async Task<IActionResult> Delete(string search_text, int? customerClassID, int? dup, string orderby)
      {
         string idParam = this.RouteData.Values["id"].ToString();
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Include(i => i.CustomerPoints).Where(b => b.ID == recordId).FirstOrDefault();
            if (customer == null)
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            else
            {
               var user = this._context.Users.Where(b => b.ID == customer.UserID).FirstOrDefault();
               var redeems = this._context.Redeems.Where(w => w.CustomerID == customer.ID);
               var mobile = this._context.MobilePoints.Where(w => w.CustomerID == customer.ID);
               var classchages = this._context.CustomerClassChanges.Where(w => w.CustomerID == customer.ID);
               var adjusts = this._context.PointAdjusts.Where(w => w.CustomerID == customer.ID);

               var rg = new RijndaelCrypt();

               var u = rg.Encrypt(user.UserName);
               var p = rg.Encrypt(DataEncryptor.Decrypt(user.Password));
               var flag = rg.Encrypt(customer.FacebookFlag);

               this._context.CustomerPoints.RemoveRange(customer.CustomerPoints);
               this._context.MobilePoints.RemoveRange(mobile);
               this._context.Redeems.RemoveRange(redeems);
               this._context.CustomerClassChanges.RemoveRange(classchages);
               this._context.PointAdjusts.RemoveRange(adjusts);
               this._context.Customers.Remove(customer);

               if (user != null)
                  this._context.Users.Remove(user);

               this._context.SaveChanges();
               /*delete customer imobile*/
               using (var client = new HttpClient())
               {
                  client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile/delete");
                  client.DefaultRequestHeaders.Accept.Clear();
                  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                  var model = new { u = u, p = p, flag = flag };

                  StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                  HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
                  if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                  {
                     customer.Success = true;
                     this._context.SaveChanges();
                  }
               }

            }
         }
         return RedirectToAction("Index", new { search_text = search_text, customerClassID = customerClassID, dup = dup, orderby = orderby });
      }

      [HttpGet]
      public IActionResult Point()
      {
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
               idParam = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault().ID.ToString();
            }
         }
         CustomerDTO model = null;
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Login", "Accounts");
            }
            else
            {
               model = CustomerBinding.Binding(customer);
               var province = this._context.Provinces.Where(w => w.ProvinceID == model.provinceId).FirstOrDefault();
               if (province != null)
                  model.provinceName = province.ProvinceName;

               var district = this._context.Aumphurs.Where(w => w.AumphurID == model.districtId).FirstOrDefault();
               if (district != null)
                  model.districtName = district.AumphurName;

               var subdistrict = this._context.Tumbons.Where(w => w.TumbonID == model.subDistrictId).FirstOrDefault();
               if (subdistrict != null)
                  model.subdistrictName = subdistrict.TumbonName;

               ViewData["breadcrumb"] = model.firstName + " " + model.lastName;
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
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
         var redeems = this._context.Redeems
                                         .Include(i => i.Privilege)
                                         .Include(i => i.Privilege.PrivilegeImages)
                                         .Include(i => i.Privilege.Merchant)
                                         .Include(i => i.Privilege.MerchantCategory)
                                         .Where(w => w.CustomerID == model.ID);

         var points = this._context.CustomerPoints.Where(w => w.CustomerID == model.ID);

         var transtions = new List<CustomerPointHistoryDTO>();
         transtions.AddRange(redeems.Select(s => new CustomerPointHistoryDTO() { Redeem = s, TransactionDate = s.RedeemDate.Value, Point = s.Point }));
         transtions.AddRange(points.Select(s => new CustomerPointHistoryDTO() { CustomerPoint = s, TransactionDate = s.Create_On.Value, Point = s.Point }));

         var list = transtions.OrderByDescending(o => o.TransactionDate);
         ViewBag.TotalPoint = points.Sum(s => s.Point) - redeems.Sum(s => s.Point);

         ViewBag.ItemCount = list.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 25);
         if (ViewBag.ItemCount % 25 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;

         ViewBag.ListPoints = list.Skip(skipRows).Take(25);

         return View("CustomerPoint", model);
      }

      [HttpGet]
      public IActionResult History()
      {
         string idParam = "";
         if (this.RouteData.Values["id"] != null)
         {
            idParam = this.RouteData.Values["id"].ToString();
            if (!_loginServices.isInAdminRoles(this.GetRoles()))
            {
               var userlogin = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (userlogin == null || userlogin.ID.ToString() != idParam)
               {
                  return RedirectToAction("Login", "Accounts");
               }
            }
         }
         else
         {
            var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
               idParam = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault().ID.ToString();
            }
         }
         CustomerDTO model = null;
         int recordId = -1;
         if (!string.IsNullOrEmpty(idParam))
         {
            recordId = Int32.Parse(idParam);
            var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.ID == recordId).FirstOrDefault();
            if (customer == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
               return RedirectToAction("Login", "Accounts");
            }
            else
            {
               model = CustomerBinding.Binding(customer);
            }
         }
         else
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสมาชิก");
            return RedirectToAction("Login", "Accounts");
         }


         int pageno = 1;
         if (this.RouteData.Values["pno"] != null)
         {
            pageno = NumUtil.ParseInteger(this.RouteData.Values["pno"].ToString());
            if (pageno == 0)
               pageno = 1;
         }
         int skipRows = (pageno - 1) * 6;
         var redeems = this._context.Redeems
                                         .Include(i => i.Privilege)
                                         .Include(i => i.Privilege.PrivilegeImages)
                                         .Include(i => i.Privilege.Merchant)
                                         .Include(i => i.Privilege.MerchantCategory)
                                         .Where(w => w.CustomerID == model.ID).OrderByDescending(o => o.RedeemDate);


         ViewBag.ItemCount = redeems.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 6);
         if (ViewBag.ItemCount % 6 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         ViewBag.ListRedeems = redeems.Skip(skipRows).Take(6);


         return View("CustomerHistory", model);
      }

      public IActionResult GetPolicyActive(string id, string name, string surname)
      {
         try
         {
            return Json(new { result = GetRequestPolicyActive(_iia, id, name, surname) });
         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
      }
   }


}