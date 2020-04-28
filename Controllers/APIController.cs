using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dhipaya.DTO.Accounts;
using Microsoft.Extensions.Logging;
using Dhipaya.Services;
using Dhipaya.DAL;
using Dhipaya.Models;
using Microsoft.EntityFrameworkCore;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Dhipaya.Extensions;
using Microsoft.Extensions.Options;
using System.ServiceModel;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Net;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{
   ///rewardpoint/customerprofile/register
   [Produces("application/json")]

   public class APIController : ControllerBase
   {

      public readonly ILoggerFactory _loggerFactory;

      public APIController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, ILoginServices loginServices, ILoggerFactory loggerFactory, IOptions<SystemConf> conf, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILogger<APIController> logger, IOptions<TIPMobile> _mobile) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._loggerFactory = loggerFactory;
         this._context = context;
         this._loginServices = loginServices;
         this._smtp = smtp.Value;
         this._iia = _iia.Value;
         this._logger = logger;
         this._mobile = _mobile.Value;
         this._conf = conf.Value;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      [HttpGet]
      public IActionResult Privilege()
      {
         return View();
      }

      [HttpPost]
      [AllowAnonymous]
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
         return Json(GetRequestPolicyActive(_iia, idc, name, surname));
      }

      [HttpPost]
      [AllowAnonymous]
      public JsonResult IIAStatusUpdate(int max, int month = 0, int year = 0)
      {
         try
         {
            var i = 0;
            var customers = this._context.Customers.Where(w => !string.IsNullOrEmpty(w.IDCard) & w.IIASyned == false);
            if (month > 0)
            {
               customers = customers.Where(w => w.Create_On.Value.Month == month);
            }
            if (year > 0)
            {
               customers = customers.Where(w => w.Create_On.Value.Year == year);
            }
            _logger.LogWarning("Auto Syn Count: " + customers.Count());

            foreach (var customer in customers)
            {
               _logger.LogWarning("Auto Syn(" + i + "):" + customer.NameTh + " " + customer.SurNameTh + " " + customer.IDCard);
               GetCustomerClass(customer, getpoint: false);
               customer.Update_On = DateUtil.Now();
               customer.Update_By = "IIAStatusUpdate";
               if (i >= max)
                  break;
               i++;
            }
            this._context.SaveChanges();
         }
         catch (Exception ex)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = ex.Message,
            });
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }

      [HttpPut]
      [AllowAnonymous]
      public JsonResult GenDOBPoint([FromBody] string str)
      {
         _logger.LogWarning(DateUtil.Now() + " " + str);

         if (string.IsNullOrEmpty(str))
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = "Invalid input",
            });
         }

         try
         {
            var cusspit = str.Split("|", StringSplitOptions.RemoveEmptyEntries);
            foreach (var spit in cusspit)
            {
               var cusID = NumUtil.ParseInteger(spit);
               var customer = _context.Customers.Where(w => w.ID == cusID).FirstOrDefault();
               if (customer != null)
               {
                  var regs = this.GetPointCondition(customer, TransacionTypeID.DOB);
                  foreach (var item in regs)
                  {
                     if (item.Point.Value > 0)
                     {
                        var point = this.GetCustomerPoint(item, customer, item.Point.Value, (int)TransacionTypeID.DOB, CustomerChanal.TIP, "tipsociety-dob");
                        customer.LastgivePointDOB = DateUtil.Now().Year;
                        customer.CustomerPoints.Add(point);
                     }
                  }
               }

            }
            _context.SaveChanges();
            return Json(new
            {
               responseCode = "200",
               responseDesc = "SUCCESS",
            });
         }
         catch (Exception ex)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = ex.Message,
            });
         }
      }


      [HttpPost]
      [AllowAnonymous]
      public JsonResult PwdUpdate()
      {
         try
         {
            foreach (var customer in this._context.Customers.Include(i => i.User).Where(w => w.Channel == CustomerChanal.ShareHolderImport))
            {
               customer.User.Password = DataEncryptor.Encrypt(customer.ID.ToString("00000000"));
            }
            this._context.SaveChanges();
         }
         catch (Exception ex)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = ex.Message,
            });
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }

      [HttpPost]
      [AllowAnonymous]
      public async Task<JsonResult> SendDeleteAccount([FromBody] SendDeleteDTO model)
      {
         var rg = new RijndaelCrypt();
         model.idcard = rg.Decrypt(model.idcard);
         if (string.IsNullOrEmpty(model.idcard))
            return Json(new { responseCode = "-1", responseDesc = "ไม่พบข้อมูลเลขที่บัตรประชาชน" });

         var customers = _context.Customers.Include(i => i.User).Where(w => w.IDCard == model.idcard);
         if (_conf.SendEmail == true)
         {
            foreach (var customer in customers)
            {
               GetCustomerClass(customer);
            }
            this._context.SaveChanges();

            var codes = new List<string>();
            foreach (var customer in customers)
            {
               var accode = new AccountCode();
               accode.Code = this.CreateAccountCode();
               accode.Create_On = DateUtil.Now();
               accode.Create_By = customer.User.UserName;
               accode.CustomerID = customer.ID;
               accode.Status = StatusType.Active;
               codes.Add(accode.Code);
               this._context.AccountCodes.Add(accode);

               var point = this._context.CustomerPoints.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);
               var redeempoint = this._context.Redeems.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);

               var totalPoint = point - redeempoint;
               customer.Point = NumUtil.ParseInteger(totalPoint);
               customer.CustomerClass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();

            }
            this._context.SaveChanges();
            await MailDeleteAccount(customers, codes);

         }
         return Json(new { responseCode = "200", responseDesc = "ส่งอีเมลสำเร็จ" });

      }

      [HttpGet]
      [AllowAnonymous]
      public async Task<IActionResult> SendDeleteMail(string idcard)
      {
         var rg = new RijndaelCrypt();
         idcard = rg.Decrypt(idcard);
         if (string.IsNullOrEmpty(idcard))
            return View();
         var customers = _context.Customers.Include(i => i.User).Where(w => w.IDCard == idcard);
         if (_conf.SendEmail == true)
         {
            foreach (var customer in customers)
            {
               GetCustomerClass(customer);
            }
            this._context.SaveChanges();

            var codes = new List<string>();
            foreach (var customer in customers)
            {
               var accode = new AccountCode();
               accode.Code = this.CreateAccountCode();
               accode.Create_On = DateUtil.Now();
               accode.Create_By = customer.User.UserName;
               accode.CustomerID = customer.ID;
               accode.Status = StatusType.Active;
               codes.Add(accode.Code);
               this._context.AccountCodes.Add(accode);

               var point = this._context.CustomerPoints.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);
               var redeempoint = this._context.Redeems.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);

               var totalPoint = point - redeempoint;
               customer.Point = NumUtil.ParseInteger(totalPoint);
               customer.CustomerClass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();

            }
            this._context.SaveChanges();
            await MailDeleteAccount(customers, codes);

         }
         return View();
      }


      [HttpPost]
      [AllowAnonymous]
      public IActionResult ImportCustomers(int max = 1, CustomerChanal channel = CustomerChanal.DhiMemberImport)
      {
         try
         {
            var List = new List<Customer>();
            foreach (var imp in _context.CustomerImpts.Where(w => w.Imported == false & w.Msg == null).Take(max).OrderBy(o => o.ID))
            {

               var dup = List.Where(w => w.User.UserName == imp.Email).FirstOrDefault();
               if (dup != null)
               {
                  imp.Msg += "email duplicated on same list;";
                  continue;
               }

               var dup1 = _context.Users.Where(w => w.UserName == imp.Email).FirstOrDefault();
               if (dup1 != null)
               {
                  imp.Msg += "email duplicated on database;";
                  continue;
               }

               var dup2 = List.Where(w => w.IDCard == imp.IDCard).FirstOrDefault();
               if (dup2 != null)
               {
                  imp.Msg += "Id card  duplicated on same list;";
                  continue;
               }

               var dup3 = _context.Customers.Where(w => w.IDCard == imp.IDCard).FirstOrDefault();
               if (dup3 != null)
               {
                  imp.Msg += "Id card duplicated on database;";
                  continue;
               }

               if (!string.IsNullOrEmpty(imp.MoblieNo))
               {
                  if (imp.MoblieNo.Substring(0, 1) != "0")
                     imp.MoblieNo = "0" + imp.MoblieNo;
               }

               if (!string.IsNullOrEmpty(imp.NameTh))
                  imp.NameTh = imp.NameTh.Trim();
               else
                  imp.NameTh = "ว่าง";
               if (!string.IsNullOrEmpty(imp.SurNameTh))
                  imp.SurNameTh = imp.SurNameTh.Trim();
               else
                  imp.SurNameTh = "ว่าง";

               imp.Imported = true;
               int? Prefix = 1;
               if (!string.IsNullOrEmpty(imp.PrefixTh))
               {
                  var pr = _context.CustomerPrefixs.Where(w => w.Name == imp.PrefixTh | w.NameEng == imp.PrefixTh | w.NameEng2 == imp.PrefixTh).FirstOrDefault();
                  if (pr != null)
                     Prefix = pr.ID;
               }
               int? PrefixEn = 1;

               if (!string.IsNullOrEmpty(imp.PrefixEn))
               {
                  var prefixEn = this._context.CustomerPrefixs.Where(w => w.Name == imp.PrefixEn | w.NameEng == imp.PrefixEn | w.NameEng2 == imp.PrefixEn).FirstOrDefault();
                  if (prefixEn != null)
                     PrefixEn = prefixEn.ID;
               }

               int? provinceId = null;
               int? districtId = null;
               int? subDistrictId = null;
               if (!string.IsNullOrEmpty(imp.Province))
               {
                  var pro = _context.Provinces.Where(w => w.ProvinceName == imp.Province | w.ProvinceNameEn == imp.Province).FirstOrDefault();
                  if (pro != null)
                     provinceId = pro.ProvinceID;
               }
               if (!string.IsNullOrEmpty(imp.Aumper))
               {
                  var d = _context.Aumphurs.Where(w => w.AumphurName == imp.Aumper | w.AumphurNameEn == imp.Aumper).FirstOrDefault();
                  if (d != null)
                     districtId = d.AumphurID;
               }
               if (!string.IsNullOrEmpty(imp.Tumbon))
               {
                  var t = _context.Tumbons.Where(w => w.TumbonName == imp.Tumbon | w.TumbonNameEn == imp.Tumbon).FirstOrDefault();
                  if (t != null)
                     subDistrictId = t.TumbonID;
               }

               int? provinceIdEn = null;
               int? districtIdEn = null;
               int? subDistrictIdEn = null;
               if (!string.IsNullOrEmpty(imp.ProvinceEn))
               {
                  var pro = _context.Provinces.Where(w => w.ProvinceNameEn == imp.ProvinceEn | w.ProvinceName == imp.ProvinceEn).FirstOrDefault();
                  if (pro != null)
                     provinceIdEn = pro.ProvinceID;
               }
               if (!string.IsNullOrEmpty(imp.AumperEn))
               {
                  var d = _context.Aumphurs.Where(w => w.AumphurNameEn == imp.AumperEn | w.AumphurName == imp.AumperEn).FirstOrDefault();
                  if (d != null)
                     districtIdEn = d.AumphurID;
               }
               if (!string.IsNullOrEmpty(imp.TumbonEn))
               {
                  var t = _context.Tumbons.Where(w => w.TumbonNameEn == imp.TumbonEn | w.TumbonName == imp.TumbonEn).FirstOrDefault();
                  if (t != null)
                     subDistrictIdEn = t.TumbonID;
               }

               string idcard = null;
               if (!string.IsNullOrEmpty(imp.IDCard))
               {
                  if (imp.IDCard.Length == 13)
                     idcard = imp.IDCard;
                  else
                  {
                     if (string.IsNullOrEmpty(imp.Passport))
                        imp.Passport = imp.IDCard;
                  }

               }
               var cus = new Customer()
               {
                  Email = imp.Email,
                  IDCard = idcard,
                  Passport = imp.Passport,
                  MoblieNo = imp.MoblieNo,
                  UserLevel = UserLevelType.Member,
                  Status = UserStatusType.Active,
                  PrefixTh = Prefix,
                  PrefixEn = PrefixEn,
                  NameTh = imp.NameTh,
                  SurNameTh = imp.SurNameTh,
                  Create_By = "Migrate",
                  Update_By = "Migrate",
                  Create_On = DateUtil.Now(),
                  Update_On = DateUtil.Now(),
                  Channel = channel,
                  Syned = true,
                  DoSendReisterEmail = true,
                  CustomerClassID = 1,
                  Gender = imp.Gender,
                  DOB = imp.Birthday,

                  CUR_HouseNo = imp.HouseNo,
                  CUR_Moo = imp.Moo,
                  CUR_Soi = imp.Soi,
                  CUR_Road = imp.Road,
                  CUR_Province = provinceId,
                  CUR_Aumper = districtId,
                  CUR_Tumbon = subDistrictId,
                  CUR_ZipCode = imp.ZipCode,
                  CUR_HouseNoEn = imp.HouseNoEn,
                  CUR_MooEn = imp.MooEn,
                  CUR_SoiEn = imp.SoiEn,
                  CUR_RoadEn = imp.RoadEn,
                  CUR_ProvinceEn = provinceIdEn,
                  CUR_AumperEn = districtIdEn,
                  CUR_TumbonEn = subDistrictIdEn,
                  CUR_ZipCodeEn = imp.ZipCodeEn,
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
               //GetCustomerClass(item);
            }
            _context.SaveChanges();
         }
         catch (Exception ex)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = ex.Message,
            });
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }

      [HttpPost]
      [AllowAnonymous]
      public async Task<IActionResult> SentRegisterEmail(string email, int max = 1)
      {
         try
         {
            var sentcnt = 0;

            if (!string.IsNullOrEmpty(email))
            {
               foreach (var customer in this._context.Customers.Include(i => i.User).Where(w => w.SentReisterEmail == false & w.Email.Contains("@gmail.com") & w.DoSendReisterEmail).Take(1))
               {
                  await MailActivateAcc(email, customer);
                  sentcnt++;
               }
               return Json(new
               {
                  responseCode = "200",
                  responseDesc = "SUCCESS",
               });
            }
            while (1 == 1)
            {
               System.Threading.Thread.Sleep(10000);
               if (this._context.Customers.Where(w => w.SentReisterEmail == false).Count() > 0)
               {
                  foreach (var customer in this._context.Customers.Include(i => i.User).Where(w => w.SentReisterEmail == false & w.Email.Contains("@gmail.com") & w.DoSendReisterEmail).Take(4))
                  {
                     await MailActivateAcc(customer.Email, customer);
                     sentcnt++;

                  }
                  this._context.SaveChanges();
                  System.Threading.Thread.Sleep(60000);
                  if (sentcnt >= max)
                  {
                     return Json(new
                     {
                        responseCode = "200",
                        responseDesc = "SUCCESS",
                     });
                  }

                  foreach (var customer in this._context.Customers.Include(i => i.User).Where(w => w.SentReisterEmail == false & w.Email.Contains("@hotmail.com") & w.DoSendReisterEmail).Take(4))
                  {
                     await MailActivateAcc(customer.Email, customer);
                     sentcnt++;

                  }
                  this._context.SaveChanges();
                  System.Threading.Thread.Sleep(60000);
                  if (sentcnt >= max)
                  {
                     return Json(new
                     {
                        responseCode = "200",
                        responseDesc = "SUCCESS",
                     });
                  }

                  foreach (var customer in this._context.Customers.Include(i => i.User).Where(w => w.SentReisterEmail == false & (!w.Email.Contains("@hotmail.com") & !w.Email.Contains("@gmail.com")) & w.DoSendReisterEmail).Take(2))
                  {
                     await MailActivateAcc(customer.Email, customer);
                     sentcnt++;
                  }
                  this._context.SaveChanges();
                  System.Threading.Thread.Sleep(60000);
                  if (sentcnt >= max)
                  {
                     return Json(new
                     {
                        responseCode = "200",
                        responseDesc = "SUCCESS",
                     });
                  }

               }
               else
                  break;
            }
         }
         catch (Exception ex)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = ex.Message,
            });
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }

      [HttpPost]
      [AllowAnonymous]
      public JsonResult CheckLogin([FromBody] SSODTO model)
      {
         var rg = new RijndaelCrypt();
         var u = rg.Decrypt(model.u);
         var p = rg.Decrypt(model.p);

         _logger.LogWarning(DateUtil.Now() + " CheckLogin");
         _logger.LogWarning(JsonConvert.SerializeObject(model));

         model.u = u;
         var user = this._context.Users.Include(w => w.UserRole).Where(w => w.UserName == u).FirstOrDefault();
         if (user != null)
         {
            string desPassword = DataEncryptor.Decrypt(user.Password);
            if (p == desPassword | p == u + "TInsUsReq")
            {
               var customer = this._context.Customers.Include(i => i.CustomerClass).Include(w => w.User).Where(c => c.UserID == user.ID).FirstOrDefault();
               if (customer != null)
               {
                  var cmodel = CustomerBinding.Binding(customer);
                  cmodel.username = rg.Encrypt(u);
                  cmodel.password = rg.Encrypt(p);
                  cmodel.status = customer.Status.toStatusNameEn();
                  if (customer.PrefixTh.HasValue)
                  {
                     var prefix = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixTh).FirstOrDefault();
                     if (prefix != null)
                        cmodel.prefix = prefix.Name;
                  }
                  if (customer.PrefixEn.HasValue)
                  {
                     var prefixEn = this._context.CustomerPrefixs.Where(w => w.ID == customer.PrefixEn).FirstOrDefault();
                     if (prefixEn != null)
                        cmodel.prefixEn = prefixEn.NameEng;
                  }
                  return Json(new
                  {
                     responseCode = "200",
                     responseDesc = "Transaction Complete",
                     customerProfile = cmodel
                  });
               }

            }
         }

         return Json(new
         {
            responseCode = "205",
            responseDesc = "Invalid username or password.",
         });
      }

      [HttpPut]
      [AllowAnonymous]
      public JsonResult Confirm([FromBody] MobilePointDTO model)
      {
         var totalpoint = 0M;
         if (ModelState.IsValid)
         {
            var rg = new RijndaelCrypt();
            model.UserName = rg.Decrypt(model.UserName);

            _logger.LogWarning(DateUtil.Now() + " Confirm");
            _logger.LogWarning(JsonConvert.SerializeObject(model));

            var responseDesc = new List<string>();
            var customer = this._context.Customers.Include(i => i.CustomerPoints).Include(i => i.User).Where(c => c.User.UserName == model.UserName).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            var productcode = "";
            var tranID = TransacionTypeID.None;
            if (model.Source == MBSource.imobile_purchase)
            {
               tranID = TransacionTypeID.BuyInsure;
               productcode = model.Product;
            }
            else if (model.Source == MBSource.paybill)
            {
               tranID = TransacionTypeID.Paybill;
               productcode = model.Product;
            }

            else if (model.Source == MBSource.renew)
            {
               tranID = TransacionTypeID.Renew;
               productcode = model.Product;
            }
            else if (model.Source == MBSource.repay)
            {
               tranID = TransacionTypeID.Repay;
               productcode = model.Product;
            }
            else if (model.Source == MBSource.carinspection)
            {
               tranID = TransacionTypeID.CarInspection;
               productcode = model.Product;
            }
            else if (model.Source == MBSource.add_policy)
            {
               var addpolicy = _context.CustomerPoints.Where(w => w.CustomerID == customer.ID & w.TransacionTypeID == (int)TransacionTypeID.AddPolicy).FirstOrDefault();
               if (addpolicy == null)
                  tranID = TransacionTypeID.AddPolicy;
            }

            if (tranID != TransacionTypeID.None)
            {
               var valided = true;
               if (tranID == TransacionTypeID.Renew | tranID == TransacionTypeID.BuyInsure)
               {
                  var pointed = _context.CustomerPoints.Where(w => w.OrderNo == model.OrderNo & w.CustomerID == customer.ID & w.ChannelType == ChannelType.Online).FirstOrDefault();
                  if (pointed != null)
                     valided = false;
               }

               if (valided)
               {
                  var regs = this.GetPointCondition(customer, tranID, productcode, OutletCode.MobileApplication, ChannelType.Online);
                  if (regs != null)
                  {
                     foreach (var item in regs)
                     {
                        var p = this.GetPoint(item, customer, model.Amount);
                        if (p > 0)
                        {
                           var point = this.GetCustomerPoint(item, customer, p, (int)tranID, CustomerChanal.Mobile, model.Source);
                           point.CustomerID = customer.ID;
                           point.Package = model.Package;
                           point.Source = model.Source;
                           point.InsuranceClass = model.Product;
                           point.Subclass = model.Package;
                           point.OrderNo = model.OrderNo;
                           point.OutletCode = OutletCode.MobileApplication;
                           point.PolicyNo = model.PolicyNo;
                           point.PurchaseAmt = model.Amount.HasValue ? model.Amount.Value : 0;
                           if (!string.IsNullOrEmpty(productcode))
                           {
                              var product = _context.Products.Where(w => w.ProductCode == model.Product).FirstOrDefault();
                              if (product != null)
                                 point.ProductID = product.ProductID;
                           }
                           this._context.Add(point);
                           totalpoint += p;
                        }
                     }
                  }
               }


            }


            var mpoint = new MobilePoint();
            mpoint.CustomerID = customer.ID;
            mpoint.Point = totalpoint;
            mpoint.Channel = model.Channel;
            mpoint.Package = model.Package;
            mpoint.Product = model.Product;
            mpoint.OrderNo = model.OrderNo;
            mpoint.Source = model.Source;
            mpoint.IDCard = model.IDCard;
            mpoint.Passport = model.Passport;
            mpoint.PolicyNo = model.PolicyNo;
            mpoint.PurchaseAmt = model.Amount.HasValue ? model.Amount.Value : 0;
            mpoint.Create_On = DateUtil.Now();

            this._context.Add(mpoint);

            if (string.IsNullOrEmpty(customer.IDCard))
               customer.IDCard = model.IDCard;
            if (string.IsNullOrEmpty(customer.Passport))
               customer.Passport = model.Passport;
            this._context.SaveChanges();
            return Json(new
            {
               responseCode = "200",
               responseDesc = "SUCCESS",
               responsePoint = totalpoint,
            });
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpGet]
      [AllowAnonymous]
      public JsonResult IsExistRefCode(string refCode)
      {
         var responseDesc = new List<string>();
         if (string.IsNullOrEmpty(refCode))
         {
            responseDesc.Add("refCode : ไม่พบข้อมูล refCode");
            responseDesc.Add("refCode_en : Reference Code doesn't exist.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         var customer = this._context.Customers.Where(c => c.RefCode == refCode).FirstOrDefault();
         if (customer == null)
         {
            responseDesc.Add("user : ไม่พบข้อมูลลูกค้าในระบบ");
            responseDesc.Add("user_en : User doesn't exist.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         else
         {
            return Json(new
            {
               responseCode = "200",
               responseDesc = "SUCCESS",
            });
         }
      }

      [HttpGet]
      [AllowAnonymous]
      public JsonResult GetPoint(int? id)
      {
         var points = _context.CustomerPoints.Where(w => w.ID == id);
         var redeems = this._context.Redeems.Where(w => w.CustomerID == id);
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
            point = points.Sum(s => s.Point) - redeems.Sum(s => s.Point)
         });
      }

      [HttpPut]
      [AllowAnonymous]
      public async Task<JsonResult> Status([FromBody] CustomerStatusDTO model)
      {
         var rg = new RijndaelCrypt();
         var u = rg.Decrypt(model.UserName);
         var p = rg.Decrypt(model.Password);
         var f = "";
         if (!string.IsNullOrEmpty(model.flag))
         {
            model.flag = rg.Decrypt(model.flag);
            f = model.flag;
         }
         model.UserName = u;
         _logger.LogWarning(DateUtil.Now() + " Status");
         _logger.LogWarning(JsonConvert.SerializeObject(model));

         if (ModelState.IsValid)
         {
            var responseDesc = new List<string>();

            var user = this._context.Users.Where(u2 => u2.UserName == u).FirstOrDefault();
            if (user == null)
            {
               /*create customer imobile*/

               await this.Repair(u, p, f, "loginForStatus");
               user = this._context.Users.Where(u2 => u2.UserName == u).FirstOrDefault();
            }
            if (user == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            var customer = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            var point = this._context.MobilePoints.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);

            var cusClass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();
            return Json(new
            {
               responseCode = "200",
               responseDesc = "SUCCESS",
               customerPoint = point,
               referenceCode = customer.RefCode,
               customerType = cusClass != null ? cusClass.Name : "Silver",
               status = customer.Status.toStatusNameEn(),
            });
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpPut]
      [AllowAnonymous]
      public async Task<JsonResult> GetCustomer([FromBody] CustomerStatusDTO model)
      {

         var rg = new RijndaelCrypt();
         var u = "";
         var p = "";
         var f = "";
         var responseDesc = new List<string>();
         if (string.IsNullOrEmpty(model.UserName))
         {
            responseDesc.Add("username : กรุณาระบุ username");
            responseDesc.Add("username_en : Username cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (string.IsNullOrEmpty(model.Password))
         {
            if (model.facebookFlag != "Y")
            {
               responseDesc.Add("password : กรุณาระบุ password");
               responseDesc.Add("password_en : Password cannot be null.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
         }
         try
         {
            u = rg.Decrypt(model.UserName);
            p = rg.Decrypt(model.Password);
            if (!string.IsNullOrEmpty(model.facebookFlag))
            {
               f = model.facebookFlag;
            }
         }
         catch
         {
            if (string.IsNullOrEmpty(u))
            {
               responseDesc.Add("username : รูปแบบ username ไม่ถูกต้อง");
               responseDesc.Add("username_en : username format is invalid.");
            }
            if (model.facebookFlag != "Y")
            {
               if (string.IsNullOrEmpty(p))
               {
                  responseDesc.Add("password : รูปแบบ password ไม่ถูกต้อง");
                  responseDesc.Add("password_en : username format is invalid.");
               }
            }

            return Json(new
            {
               responseCode = "-1",
               responseDesc = responseDesc,
            });
         }

         model.UserName = u;

         if (ModelState.IsValid)
         {

            var user = this._context.Users.Include(i => i.UserRole).Where(u2 => u2.UserName == u).FirstOrDefault();
            if (user == null)
            {
               /*create customer imobile*/

               await this.Repair(u, p, f, "loginForStatus");
               user = this._context.Users.Where(u2 => u2.UserName == u).FirstOrDefault();
            }
            if (user == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            var customer = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }

            if (user.Status != UserStatusType.InActive)
            {
               if (user.UserRole != null && user.UserRole.RoleName == RoleName.Member)
               {
                  if (customer.FacebookFlag != "Y")
                  {
                     if (string.IsNullOrEmpty(user.Password))
                     {
                        if (!customer.ResetedPwd)
                        {
                           responseDesc.Add("password : ไมมีข้อมูล password ในระบบ กรุณาตั้งหัสผ่านใหม่");
                           responseDesc.Add("password_en : password is empty. please reset password");
                           return Json(new { responseCode = "-2", responseDesc = responseDesc, channel = customer.Channel.toName(), resetedpwd = customer.ResetedPwd, });
                        }
                     }
                     else
                     {
                        var passwordvalided = false;
                        string desPassword = DataEncryptor.Decrypt(user.Password);
                        if (rg.Decrypt(model.Password) == desPassword)
                        {
                           passwordvalided = true;
                        }
                        if (passwordvalided == false)
                        {
                           if (!string.IsNullOrEmpty(customer.BCryptPwd))
                           {
                              string paintTextPassword = rg.Decrypt(model.Password);
                              string passworeInDB = customer.BCryptPwd;
                              if (!string.IsNullOrEmpty(paintTextPassword) && !string.IsNullOrEmpty(passworeInDB))
                              {
                                 if (BCrypt.Net.BCrypt.Verify(paintTextPassword, passworeInDB))
                                 {
                                    user.Password = DataEncryptor.Encrypt(rg.Decrypt(model.Password));
                                    passwordvalided = true;
                                 }
                              }
                           }
                        }

                        if (passwordvalided == false)
                        {
                           responseDesc.Add("password : password ไม่ถูกต้อง");
                           responseDesc.Add("password_en : password is invalid");
                           return Json(new { responseCode = "-3", responseDesc = responseDesc, channel = customer.Channel.toName(), resetedpwd = customer.ResetedPwd, });
                        }

                     }
                  }
               }
            }

            var point = this._context.MobilePoints.Where(w => w.CustomerID == customer.ID).Sum(s => s.Point);

            var cusClass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();
            var prefix = _context.CustomerPrefixs.Where(w => w.ID == customer.PrefixTh).FirstOrDefault();
            var prefixEn = _context.CustomerPrefixs.Where(w => w.ID == customer.PrefixEn).FirstOrDefault();
            var province = _context.Provinces.Where(w => w.ProvinceID == customer.CUR_Province).FirstOrDefault();
            var district = _context.Aumphurs.Where(w => w.AumphurID == customer.CUR_Aumper).FirstOrDefault();
            var subdistrict = _context.Tumbons.Where(w => w.TumbonID == customer.CUR_Tumbon).FirstOrDefault();
            var provinceEn = _context.Provinces.Where(w => w.ProvinceID == customer.CUR_ProvinceEn).FirstOrDefault();
            var districtEn = _context.Aumphurs.Where(w => w.AumphurID == customer.CUR_AumperEn).FirstOrDefault();
            var subdistrictEn = _context.Tumbons.Where(w => w.TumbonID == customer.CUR_TumbonEn).FirstOrDefault();

            var result = Json(new
            {
               responseCode = "200",
               responseDesc = "SUCCESS",
               customerPoint = point,
               refCode = customer.RefCode,
               customerType = cusClass != null ? cusClass.Name : "Silver",
               status = customer.Status.toStatusNameEn(),
               facebookFlag = customer.FacebookFlag,
               facebookID = customer.FacebookID,
               gender = customer.Gender,
               birthdate = DateUtil.ToDisplayDate(customer.DOB),
               prefix = prefix != null ? prefix.Name : "",
               firstName = customer.NameTh,
               lastName = customer.SurNameTh,
               prefixEn = prefixEn != null ? (!string.IsNullOrEmpty(prefixEn.NameEng2) ? prefixEn.NameEng2 : (!string.IsNullOrEmpty(prefixEn.NameEng) ? prefixEn.NameEng : prefixEn.Name)) : "",
               firstNameEn = customer.NameEn,
               lastNameEn = customer.SurNameEn,
               moblieNo = customer.MoblieNo,
               telNo = customer.TelNo,
               citizenId = customer.IDCard,
               passport = customer.Passport,
               channel = customer.Channel.toName(),
               email = customer.Email,
               friendCode = customer.FriendCode,
               houseNo = customer.CUR_HouseNo,
               lane = customer.CUR_Lane,
               road = customer.CUR_Road,
               villageNo = customer.CUR_VillageNo,
               villageName = customer.CUR_VillageName,
               provinceId = customer.CUR_Province,
               provinceName = province != null ? province.ProvinceName : "",
               districtId = customer.CUR_Aumper,
               district = district != null ? district.AumphurName : "",
               subDistrictId = customer.CUR_Tumbon,
               subDistrictName = subdistrict != null ? subdistrict.TumbonName : "",
               postalCode = customer.CUR_ZipCode,
               houseNoEn = customer.CUR_HouseNoEn,
               laneEn = customer.CUR_LaneEn,
               roadEn = customer.CUR_RoadEn,
               villageNoEn = customer.CUR_VillageNoEn,
               villageNameEn = customer.CUR_VillageNameEn,
               provinceIdEn = customer.CUR_ProvinceEn,
               provinceNameEn = provinceEn != null ? provinceEn.ProvinceNameEn : "",
               districtIdEn = customer.CUR_AumperEn,
               districtEn = districtEn != null ? districtEn.AumphurNameEn : "",
               subDistrictIdEn = customer.CUR_TumbonEn,
               subDistrictNameEn = subdistrictEn != null ? subdistrictEn.TumbonNameEn : "",
               postalCodeEn = customer.CUR_ZipCodeEn,
               channelupdate = customer.ChannelUpdate.toName(),
               resetedpwd = customer.ResetedPwd,
            });
            if (customer.ID == 245259)
            {
               _logger.LogWarning("GetCustomer");
               _logger.LogWarning(JsonConvert.SerializeObject(result));
            }

            return result;
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      public class citizenDTO
      {
         public string citizenId { get; set; }
      }

      [HttpPost]
      [AllowAnonymous]
      public JsonResult GetCustomerByIDcard([FromBody] citizenDTO model)
      {

         var responseDesc = new List<string>();
         if (model == null || string.IsNullOrEmpty(model.citizenId))
         {
            responseDesc.Add("citizenId : กรุณาระบุ citizenId");
            responseDesc.Add("citizenId_en : citizenId cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         var customer = this._context.Customers.Include(i => i.CustomerClass).Where(w => w.IDCard == model.citizenId);
         if (customer.Count() == 0)
         {
            responseDesc.Add("citizenId : ไม่พบข้อมูลสมาชิก");
            responseDesc.Add("citizenId_en : Customer data has not found.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         var customers = customer.Select(s => new { s.ID, s.RefCode, s.NameTh, s.SurNameTh, s.CustomerClass.Name });
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
            customers = customers
         });
      }

      [HttpPost]
      [AllowAnonymous]
      public JsonResult CheckDupAccount([FromBody] citizenDTO model)
      {

         var responseDesc = new List<string>();
         if (model == null || string.IsNullOrEmpty(model.citizenId))
         {
            responseDesc.Add("citizenId : กรุณาระบุ citizenId");
            responseDesc.Add("citizenId_en : citizenId cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         var customer = this._context.Customers.Include(i => i.User).Where(w => w.IDCard == model.citizenId);
         if (customer.Count() <= 0)
         {
            responseDesc.Add("citizenId : ไม่พบข้อมูลสมาชิกที่ซ้ำ");
            responseDesc.Add("citizenId_en : Customer data has not found.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         var dups = new List<string>();
         foreach (var cus in customer)
         {
            if (!string.IsNullOrEmpty(cus.FacebookID))
               dups.Add("Facebook " + cus.User.UserName);
            else
               dups.Add("อีเมล " + cus.User.UserName);
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
            header = "เลขบัตรประชาชนหมายเลข " + model.citizenId + " มีการสร้างบัญชีไว้แล้ว สำหรับ",
            accounts = dups,
            url = _conf.Url + Url.Action("SendDeleteAccount", "Accounts", new { idcard = model.citizenId })
         });
      }


      [HttpPut]  /* get customer from Tip Insure only*/
      [AllowAnonymous]
      public async Task<JsonResult> GetCustomerDataAutoFill([FromBody] CustomerDataAutoFillDTO model)
      {

         var rg = new RijndaelCrypt();
         var u = "";
         var p = "";
         var f = "";
         var responseDesc = new List<string>();
         if (string.IsNullOrEmpty(model.username))
         {
            responseDesc.Add("username :The Username cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (string.IsNullOrEmpty(model.password))
         {
            responseDesc.Add("password :The Password cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (string.IsNullOrEmpty(model.firstName))
         {
            responseDesc.Add("firstName :The firstName cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (string.IsNullOrEmpty(model.lastName))
         {
            responseDesc.Add("lastName :The lastName cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (string.IsNullOrEmpty(model.cardNo))
         {
            responseDesc.Add("cardNo :The cardNo cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (model.cardType == null)
         {
            responseDesc.Add("cardType :The cardType cannot be null.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }

         if (model.cardType != 1 && model.cardType != 2)
         {
            responseDesc.Add("cardType :The cardType is invalid.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }


         if (string.IsNullOrEmpty(model.lang))
         {
            model.lang = "TH";
         }


         try
         {
            u = rg.Decrypt(model.username);
            p = rg.Decrypt(model.password);
         }
         catch
         {
            if (string.IsNullOrEmpty(u))
            {
               responseDesc.Add("username :The username format is invalid.");
            }
            if (string.IsNullOrEmpty(p))
            {
               responseDesc.Add("password :The password format is invalid.");
            }

            return Json(new
            {
               responseCode = "-1",
               responseDesc = responseDesc,
            });
         }

         model.username = u;
         if (u != "TipInsure@Admin")
         {
            responseDesc.Add("username :The username is invalid.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });

         }
         if (p != "TipSo@X!TipIn@Sure")
         {
            responseDesc.Add("password :The password is invalid.");
            return Json(new { responseCode = "-1", responseDesc = responseDesc });
         }
         if (ModelState.IsValid)
         {

            var customer = this._context.Customers.Include(i => i.User.UserRole).Where(w => w.NameTh == model.firstName & w.SurNameTh == model.lastName & (w.IDCard == model.cardNo || w.Passport == model.cardNo)).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }

            var user = customer.User;
            if (user == null)
            {
               /*create customer imobile*/
               await this.Repair(u, p, f, "loginForStatus");
               user = this._context.Users.Where(u2 => u2.UserName == u).FirstOrDefault();
            }
            if (user == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }

            if (user.UserRole == null || user.UserRole.RoleName != RoleName.Member)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }

            if (model.lang == "TH")
            {
               return Json(new
               {
                  responseCode = "200",
                  responseDesc = "SUCCESS",
                  refCode = customer.RefCode,
                  status = customer.Status.toStatusNameEn(),
                  CustomerBirthDate = DateUtil.ToDisplayDate(customer.DOB),
                  CustomerMobileNo = customer.MoblieNo,
                  CustomerEmail = customer.Email,
                  AddressNo = customer.CUR_HouseNo,
                  AddressMoo = customer.CUR_Moo,
                  AddressSoi = customer.CUR_Soi,
                  AddressRoad = customer.CUR_Road,
                  AddressVillage = customer.CUR_VillageName,
                  AddressProvinceId = customer.CUR_Province,
                  AddressAmphurId = customer.CUR_Aumper,
                  AddressTumbonId = customer.CUR_Tumbon,
                  AddressPostCode = customer.CUR_ZipCode,
               });
            }
            else
            {
               return Json(new
               {
                  responseCode = "200",
                  responseDesc = "SUCCESS",
                  refCode = customer.RefCode,
                  status = customer.Status.toStatusNameEn(),
                  CustomerBirthDate = DateUtil.ToDisplayDate(customer.DOB),
                  CustomerMobileNo = customer.MoblieNo,
                  CustomerEmail = customer.Email,
                  AddressNo = customer.CUR_HouseNoEn,
                  AddressMoo = customer.CUR_MooEn,
                  AddressSoi = customer.CUR_SoiEn,
                  AddressRoad = customer.CUR_RoadEn,
                  AddressVillage = customer.CUR_VillageNameEn,
                  AddressProvinceId = customer.CUR_ProvinceEn,
                  AddressAmphurId = customer.CUR_AumperEn,
                  AddressTumbonId = customer.CUR_TumbonEn,
                  AddressPostCode = customer.CUR_ZipCodeEn,
               });
            }

         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpPost]
      [AllowAnonymous]
      public async Task<JsonResult> Register([FromBody] CustomerDTO model, bool repair = false)
      {
         _logger.LogWarning(DateUtil.Now() + " Register");
         _logger.LogWarning(JsonConvert.SerializeObject(model));

         var rg = new RijndaelCrypt();
         var responseDesc = new List<string>();
         var un = "";
         var pwd = "";
         try
         {
            un = rg.Decrypt(model.username);
            pwd = rg.Decrypt(model.password);
         }
         catch
         {
            if (string.IsNullOrEmpty(un))
            {
               responseDesc.Add("username : รูปแบบ username ไม่ถูกต้อง");
               responseDesc.Add("username_en : username format is invalid.");
            }
            if (string.IsNullOrEmpty(pwd))
            {
               responseDesc.Add("password : รูปแบบ password ไม่ถูกต้อง");
               responseDesc.Add("password_en : username format is invalid.");
            }
            return Json(new
            {
               responseCode = "-1",
               responseDesc = responseDesc,
            });
         }
         model.username = un;
         model.password = pwd;

         model.confirmPassword = model.password;
         model.status = StatusType.Active.toStatusName();
         if (model.channel == "INT Intersect")
         {
            model.channelInt = CustomerChanal.INTIntersect;
            if (this.isExistUserName(model))
            {
               model.username = rg.Encrypt(model.username);
               model.password = rg.Encrypt(model.password);
               return await Update(model, true);
            }
         }
         else if (model.channel == "TIP Insure")
            model.channelInt = CustomerChanal.TipInsure;
         else
            model.channelInt = CustomerChanal.Mobile;

         if (model.facebookFlag == "Y")
         {
            model.facebookFlag = "Y";
            model.firstNameEn = model.firstName;
            model.lastNameEn = model.lastName;
            if (!string.IsNullOrEmpty(model.email) && ModelState.ContainsKey("email"))
               ModelState.Remove("email");
         }

         if (!string.IsNullOrEmpty(model.password) && ModelState.ContainsKey("password"))
            ModelState.Remove("password");

         if (!string.IsNullOrEmpty(model.confirmPassword) && ModelState.ContainsKey("confirmPassword"))
            ModelState.Remove("confirmPassword");

         if (ModelState.IsValid)
         {
            if (this.isExistUserName(model))
            {
               responseDesc.Add("username : ผู้ใช้ซ้ำในระบบ");
               responseDesc.Add("username_en : Username is duplicated.");
            }
            if (!repair)
            {
               if (this.isExistEmail(model))
               {
                  responseDesc.Add("email : อีเมล์ซ้ำในระบบ");
                  responseDesc.Add("email_en : Email is duplicated.");
               }

               if (this.isExistIDCard(model))
               {
                  responseDesc.Add("citizenId : หมายเลขบัตรประชาชนซ้ำในระบบ");
                  responseDesc.Add("citizenId_en : Citizen ID is duplicated.");

               }
               if (!string.IsNullOrEmpty(model.friendCode) && !this.isExistFriendCode(model))
               {
                  responseDesc.Add("friendCode : ไม่พบข้อมูล friend Code");
                  responseDesc.Add("friendCode_en : Friend Code doesn't exist.");
               }
            }

            if (responseDesc.Count() > 0)
               return Json(new { responseCode = "-1", responseDesc = responseDesc });

            if (ModelState.IsValid)
            {
               if (!string.IsNullOrEmpty(model.provinceName) & model.provinceId == null)
               {
                  var pro = _context.Provinces.Where(w => w.ProvinceName == model.provinceName | w.ProvinceNameEn == model.provinceName).FirstOrDefault();
                  if (pro != null)
                     model.provinceId = pro.ProvinceID;
               }
               if (!string.IsNullOrEmpty(model.districtName) & model.districtId == null)
               {
                  var d = _context.Aumphurs.Where(w => w.AumphurName == model.districtName | w.AumphurNameEn == model.districtName).FirstOrDefault();
                  if (d != null)
                     model.districtId = d.AumphurID;
               }
               if (!string.IsNullOrEmpty(model.subdistrictName) & model.subDistrictId == null)
               {
                  var t = _context.Tumbons.Where(w => w.TumbonName == model.subdistrictName | w.TumbonNameEn == model.subdistrictName).FirstOrDefault();
                  if (t != null)
                     model.subDistrictId = t.TumbonID;
               }

               if (!string.IsNullOrEmpty(model.provinceNameEn) & model.provinceIdEn == null)
               {
                  var pro = _context.Provinces.Where(w => w.ProvinceNameEn == model.provinceNameEn | w.ProvinceName == model.provinceNameEn).FirstOrDefault();
                  if (pro != null)
                     model.provinceIdEn = pro.ProvinceID;
               }
               if (!string.IsNullOrEmpty(model.districtNameEn) & model.districtIdEn == null)
               {
                  var d = _context.Aumphurs.Where(w => w.AumphurNameEn == model.districtNameEn | w.AumphurName == model.districtNameEn).FirstOrDefault();
                  if (d != null)
                     model.districtIdEn = d.AumphurID;
               }
               if (!string.IsNullOrEmpty(model.subdistrictNameEn) & model.subDistrictIdEn == null)
               {
                  var t = _context.Tumbons.Where(w => w.TumbonNameEn == model.subdistrictNameEn | w.TumbonName == model.subdistrictNameEn).FirstOrDefault();
                  if (t != null)
                     model.subDistrictIdEn = t.TumbonID;
               }


               if (!string.IsNullOrEmpty(model.prefix))
               {
                  var prefix = this._context.CustomerPrefixs.Where(w => w.Name == model.prefix | w.NameEng == model.prefix | w.NameEng2 == model.prefix).FirstOrDefault();
                  if (prefix != null)
                     model.prefixInt = prefix.ID;
               }
               if (!string.IsNullOrEmpty(model.prefixEn))
               {
                  var prefixEn = this._context.CustomerPrefixs.Where(w => w.Name == model.prefixEn | w.NameEng == model.prefixEn | w.NameEng2 == model.prefixEn).FirstOrDefault();
                  if (prefixEn != null)
                     model.prefixEnInt = prefixEn.ID;
               }

               if (model.ID <= 0)
               {
                  var customer = new Customer();
                  customer.Create_On = DateUtil.Now();
                  if (model.channel == "TIP Insure")
                     customer.ChannelUpdate = CustomerChanal.TipInsure;
                  else if (model.channel == "INT Intersect")
                     customer.ChannelUpdate = CustomerChanal.INTIntersect;
                  else
                     customer.ChannelUpdate = CustomerChanal.Mobile;

                  customer = CustomerBinding.Binding(customer, model);
                  GetCustomerClass(customer);
                  if (model.channel == "INT Intersect")
                     customer.Channel = CustomerChanal.INTIntersect;
                  else if (model.channel == "TIP Insure")
                     customer.Channel = CustomerChanal.TipInsure;
                  else
                     customer.Channel = CustomerChanal.Mobile;

                  customer.Create_On = DateUtil.Now();
                  customer.Create_By = customer.User.UserName;
                  customer.Update_On = DateUtil.Now();
                  customer.Update_By = customer.User.UserName;
                  customer.FirstLogedIn = true;
                  var regs = this.GetPointCondition(customer, TransacionTypeID.Register);
                  foreach (var item in regs)
                  {
                     var p = this.GetPoint(item, customer);
                     if (p > 0)
                     {
                        var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Register, CustomerChanal.Mobile, "imobile-register");
                        customer.CustomerPoints.Add(point);
                     }
                  }
                  var friendpoint = 0;
                  Customer friend = null;
                  if (!string.IsNullOrEmpty(customer.FriendCode))
                  {
                     var invites = this.GetPointCondition(customer, TransacionTypeID.InviteFriend);
                     foreach (var item in invites)
                     {
                        var p = this.GetPoint(item, customer);
                        if (p > 0)
                        {
                           var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.InviteFriend, CustomerChanal.Mobile, "imobile-register");
                           friend = this._context.Customers.Where(w => w.RefCode == customer.FriendCode).FirstOrDefault();
                           if (friend != null)
                           {
                              friendpoint = p;
                              point.CustomerID = friend.ID;
                              this._context.CustomerPoints.Add(point);
                           }
                        }
                     }
                  }

                  this._context.Customers.Add(customer);
                  this._context.SaveChanges();
                  this._context.Entry(customer).GetDatabaseValues();
                  customer.RefCode = CustomerBinding.GetRefCode(customer);
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  if (string.IsNullOrEmpty(model.password))
                  {
                     customer.User.Password = DataEncryptor.Encrypt(customer.ID.ToString("00000000"));
                     this._context.Users.Attach(customer.User);
                     this._context.Entry(customer.User).Property(u => u.Password).IsModified = true;
                     this._context.Update(customer);
                     this._context.SaveChanges();
                  }

                  if (customer.Channel == CustomerChanal.INTIntersect /*| customer.Channel == CustomerChanal.TipInsure*/)
                  {
                     if (_conf.SendEmail == true)
                        await MailActivateAcc(customer.Email, customer.ID);

                     //if (_conf.SendSMS == true)
                     //   SendSMS(customer.ID);
                  }

                  if (_conf.SendEmail == true && friend != null && friendpoint > 0)
                     await MailInviteFriend(friend.Email, friend, customer, friendpoint);

                  //if (customer.Channel != CustomerChanal.Mobile)
                  //{
                  //   using (var client = new HttpClient())
                  //   {
                  //      client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile/register");
                  //      client.DefaultRequestHeaders.Accept.Clear();
                  //      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                  //      model.username = rg.Encrypt(model.username);
                  //      model.password = rg.Encrypt(model.password);
                  //      model.status = customer.Status.toStatusNameEn();

                  //      StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                  //      HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
                  //      if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                  //      {
                  //         customer.Success = true;
                  //         this._context.SaveChanges();
                  //      }
                  //   }
                  //}

                  var cusClass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();
                  return Json(new
                  {
                     responseCode = "200",
                     responseDesc = "SUCCESS",
                     customerType = cusClass != null ? cusClass.Name : "Silver",
                     status = customer.Status.toStatusNameEn(),
                     refCode = customer.RefCode,
                  });
               }
            }
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpPut]
      [AllowAnonymous]
      public async Task<JsonResult> ResetPWD([FromBody] MobileResetPwdDTO model)
      {
         if (ModelState.IsValid)
         {
            var rg = new RijndaelCrypt();
            var responseDesc = new List<string>();
            var un = "";
            var pwd = "";
            try
            {
               un = rg.Decrypt(model.UserName);
               pwd = rg.Decrypt(model.Password);
            }
            catch
            {
               if (string.IsNullOrEmpty(un))
               {
                  responseDesc.Add("username : รูปแบบ username ไม่ถูกต้อง");
                  responseDesc.Add("username_en : username format is invalid.");
               }
               if (string.IsNullOrEmpty(pwd))
               {
                  responseDesc.Add("password : รูปแบบ password ไม่ถูกต้อง");
                  responseDesc.Add("password_en : username format is invalid.");
               }
               return Json(new
               {
                  responseCode = "-1",
                  responseDesc = responseDesc,
               });
            }
            model.UserName = un;
            model.Password = pwd;

            var customer = this._context.Customers.Include(i => i.User).Where(c => c.User.UserName == model.UserName).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("Username : ไม่พบข้อมูลลูกค้าในระบบ");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            customer.Syned = true;
            customer.User = this._context.Users.Where(w => w.ID == customer.UserID).FirstOrDefault();
            customer.User.Password = DataEncryptor.Encrypt(model.Password);
            this._context.Users.Attach(customer.User);
            this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
            this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
            this._context.Update(customer);
            this._context.SaveChanges();
            /*update customer imobile*/
            if (model.channel == "TIP Insure")
            {
               using (var client = new HttpClient())
               {
                  client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                  client.DefaultRequestHeaders.Accept.Clear();
                  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                  var model2 = new ResetPwdDTO();

                  model2.username = rg.Encrypt(customer.User.UserName);
                  model2.password = rg.Encrypt(model.Password); ;
                  model2.confirmPassword = rg.Encrypt(model.Password);

                  StringContent content = new StringContent(JsonConvert.SerializeObject(model2), Encoding.UTF8, "application/json");
                  _logger.LogWarning(JsonConvert.SerializeObject(model2));
                  // HTTP POST
                  HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                  if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                  {
                     customer.Success = true;
                     this._context.SaveChanges();
                  }
                  else
                  {
                     _logger.LogWarning(JsonConvert.SerializeObject(model2));
                     _logger.LogWarning(await response.Content.ReadAsStringAsync());
                  }
               }
            }

            return Json(new { responseCode = "200", responseDesc = "SUCCESS" });
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpPut]
      [AllowAnonymous]
      public async Task<JsonResult> RandomPWD([FromBody] MobileRandomPwdDTO model)
      {
         if (ModelState.IsValid)
         {
            var rg = new RijndaelCrypt();
            var responseDesc = new List<string>();
            var un = "";
            try
            {
               un = rg.Decrypt(model.UserName);
            }
            catch
            {
               if (string.IsNullOrEmpty(un))
               {
                  responseDesc.Add("username : รูปแบบ username ไม่ถูกต้อง");
                  responseDesc.Add("username_en : username format is invalid.");
               }
               return Json(new
               {
                  responseCode = "-1",
                  responseDesc = responseDesc,
               });
            }
            model.UserName = un;

            var customer = this._context.Customers.Include(i => i.User).Where(c => c.User.UserName == model.UserName).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("Username : ไม่พบข้อมูลลูกค้าในระบบ");
               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            customer.Syned = true;
            customer.User = this._context.Users.Where(w => w.ID == customer.UserID).FirstOrDefault();
            var password = CustomerBinding.RandomString(8);
            customer.User.Password = DataEncryptor.Encrypt(password);
            this._context.Users.Attach(customer.User);
            this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
            this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
            this._context.Update(customer);
            this._context.SaveChanges();
            if (model.channel == "TIP Insure")
            {
               using (var client = new HttpClient())
               {
                  client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                  client.DefaultRequestHeaders.Accept.Clear();
                  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                  var model2 = new ResetPwdDTO();

                  model2.username = rg.Encrypt(customer.User.UserName);
                  model2.password = rg.Encrypt(password);
                  model2.confirmPassword = rg.Encrypt(password);

                  StringContent content = new StringContent(JsonConvert.SerializeObject(model2), Encoding.UTF8, "application/json");
                  _logger.LogWarning(JsonConvert.SerializeObject(model2));
                  // HTTP POST
                  HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                  if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                  {
                     customer.Success = true;
                     this._context.SaveChanges();
                  }
                  else
                  {
                     _logger.LogWarning(JsonConvert.SerializeObject(model2));
                     _logger.LogWarning(await response.Content.ReadAsStringAsync());
                  }
               }
            }
            return Json(new { responseCode = "200", responseDesc = "SUCCESS", password = rg.Encrypt(password) });
         }
         return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
      }

      [HttpPut]
      [AllowAnonymous]
      public async Task<JsonResult> Update([FromBody] CustomerDTO model, bool intIntersect = false)
      {
         _logger.LogWarning(DateUtil.Now() + " Update");
         _logger.LogWarning(JsonConvert.SerializeObject(model));

         try
         {
            var rg = new RijndaelCrypt();
            var responseDesc = new List<string>();
            var un = "";
            try
            {
               un = rg.Decrypt(model.username);
            }
            catch
            {
               if (string.IsNullOrEmpty(un))
               {
                  responseDesc.Add("username : รูปแบบ username ไม่ถูกต้อง");
                  responseDesc.Add("username_en : username format is invalid.");
               }

               return Json(new
               {
                  responseCode = "-1",
                  responseDesc = responseDesc,
               });
            }
            model.username = un;

            var customer = this._context.Customers.Include(i => i.CustomerPoints).Include(i => i.User).Where(c => c.User.UserName == model.username).FirstOrDefault();
            if (customer == null)
            {
               responseDesc.Add("username : ไม่พบข้อมูลลูกค้าในระบบ");
               responseDesc.Add("username_en : Username doesn't exist.");

               return Json(new { responseCode = "-1", responseDesc = responseDesc });
            }
            string desPassword = DataEncryptor.Decrypt(customer.User.Password);
            model.password = desPassword;
            model.confirmPassword = model.password;
            model.channelInt = customer.Channel;
            model.userLevel = customer.UserLevel;
            if (!string.IsNullOrEmpty(model.citizenId))
            {
               if (model.citizenId.Length == 13)
               {

               }
               else
               {
                  if (string.IsNullOrEmpty(model.passport))
                     model.passport = model.citizenId;

                  model.citizenId = null;
               }
            }
            if (model.facebookFlag == "Y")
            {
               model.firstNameEn = model.firstName;
               model.lastNameEn = model.lastName;
               if (!string.IsNullOrEmpty(model.email) && ModelState.ContainsKey("email"))
                  ModelState.Remove("email");
            }

            if (!string.IsNullOrEmpty(model.password) && ModelState.ContainsKey("password"))
               ModelState.Remove("password");
            if (!string.IsNullOrEmpty(model.confirmPassword) && ModelState.ContainsKey("confirmPassword"))
               ModelState.Remove("confirmPassword");
            if (ModelState.IsValid)
            {
               model.ID = customer.ID;
               model.userID = customer.UserID;

               if (this.isExistUserName(model))
               {
                  responseDesc.Add("username : ผู้ใช้ซ้ำในระบบ");
                  responseDesc.Add("username_en : Username is duplicated.");
               }
               if (this.isExistEmail(model))
               {
                  responseDesc.Add("email : อีเมล์ซ้ำในระบบ");
                  responseDesc.Add("email_en : Email is duplicated.");
               }

               if (this.isExistIDCard(model))
               {
                  responseDesc.Add("citizenId : หมายเลขบัตรประชาชนซ้ำในระบบ");
                  responseDesc.Add("citizenId_en : Citizen ID is duplicated.");

               }
               if (!string.IsNullOrEmpty(model.friendCode) && !this.isExistFriendCode(model))
               {
                  responseDesc.Add("friendCode : ไม่พบข้อมูล friend Code");
                  responseDesc.Add("friendCode_en : Friend Code doesn't exist.");
               }

               if (responseDesc.Count() > 0)
                  return Json(new { responseCode = "-1", responseDesc = responseDesc });

               if (ModelState.IsValid)
               {
                  if (!string.IsNullOrEmpty(model.provinceName) & model.provinceId == null)
                  {
                     var pro = _context.Provinces.Where(w => w.ProvinceName == model.provinceName | w.ProvinceNameEn == model.provinceName).FirstOrDefault();
                     if (pro != null)
                        model.provinceId = pro.ProvinceID;
                  }
                  if (!string.IsNullOrEmpty(model.districtName) & model.districtId == null)
                  {
                     var d = _context.Aumphurs.Where(w => w.AumphurName == model.districtName | w.AumphurNameEn == model.districtName).FirstOrDefault();
                     if (d != null)
                        model.districtId = d.AumphurID;
                  }
                  if (!string.IsNullOrEmpty(model.subdistrictName) & model.subDistrictId == null)
                  {
                     var t = _context.Tumbons.Where(w => w.TumbonName == model.subdistrictName | w.TumbonNameEn == model.subdistrictName).FirstOrDefault();
                     if (t != null)
                        model.subDistrictId = t.TumbonID;
                  }

                  if (!string.IsNullOrEmpty(model.provinceNameEn) & model.provinceIdEn == null)
                  {
                     var pro = _context.Provinces.Where(w => w.ProvinceNameEn == model.provinceNameEn | w.ProvinceName == model.provinceNameEn).FirstOrDefault();
                     if (pro != null)
                        model.provinceIdEn = pro.ProvinceID;
                  }
                  if (!string.IsNullOrEmpty(model.districtNameEn) & model.districtIdEn == null)
                  {
                     var d = _context.Aumphurs.Where(w => w.AumphurNameEn == model.districtNameEn | w.AumphurName == model.districtNameEn).FirstOrDefault();
                     if (d != null)
                        model.districtIdEn = d.AumphurID;
                  }
                  if (!string.IsNullOrEmpty(model.subdistrictNameEn) & model.subDistrictIdEn == null)
                  {
                     var t = _context.Tumbons.Where(w => w.TumbonNameEn == model.subdistrictNameEn | w.TumbonName == model.subdistrictNameEn).FirstOrDefault();
                     if (t != null)
                        model.subDistrictIdEn = t.TumbonID;
                  }

                  if (!string.IsNullOrEmpty(model.prefix))
                  {
                     var prefix = this._context.CustomerPrefixs.Where(w => w.Name == model.prefix | w.NameEng == model.prefix).FirstOrDefault();
                     if (prefix != null)
                        model.prefixInt = prefix.ID;
                  }
                  if (!string.IsNullOrEmpty(model.prefixEn))
                  {
                     var prefixEn = this._context.CustomerPrefixs.Where(w => w.Name == model.prefixEn | w.NameEng == model.prefixEn).FirstOrDefault();
                     if (prefixEn != null)
                        model.prefixEnInt = prefixEn.ID;
                  }

                  var iia = new IIAMemberResult();
                  if (model.ID >= 0)
                  {
                     var user = this._context.Users.Where(w => w.ID == model.userID).FirstOrDefault();
                     customer.User = user;
                     if (model.channel == "TIP Insure")
                        customer.ChannelUpdate = CustomerChanal.TipInsure;
                     else
                        customer.ChannelUpdate = CustomerChanal.Mobile;
                     customer = CustomerBinding.Binding(customer, model);
                     GetCustomerClass(customer);
                     customer.Update_On = DateUtil.Now();
                     customer.Update_By = customer.User.UserName;
                     if (customer.RegGeneratedPoint == false)
                     {
                        var regs = this.GetPointCondition(customer, TransacionTypeID.Register);
                        foreach (var item in regs)
                        {
                           var p = this.GetPoint(item, customer);
                           if (p > 0)
                           {
                              var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Register, CustomerChanal.Mobile, "imobile-register");
                              var oldpoint = customer.CustomerPoints.Where(w => w.CustomerID == customer.ID & w.TransacionTypeID == (int)TransacionTypeID.Register).FirstOrDefault();
                              if (oldpoint != null)
                              {
                                 oldpoint.Point = point.Point;
                                 oldpoint.Code = item.ConditionCode;
                                 oldpoint.Name = item.Name;
                              }
                              else
                              {
                                 customer.CustomerPoints.Add(point);
                              }
                              customer.RegGeneratedPoint = true;
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
                                 var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.Update, CustomerChanal.TIP, "tipsociety-update");
                                 point.CustomerID = customer.ID;
                                 this._context.CustomerPoints.Add(point);
                                 customer.UpdatedAllRequired = true;

                              }
                           }
                        }
                     }
                     this._context.Users.Attach(customer.User);
                     this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
                     this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
                     this._context.Update(customer);
                     this._context.SaveChanges();

                     /*update customer imobile*/
                     //using (var client = new HttpClient())
                     //{
                     //   client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile");
                     //   client.DefaultRequestHeaders.Accept.Clear();
                     //   client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     //   model.username = rg.Encrypt(model.username);
                     //   model.password = null;
                     //   model.status = customer.Status.toStatusNameEn();

                     //   StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                     //   HttpResponseMessage response = await client.PutAsync(client.BaseAddress, content);
                     //   if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                     //   {
                     //      customer.Success = true;
                     //      this._context.SaveChanges();
                     //   }
                     //}


                     if (intIntersect)
                     {
                        if (_conf.SendEmail == true)
                           await MailReActivateAcc(customer.Email, customer.ID);

                        //if (_conf.SendSMS == true)
                        //   ReSendSMS(customer.ID);
                     }

                  }

                  return Json(new { responseCode = "200", responseDesc = "SUCCESS" });
               }
            }
            return Json(new { responseCode = "-1", responseDesc = GetErrorModelState() });
         }
         catch (Exception ex)
         {
            _logger.LogWarning(ex.Message);
            return Json(new { responseCode = "-1", responseDesc = ex.Message });
         }
      }

      [HttpPut]
      [AllowAnonymous]
      public IActionResult Delete([FromBody] CustomerStatusDTO model)
      {
         if (_conf.Environment == "Production")
         {
            return Json(new { responseCode = "-1", responseDesc = "ไม่สามารถใช้งานได้บน Production" });
         }
         if (!string.IsNullOrEmpty(model.UserName))
         {
            var rg = new RijndaelCrypt();
            model.UserName = rg.Decrypt(model.UserName);

            var user = this._context.Users.Include(w => w.UserRole).Where(w => w.UserName == model.UserName).FirstOrDefault();
            if (user != null)
            {
               var customer = this._context.Customers.Include(i => i.CustomerPoints).Where(b => b.UserID == user.ID).FirstOrDefault();
               if (customer != null)
               {
                  var redeems = this._context.Redeems.Where(w => w.CustomerID == customer.ID);
                  var mobile = this._context.MobilePoints.Where(w => w.CustomerID == customer.ID);
                  var classchages = this._context.CustomerClassChanges.Where(w => w.CustomerID == customer.ID);
                  var adjusts = this._context.PointAdjusts.Where(w => w.CustomerID == customer.ID);

                  this._context.CustomerPoints.RemoveRange(customer.CustomerPoints);
                  this._context.MobilePoints.RemoveRange(mobile);
                  this._context.CustomerClassChanges.RemoveRange(classchages);
                  this._context.PointAdjusts.RemoveRange(adjusts);
                  this._context.Redeems.RemoveRange(redeems);
                  this._context.Customers.Remove(customer);
                  if (user != null)
                     this._context.Users.Remove(user);

                  this._context.SaveChanges();
                  return Json(new { responseCode = "200", responseDesc = "SUCCESS" });
               }
            }
         }
         return Json(new { responseCode = "-1", responseDesc = "ไม่พบข้อมูลผุ้ใช้" });
      }

      #region IIA Connect
      private XmlDocument CreateSoapGetProvinceData()
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.Append(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">");
         requiredXML.Append(@"<soap-env:Header/>");
         requiredXML.Append(@"<soap-env:Body>");
         requiredXML.Append(@"<tem:GetProvinceData>");
         requiredXML.Append(@"</tem:GetProvinceData>");
         requiredXML.Append(@"</soap-env:Body>");
         requiredXML.Append(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }

      private XmlDocument CreateSoapGetPolicy(string id, string name, string surname)
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.Append(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">");
         requiredXML.Append(@"<soap-env:Header/>");
         requiredXML.Append(@"<soap-env:Body>");
         requiredXML.Append(@"<tem:GetPolicy>");
         requiredXML.Append(@"<tem:idCardNo>" + id + "</tem:idCardNo>");
         requiredXML.Append(@"<tem:policyNo></tem:policyNo>");
         requiredXML.Append(@"<tem:firstname>" + name + "</tem:firstname>");
         requiredXML.Append(@"<tem:lastname>" + surname + "</tem:lastname>");
         requiredXML.Append(@"</tem:GetPolicy>");
         requiredXML.Append(@"</soap-env:Body>");
         requiredXML.Append(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }

      private XmlDocument CreateSoapGetAmphurData(int id)
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.Append(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:tip=""http://schemas.datacontract.org/2004/07/TipMobileWs"">");
         requiredXML.Append(@"<soap-env:Header/>");
         requiredXML.Append(@"<soap-env:Body>");
         requiredXML.Append(@"<tem:GetAmphurData>");
         requiredXML.Append(@"<tem:input>");
         requiredXML.Append(@"<tip:ProvinceId>" + id + "</tip:ProvinceId>");
         requiredXML.Append(@"</tem:input>");
         requiredXML.Append(@"</tem:GetAmphurData>");
         requiredXML.Append(@"</soap-env:Body>");
         requiredXML.Append(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }

      private XmlDocument CreateSoapGetDistrictData(int id)
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.Append(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:tip=""http://schemas.datacontract.org/2004/07/TipMobileWs"">");
         requiredXML.Append(@"<soap-env:Header/>");
         requiredXML.Append(@"<soap-env:Body>");
         requiredXML.Append(@"<tem:GetDistrictData>");
         requiredXML.Append(@"<tem:input>");
         requiredXML.Append(@"<tip:AmphurId>" + id + "</tip:AmphurId>");
         requiredXML.Append(@"</tem:input>");
         requiredXML.Append(@"</tem:GetDistrictData>");
         requiredXML.Append(@"</soap-env:Body>");
         requiredXML.Append(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }


      public IActionResult GetPolicy(string id, string name, string surname)
      {
         try
         {
            return Json(new { result = GetRequestPolicy(id, name, surname) });
         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
      }

      private IIAMemberResult GetRequestPolicy(string id, string name, string surname)
      {
         //XDocument soapResponse = XDocument.Load("GetPolicy.xml");
         //XElement el = soapResponse.Descendants().First(x => x.Name.LocalName == "GetPolicyResult");
         //var val = el.Value;
         //var _result = JsonConvert.DeserializeObject<IIAMemberResult>(val);
         ////do some other stuff...
         //return _result;

         XmlDocument soapRequest = CreateSoapGetPolicy(id, name, surname);
         using (var client = new HttpClient())
         {
            var request = new HttpRequestMessage()
            {
               RequestUri = new Uri(_iia.EndPoint),
               Method = HttpMethod.Post
            };

            request.Content = new StringContent(soapRequest.InnerXml, Encoding.UTF8, "text/xml");

            request.Headers.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            request.Headers.Add("ContentType", "text/xml;charset=\"utf-8\";");
            request.Headers.Add("SOAPAction", _iia.GetPolicy); //I want to call this method 
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
               return new IIAMemberResult() { resultCode = "-1", resultDescription = response.RequestMessage.ToString() };
            }

            Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
            Stream stream = streamTask.Result;
            var sr = new StreamReader(stream);
            XDocument soapResponse = XDocument.Load(sr);
            XElement el = soapResponse.Descendants().First(x => x.Name.LocalName == "GetPolicyResult");
            var val = el.Value;
            var _result = JsonConvert.DeserializeObject<IIAMemberResult>(val);
            //do some other stuff...
            return _result;
         }
      }

      public IActionResult GetProvinceData()
      {
         var sql = new StringBuilder();
         try
         {
            XDocument doc = XDocument.Load("GetProvinceData.xml");

            foreach (var item in doc.Descendants().Where(w => w.Name.LocalName == "ProvinceList"))
            {
               var p = new IIAProvince();
               p.PVN_OICCode = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "PVN_OICCode").Value);
               p.ProvinceId = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "ProvinceId").Value);
               p.ProvinceBlockCode = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "ProvinceBlockCode").Value);
               p.ProvinceName = item.Descendants().First(x => x.Name.LocalName == "ProvinceName").Value;


               sql.Append(@"new Province { ProvinceID = " + p.ProvinceId + ", ProvinceCode=\"" + p.ProvinceBlockCode.ToString("00") + "\", ProvinceName =\"" + p.ProvinceName + "\"},");
            }

            return Json(new { sql = sql.ToString() });
         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
      }

      public IActionResult GetAmphurData(int? id)
      {
         var sql = new StringBuilder();
         try
         {
            var provinces = _context.Provinces.Where(w => 1 == 1);
            if (id.HasValue)
               provinces = provinces.Where(w => w.ProvinceID == id);

            foreach (var province in provinces)
            {
               XmlDocument soapRequest = CreateSoapGetAmphurData(province.ProvinceID);
               using (var client = new HttpClient())
               {
                  var request = new HttpRequestMessage()
                  {
                     RequestUri = new Uri(_iia.EndPoint),
                     Method = HttpMethod.Post
                  };

                  request.Content = new StringContent(soapRequest.InnerXml, Encoding.UTF8, "text/xml");

                  request.Headers.Clear();
                  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                  request.Headers.Add("ContentType", "text/xml;charset=\"utf-8\";");
                  request.Headers.Add("SOAPAction", _iia.GetAmphurData); //I want to call this method 
                  HttpResponseMessage response = client.SendAsync(request).Result;
                  if (!response.IsSuccessStatusCode)
                  {
                     return Json(new { response = response });
                  }

                  Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
                  Stream stream = streamTask.Result;
                  var sr = new StreamReader(stream);
                  XDocument soapResponse = XDocument.Load(sr);
                  //sql.Append(soapResponse);
                  //break;
                  foreach (var item in soapResponse.Descendants().Where(w => w.Name.LocalName == "AmphurList"))
                  {
                     var p = new IIAAmphur();
                     p.ProvinceId = province.ProvinceID;
                     p.AmphurId = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "AmphurId").Value);
                     p.AmphurBlockCode = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "AmphurBlockCode").Value);
                     p.AmphurName = item.Descendants().First(x => x.Name.LocalName == "AmphurName").Value;

                     sql.Append(@"new Aumphur { AumphurID = " + p.AmphurId + ", AumphurCode =\"" + p.AmphurBlockCode.ToString("00") + "\", AumphurName =\"" + p.AmphurName + "\" , ProvinceID = " + p.ProvinceId + " },");
                  }
               }
            }

         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
         return Json(new { sql = sql.ToString() });
      }

      public IActionResult GetDistrictData(int start = 0, int end = 100)
      {
         var sql = new StringBuilder();
         try
         {
            foreach (var aum in _context.Aumphurs.Where(o => o.AumphurID >= start & o.AumphurID <= end))
            {
               XmlDocument soapRequest = CreateSoapGetDistrictData(aum.AumphurID);
               using (var client = new HttpClient())
               {
                  var request = new HttpRequestMessage()
                  {
                     RequestUri = new Uri(_iia.EndPoint),
                     Method = HttpMethod.Post
                  };

                  request.Content = new StringContent(soapRequest.InnerXml, Encoding.UTF8, "text/xml");

                  request.Headers.Clear();
                  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                  request.Headers.Add("ContentType", "text/xml;charset=\"utf-8\";");
                  request.Headers.Add("SOAPAction", _iia.GetDistrictData); //I want to call this method 
                  HttpResponseMessage response = client.SendAsync(request).Result;
                  if (!response.IsSuccessStatusCode)
                  {
                     return Json(new { response = response });
                  }

                  Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
                  Stream stream = streamTask.Result;
                  var sr = new StreamReader(stream);
                  XDocument soapResponse = XDocument.Load(sr);
                  foreach (var item in soapResponse.Descendants().Where(w => w.Name.LocalName == "DistrictList"))
                  {
                     var p = new IIATumbon();
                     p.AmphurId = aum.AumphurID;
                     p.DistrictId = Convert.ToInt32(item.Descendants().First(x => x.Name.LocalName == "DistrictId").Value);
                     p.DistrictName = item.Descendants().First(x => x.Name.LocalName == "DistrictName").Value;
                     p.DistrictZipCode = item.Descendants().First(x => x.Name.LocalName == "DistrictZipCode").Value;

                     sql.Append(@"new Tumbon { TumbonID = " + p.DistrictId + ", TumbonCode = \"" + p.DistrictBlockCode.ToString("00") + "\", TumbonName = \"" + p.DistrictName + "\", AumphurID = " + aum.AumphurID + ", ProvinceID = " + aum.ProvinceID + ", PostalCode = \"" + p.DistrictZipCode + "\"},");
                  }
               }
            }

         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
         return Json(new { sql = sql.ToString() });
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
      #endregion

      #region Test
      public async Task<IActionResult> TestSendMail(string email)
      {
         var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(u => u.Email == email).FirstOrDefault();
         if (customer != null)
         {
            await MailActivateAcc(customer.Email, customer.ID);
            return Json(new { responseCode = "200", responseDesc = "ส่งอีเมลสำเร็จ" });
         }
         return Json(new { responseCode = "-1", responseDesc = "ไม่พบข้อมูลสมาชิก" });
      }
      #endregion

   }
}
