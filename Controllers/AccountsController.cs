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
using System.Text;
using Dhipaya.Extensions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Xml;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Collections.Generic;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{
   public class AccountsController : ControllerBase
   {
      public readonly ILoggerFactory _loggerFactory;

      public AccountsController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, ILoginServices loginServices, ILogger<AccountsController> logger, IOptions<SystemConf> conf, IOptions<Smtp> smtp, IOptions<IIA> _iia, IOptions<TIPMobile> _mobile) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._logger = logger;
         this._context = context;
         this._smtp = smtp.Value;
         this._iia = _iia.Value;
         this._mobile = _mobile.Value;
         this._conf = conf.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      #region  Register      
      public IActionResult Register()
      {
         var model = new CustomerDTO();
         return View(model);
      }

      [HttpPost]
      public async Task<IActionResult> Register(CustomerDTO model, bool repair = false)
      {
         if (ModelState.IsValid)
         {
            if (!repair)
            {
               if (string.IsNullOrEmpty(model.username))
                  model.username = model.email;
               if (!model.isDhiMember)
                  model.citizenId = null;
               if (this.isExistIDCard(model))
               {
                  var rg = new RijndaelCrypt();

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
                  ModelState.AddModelError("citizenId", "รหัสบัตรประชาชนซ้ำในระบบ");
               }
               if (this.isExistEmail(model))
                  ModelState.AddModelError("email", "อีเมลซ้ำในระบบ");
               if (this.isExistUserName(model))
                  ModelState.AddModelError("email", "รหัสผู้ใช้งานซ้ำในระบบ");
               //if (this.isExistMobileNo(model))
               //   ModelState.AddModelError("moblieNo", "เบอร์โทรศัพท์ซ้ำในระบบ");
               //if (this.isExistName(model))
               //{
               //   ModelState.AddModelError("firstName", "ชื่อนามสกุลซ้ำในระบบ");
               //   ModelState.AddModelError("lastName", "ชื่อนามสกุลซ้ำในระบบ");
               //}
               if (!string.IsNullOrEmpty(model.friendCode) && !this.isExistFriendCode(model))
                  ModelState.AddModelError("friendCode", "ไม่พบข้อมูล friend Code");
            }


            if (ModelState.IsValid)
            {
               if (model.valid)
               {
                  model.password = DataEncryptor.Decrypt(model.pEncyprt);
                  var customer = new Customer();
                  customer.Create_On = DateUtil.Now();
                  customer.ChannelUpdate = CustomerChanal.TIP;
                  customer = CustomerBinding.Binding(customer, model);

                  GetCustomerClass(customer);
                  customer.Create_On = DateUtil.Now();
                  customer.Create_By = customer.User.UserName;
                  customer.Update_On = DateUtil.Now();
                  customer.Update_By = customer.User.UserName;
                  customer.Success = false;
                  var regs = this.GetPointCondition(customer, TransacionTypeID.Register);
                  foreach (var item in regs)
                  {
                     if (item.Point.Value > 0)
                     {
                        var point = this.GetCustomerPoint(item, customer, item.Point.Value, (int)TransacionTypeID.Register, CustomerChanal.TIP, "tipsociety-register");
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
                           var point = this.GetCustomerPoint(item, customer, p, (int)TransacionTypeID.InviteFriend, CustomerChanal.TIP, "tipsociety-register");
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
                  this._context.Users.Attach(customer.User);
                  this._context.Entry(customer.User).Property(u => u.Email).IsModified = true;
                  this._context.Entry(customer.User).Property(u => u.PhoneNumber).IsModified = true;
                  this._context.Update(customer);
                  this._context.SaveChanges();

                  if (_conf.SendEmail == true && friend != null && friendpoint > 0)
                     await MailInviteFriend(friend.Email, friend, customer, friendpoint);
                  if (!repair)
                  {
                     using (var client = new HttpClient())
                     {
                        client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile/register");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var rg = new RijndaelCrypt();
                        model.username = rg.Encrypt(model.username);
                        model.password = rg.Encrypt(model.password);
                        model.status = customer.Status.toStatusNameEn();

                        StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


                        HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
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
                     if (_conf.SendEmail == true)
                        await MailActivateAcc(customer.Email, customer.ID);

                     //if (_conf.SendSMS == true)
                     //   SendSMS(customer.ID);

                     return await Login(new Login() { UserName = model.email, Password = model.password }, true);
                  }
               }
               else
               {
                  model.pEncyprt = DataEncryptor.Encrypt(model.password);
               }
               model.valid = true;
            }
         }
         return View(model);
      }

      public IActionResult RegisterCompleted(CustomerDTO model)
      {
         var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(u => u.Email == model.email).FirstOrDefault();
         if (customer != null)
            model = CustomerBinding.Binding(customer);


         return View(model);
      }
      #endregion

      #region  Login      
      public IActionResult Login(string message)
      {
         //_logger.LogError("Test LogError");
         //_logger.LogWarning("Test LogWarning");
         //_logger.LogDebug("Test LogDebug");
         //_logger.LogInformation("Test LogInformation");
         if (this._loginServices.isAuthen())
         {

            //return RedirectToAction("Index", "Home");
         }
         ViewData["ErrorMessage"] = message;
         return View();
      }

      [HttpGet]
      [AllowAnonymous]
      public async Task<IActionResult> SSO(SSODTO model)
      {
         if (string.IsNullOrEmpty(model.u))
            model.u = model.UserName;
         if (string.IsNullOrEmpty(model.p))
            model.p = model.Password;
         if (string.IsNullOrEmpty(model.p))
            model.p = model.u;
         if (!string.IsNullOrEmpty(model.u) && !string.IsNullOrEmpty(model.p))
         {
            var rg = new RijndaelCrypt();
            var u = rg.Decrypt(model.u);
            var p = rg.Decrypt(model.p);
            var f = "";
            if (!string.IsNullOrEmpty(model.f))
               f = rg.Decrypt(model.f);
            if (!string.IsNullOrEmpty(model.facebookFlag))
               f = model.facebookFlag;
            var user = this._context.Users.Include(w => w.UserRole).Where(w => w.UserName == u).FirstOrDefault();
            /*create customer imobile*/

            _logger.LogWarning(DateUtil.Now() + "");
            _logger.LogWarning("SSO");
            _logger.LogWarning(JsonConvert.SerializeObject(model));
            model.u = u;

            if (user == null)
            {
               await this.Repair(u, p, f, "loginForStatus");
               user = this._context.Users.Include(u2 => u2.UserRole).Where(u2 => u2.UserName == u).FirstOrDefault();
            }

            if (user != null)
            {
               if (user.Status != UserStatusType.InActive)
               {
                  var customer = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
                  if (customer == null)
                  {
                     ViewData["ErrorMessage"] = "ไม่พบข้อมูลผู้ใช้";
                     _logger.LogWarning(ViewData["ErrorMessage"].ToString());
                     return RedirectToAction("Login", "Accounts", new { message = ViewData["ErrorMessage"] });
                  }
                  f = customer.FacebookFlag;
                  var valid = false;
                  if (!string.IsNullOrEmpty(f) && f.ToLower() == "y")
                  {
                     valid = true;
                  }
                  else
                  {
                     if (customer.BCryptPwd == p)
                        valid = true;

                     if (!valid)
                     {
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                           string paintTextPassword = DataEncryptor.Decrypt(user.Password);
                           string passworeInDB = p;
                           if (!valid)
                           {
                              if (!string.IsNullOrEmpty(paintTextPassword) && !string.IsNullOrEmpty(passworeInDB))
                              {
                                 try
                                 {
                                    if (BCrypt.Net.BCrypt.Verify(paintTextPassword, passworeInDB))
                                       valid = true;
                                 }
                                 catch
                                 {

                                 }
                              }
                           }
                        }
                     }

                     if (!valid)
                     {
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                           string desPassword = DataEncryptor.Decrypt(user.Password);
                           if (p == desPassword)
                              valid = true;
                        }
                     }
                  }


                  if (valid)
                  {
                     this._loginServices.Login(user, true);
                     GetCustomerClass(customer);
                     customer.FirstLogedIn = true;
                     this._context.SaveChanges();

                     return RedirectToAction("Privilege", "Home", new { /*poppromo = 1 */});
                  }
               }
               else
               {
                  ViewData["ErrorMessage"] = "ถูกระงับการเป็นสมาชิก";
                  _logger.LogWarning(ViewData["ErrorMessage"].ToString());
                  return RedirectToAction("Login", "Accounts", new { message = ViewData["ErrorMessage"] });
               }
            }
         }
         ViewData["ErrorMessage"] = "รหัสผู้ใช้ หรือ รหัสผ่านไม่ถูกต้อง";
         _logger.LogWarning(ViewData["ErrorMessage"].ToString());
         return RedirectToAction("Login", "Accounts", new { message = ViewData["ErrorMessage"] });
      }


      [HttpPost]
      [AllowAnonymous]
      public async Task<IActionResult> Login(Login model, bool registed = false)
      {
         model.UserName = model.UserName.Trim();
         model.Password = model.Password.Trim();

         ModelState.Remove("");
         if (ModelState.IsValid)
         {
            //  Login statement here
            var user = this._context.Users.Include(u => u.UserRole).Where(u => u.UserName == model.UserName).FirstOrDefault();
            if (user == null)
            {
               /*create customer imobile*/
               if (user == null)
               {
                  await this.Repair(model.UserName, model.Password, null, bcrypt: BCrypt.Net.BCrypt.HashPassword(model.Password));
                  user = this._context.Users.Include(u2 => u2.UserRole).Where(u2 => u2.UserName == model.UserName).FirstOrDefault();
               }
            }

            if (user != null)
            {
               if (registed)
               {
                  if (user != null && user.Status == UserStatusType.Active)
                  {
                     this._loginServices.Login(user, model.RememberMe);
                     var customer = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
                     if (customer != null)
                     {
                        customer.FirstLogedIn = true;
                        this._context.SaveChanges();
                     }
                     return RedirectToAction("RegisterCompleted", new { Email = model.UserName });
                  }
               }
               else
               {
                  if (user.Status != UserStatusType.InActive)
                  {
                     if (user.UserRole != null && user.UserRole.RoleName == RoleName.Member)
                     {
                        var customer = this._context.Customers.Where(w => w.UserID == user.ID).FirstOrDefault();
                        if (customer == null)
                        {
                           ViewData["ErrorMessage"] = "ไม่พบข้อมูลผู้ใช้";
                           return View(model);
                        }
                        if (customer.FirstLogedIn == false && customer.Channel == CustomerChanal.TipInsure)
                        {
                           var rg = new RijndaelCrypt();
                           return RedirectToAction("ResetPwd", "Accounts", new { u = rg.Encrypt(customer.User.UserName) });
                        }
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                           string desPassword = DataEncryptor.Decrypt(user.Password);
                           if (model.Password == desPassword)
                           {
                              this._loginServices.Login(user, model.RememberMe);
                              GetCustomerClass(customer);
                              customer.FirstLogedIn = true;
                              var conditions = this.GetPointCondition(customer, TransacionTypeID.Login);
                              foreach (var con in conditions)
                              {
                              }
                              this._context.SaveChanges();
                              return RedirectToAction("Info", "Customer");

                           }
                        }

                        if (!string.IsNullOrEmpty(customer.BCryptPwd))
                        {
                           string paintTextPassword = model.Password;
                           string passworeInDB = customer.BCryptPwd;
                           if (!string.IsNullOrEmpty(paintTextPassword) && !string.IsNullOrEmpty(passworeInDB))
                           {
                              if (BCrypt.Net.BCrypt.Verify(paintTextPassword, passworeInDB))
                              {
                                 user.Password = DataEncryptor.Encrypt(model.Password);
                                 customer.Syned = true;
                                 this._context.Users.Update(user);
                                 this._loginServices.Login(user, model.RememberMe);
                                 GetCustomerClass(customer);
                                 customer.FirstLogedIn = true;
                                 this._context.SaveChanges();
                                 return RedirectToAction("Info", "Customer");
                              }
                           }
                        }


                     }
                     else if (user.UserRole.RoleName == RoleName.Merchant)
                     {
                        string desPassword = DataEncryptor.Decrypt(user.Password);
                        if (model.Password == desPassword)
                        {
                           this._loginServices.Login(user, model.RememberMe);
                           return RedirectToAction("Index", "MerchantU");
                        }
                     }
                     else
                     {
                        string desPassword = DataEncryptor.Decrypt(user.Password);
                        if (model.Password == desPassword)
                        {
                           this._loginServices.Login(user, model.RememberMe);
                           return RedirectToAction("Index", "Admin");
                        }
                     }
                  }
                  else
                  {
                     ViewData["ErrorMessage"] = "ถูกระงับการเป็นสมาชิก";
                     return View(model);
                  }
               }

            }

         }
         ViewData["ErrorMessage"] = "รหัสผู้ใช้ หรือ รหัสผ่านไม่ถูกต้อง";
         return View(model);
      }

      public IActionResult Logout()
      {
         this._loginServices.Logout();
         return RedirectToAction("Login", "Accounts");
      }
      #endregion

      #region  Forgot      
      public IActionResult Forgot(string email)
      {
         var model = new Forgot();
         model.Email = email;
         return View(model);
      }

      [HttpPost]
      public async Task<IActionResult> Forgot(Forgot model)
      {
         if (ModelState.IsValid)
         {
            var customer = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.User.UserName == model.Email).FirstOrDefault();
            if (customer != null)
            {
               model.Customer = customer;
               var rg = new RijndaelCrypt();
               model.Url = Url.Action("ResetPwd", new { u = rg.Encrypt(customer.User.UserName) });
               var htmlToConvert = await RenderViewAsync("MailForgotPwd", model, true);
               var msg = EmailUtil.sendNotificationEmail(_smtp, customer.Email, "เปลี่ยนรหัสผ่าน", htmlToConvert.ToString());
               ViewData["Message"] = "ระบบกำลังส่่งการกำหนดรหัสผ่านใหม่ไปยังอีเมลของท่าน";
               return View(model);
            }
            ViewData["ErrorMessage"] = "ไม่พบอีเมลในระบบ";
         }
         else
            ViewData["ErrorMessage"] = "โปรดระบุอีเมล";

         return View(model);
      }

      public IActionResult ResetPwd(string u)
      {
         var rg = new RijndaelCrypt();
         var uname = rg.Decrypt(u);
         var user = this._context.Users.Include(i => i.UserRole).Where(w => w.UserName == uname).FirstOrDefault();
         if (user != null)
         {

            if (user != null && user.Status == UserStatusType.Active)
            {
               this._loginServices.Login(user, true);
               return RedirectToAction("ResetPwd", "Customer");
            }
         }
         ViewData["ErrorMessage"] = "พบข้อผิดพลาด";
         return RedirectToAction("Login", "Accounts");

      }


      #endregion

      public IActionResult TermsAndConditions()
      {
         return View("TermsAndConditions");
      }

      public async Task<IActionResult> SendDeleteAccount(string idcard)
      {
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
         var model = new List<string>();
         model = customers.Select(s => s.Email).ToList();
         return View(model);
      }
      public async Task<IActionResult> Terminate(string code)
      {
         var acccode = this._context.AccountCodes.Where(w => w.Code == code && w.Status == StatusType.Active).FirstOrDefault();
         if (acccode != null)
         {
            var customer = _context.Customers.Where(w => w.ID == acccode.CustomerID).FirstOrDefault();
            if (customer != null)
            {


               var redeems = this._context.Redeems.Where(w => w.CustomerID == customer.ID);
               var mobile = this._context.MobilePoints.Where(w => w.CustomerID == customer.ID);
               var classchages = this._context.CustomerClassChanges.Where(w => w.CustomerID == customer.ID);
               var adjusts = this._context.PointAdjusts.Where(w => w.CustomerID == customer.ID);
               var points = this._context.CustomerPoints.Where(w => w.CustomerID == customer.ID);

               var tempcus = JsonConvert.SerializeObject(customer, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
               var tcus = new TerminateCustomer();
               tcus = JsonConvert.DeserializeObject<TerminateCustomer>(tempcus);
               tcus.ID = 0;
               tcus.CustomerID = customer.ID;
               this._context.TerminateCustomers.Add(tcus);

               foreach (var item in redeems)
               {
                  var temp = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var t = new TerminateRedeem();
                  t = JsonConvert.DeserializeObject<TerminateRedeem>(temp);
                  t.ID = 0;
                  this._context.TerminateRedeems.Add(t);
               }
               foreach (var item in points)
               {
                  var temp = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var t = new TerminateCustomerPoint();
                  t = JsonConvert.DeserializeObject<TerminateCustomerPoint>(temp);
                  t.ID = 0;
                  this._context.TerminateCustomerPoints.Add(t);
               }
               foreach (var item in mobile)
               {
                  var temp = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var t = new TerminateMobilePoint();
                  t = JsonConvert.DeserializeObject<TerminateMobilePoint>(temp);
                  t.ID = 0;
                  this._context.TerminateMobilePoints.Add(t);
               }
               foreach (var item in classchages)
               {
                  var temp = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var t = new TerminateCustomerClassChange();
                  t = JsonConvert.DeserializeObject<TerminateCustomerClassChange>(temp);
                  t.ID = 0;
                  this._context.TerminateCustomerClassChanges.Add(t);
               }
               foreach (var item in adjusts)
               {
                  var temp = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var t = new TerminatePointAdjust();
                  t = JsonConvert.DeserializeObject<TerminatePointAdjust>(temp);
                  t.ID = 0;
                  this._context.TerminatePointAdjusts.Add(t);
               }

               var user = this._context.Users.Where(w => w.ID == customer.UserID).FirstOrDefault();
               if (user != null)
               {
                  var rg = new RijndaelCrypt();
                  var u = rg.Encrypt(user.UserName);
                  var p = rg.Encrypt(DataEncryptor.Decrypt(user.Password));
                  var flag = rg.Encrypt(customer.FacebookFlag);

                  var tempuser = JsonConvert.SerializeObject(user, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                  var tuser = new TerminateUser();
                  tuser = JsonConvert.DeserializeObject<TerminateUser>(tempuser);
                  tuser.ID = 0;
                  tuser.CustomerID = customer.ID;
                  this._context.TerminateUsers.Add(tuser);

                  this._context.CustomerPoints.RemoveRange(points);
                  this._context.MobilePoints.RemoveRange(mobile);
                  this._context.CustomerClassChanges.RemoveRange(classchages);
                  this._context.PointAdjusts.RemoveRange(adjusts);
                  this._context.Redeems.RemoveRange(redeems);
                  this._context.Customers.Remove(customer);
                  this._context.Users.Remove(user);

                  acccode.Status = StatusType.InActive;
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
         }
         this._loginServices.Logout();
         return View();
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
