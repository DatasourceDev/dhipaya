using System.Linq;
using Dhipaya.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Dhipaya.Models;
using Microsoft.Extensions.Options;
using Dhipaya.Extensions;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Dhipaya.Services;
using Dhipaya.ModelsDapper;
using Microsoft.Extensions.FileProviders;

namespace Dhipaya.Controllers
{
   public class HomeController : ControllerBase
   {
      public HomeController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<HomeController> logger, IOptions<Smtp> smtp, IOptions<TIPMobile> _mobile, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._logger = logger;
         this._context = context;
         this._smtp = smtp.Value;
         this._mobile = _mobile.Value;
         this._conf = conf.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;

      }


      public IActionResult Index()
      {
         var model = new HomeDTO();

         if (_loginServices.isAuthen())
         {
            var customer = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (customer != null)
               GetCustomerClass(customer, true);
            model.CustomerClass = ViewBag.CustomerClass;
         }
         return View(model);
      }
      [HttpGet]
      public IActionResult Contact()
      {
         var model = new Contact();
         return View(model);
      }

      [HttpPost]
      public async Task<IActionResult> Contact(Contact model)
      {
         if (ModelState.IsValid)
         {
            var blocked = this._context.ContactBlocks.Where(w => w.Email == model.Email).FirstOrDefault();
            if (blocked == null)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = model.Email;
               this._context.Contacts.Add(model);
               this._context.SaveChanges();
               if (_conf.SendEmail == true)
               {
                  await MailContact(model);
                  model = new Contact();
                  ModelState.Clear();
                  return View(model);
               }
            }

         }
         return View(model);
      }

      public async Task<IActionResult> MailContact(Contact model)
      {

         var htmlToConvert = await RenderViewAsync("MailContact", model, true);
         var msg = EmailUtil.sendNotificationEmail(_smtp, _conf.SupportEmail, "ติดต่อจากคุณ" + model.Name, htmlToConvert.ToString());
         if (string.IsNullOrEmpty(msg))
            ViewData["Message"] = "ส่งข้อมูลสำเร็จ";
         else
            ViewData["ErrorMessage"] = "ไม่สามารถส่งข้อมูลได้ กรุณาติดต่อไปยังผู้ดูแลระบบ";
         return Json(new { Msg = msg });
      }
      [HttpGet]
      public IActionResult GetPrivilegeDetail(int id)
      {

         var model = this._context.Privileges.Include(s => s.Merchant)
                                 .Include(s => s.PrivilegeImages)
                                 .Include(s => s.PrivilegeCustomerClasses)
                                 .Include(s => s.MerchantCategory)
                                 .Where(w => w.PrivilegeID == id).FirstOrDefault();
         if (model != null)
         {
            var icon = model.MerchantCategory != null ? Url.Content(model.MerchantCategory.Logo) : "";
            var url = Url.Content("~/tip/img/privilege-default.jpg");
            if (!string.IsNullOrEmpty(model.ImgUrl))
               url = Url.Content(model.ImgUrl);
            else if (model.Merchant != null && !string.IsNullOrEmpty(model.Merchant.Url))
               url = Url.Content(model.Merchant.Url);
            var periodfrom = "วันนี้ - ";
            if (model.StartDate.HasValue)
               periodfrom = model.StartDate.Value.Day + " " + DateUtil.GetShortMonth(model.StartDate.Value.Month) + " " + model.StartDate.Value.Year + " - ";
            var periodto = "";
            if (model.EndDate.HasValue)
               periodto = model.EndDate.Value.Day + " " + DateUtil.GetShortMonth(model.EndDate.Value.Month) + " " + model.EndDate.Value.Year;
            var youtube = "";
            if (!string.IsNullOrEmpty(model.Youtube))
            {
               model.Youtube = model.Youtube.Replace("\n", "");
               youtube = Url.Content(model.Youtube);
               if (!model.Youtube.Contains("https://"))
                  youtube = Url.Content("https://" + model.Youtube);
            }
            else if (!string.IsNullOrEmpty(model.Merchant.Youtube))
            {
               model.Youtube = model.Merchant.Youtube.Replace("\n", "");
               youtube = Url.Content(model.Merchant.Youtube);
               if (!model.Merchant.Youtube.Contains("https://"))
                  youtube = Url.Content("https://" + model.Merchant.Youtube);
            }
            var imgs = model.PrivilegeImages.Select(s => Url.Content(s.Url));

            var error = "";
            Customer customer = null;
            if (_loginServices.isAuthen())
            {
               customer = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
               if (customer != null)
               {
                  if (customer.CustomerClassID == 1 | customer.CustomerClassID == 2)
                  {
                     if (!model.PrivilegeCustomerClasses.Where(w => w.CustomerClassID == customer.CustomerClassID).Any())
                     {
                        error = "เฉพาะลูกค้า ";
                        foreach (var item in model.PrivilegeCustomerClasses.Where(w => w.ID > 0))
                        {
                           var cclass = _context.CustomerClasses.Where(w => w.ID == item.CustomerClassID).FirstOrDefault();
                           if (cclass != null)
                              error += " " + cclass.Name;
                        }
                        error += " เท่านั้น";
                     }
                  }
               }
            }
               
            return Json(new
            {
               msg = error,
               result = 1,
               customerID = customer != null ? customer.ID : 0,
               privilegeID = model.PrivilegeID,
               condition = StrUtil.Raw(model.PrivilegeCondition),
               image = url,
               iconUrl = icon,
               outlets = StrUtil.Raw(model.Allowable_Outlet),
               privilegeName = StrUtil.Raw(model.PrivilegeName),
               periodFrom = periodfrom,
               periodTo = periodto,
               youtube = youtube,
               imps = imgs,
               creditPoint = model.CreditPoint,
               merchantName = StrUtil.Raw(model.Merchant.MerchantName),
               categoryName = StrUtil.Raw(model.MerchantCategory.CategoryName),
               redeemtype = model.RedeemType,
            });
         }
         return Json(new { error = "ไม่พบข้อมูลสิทธิพิเศษ", result = -1 });
      }

      [HttpGet]
      public IActionResult Privilege(PrivilegeDTO model)
      {

         ViewData["FBAppID"] = this._conf.FBAppID;
         model.Privileges = this._context.Privileges
                                 .Include(s => s.Merchant)
                                 .Include(s => s.PrivilegeImages)
                                 .Include(s => s.PrivilegeCustomerClasses)
                                 .Include(s => s.MerchantCategory)
                                 .Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));


         if (model.CategoryID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.CategoryID == model.CategoryID);
         if (!string.IsNullOrEmpty(model.Outlets))
            model.Privileges = model.Privileges.Where(w => (!string.IsNullOrEmpty(w.Allowable_Outlet) && w.Allowable_Outlet.ToLower().Contains(model.Outlets.ToLower())) | (!string.IsNullOrEmpty(w.Merchant.MerchantName) && w.Merchant.MerchantName.ToLower().Contains(model.Outlets.ToLower())));
         if (model.ProvinceID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.Merchant.ProvinceID == model.ProvinceID);

         if (model.CustomerClassID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == model.CustomerClassID));

         if (_loginServices.isAuthen())
         {
            var customer = this._context.Customers.Where(w => w.User.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (customer != null)
            {
               GetCustomerClass(customer, true);
               var customerclass = _context.CustomerClasses.Where(w => w.ID == customer.CustomerClassID).FirstOrDefault();
               if (customerclass != null && customerclass.ID != 1 && customerclass.ID != 2)
               {
                  /*TIP Lady or Other*/
                  model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 1) | w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 2) | w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == customerclass.ID));
                  ViewBag.ListCustomerClass = this._context.CustomerClasses.Where(w => w.Status == StatusType.Active & (w.ID == 1 | w.ID == 2 | w.ID == customerclass.ID));
               }
               else
               {
                  model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 1) | w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 2));
                  ViewBag.ListCustomerClass = this._context.CustomerClasses.Where(w => w.Status == StatusType.Active & (w.ID == 1 | w.ID == 2));
               }
            }
            else
            {
               model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 1) | w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 2));
               ViewBag.ListCustomerClass = this._context.CustomerClasses.Where(w => w.Status == StatusType.Active & (w.ID == 1 | w.ID == 2));
            }
         }
         else
         {
            model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 1) | w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == 2));
            ViewBag.ListCustomerClass = this._context.CustomerClasses.Where(w => w.Status == StatusType.Active & (w.ID == 1 | w.ID == 2));
         }
         

         model.AllPrivilegeCnt = model.Privileges.Count();
         model.Privileges = model.Privileges.OrderBy(c => c.Index).ThenByDescending(o => o.PrivilegeID).Take(12);

         model.Provinces = this._context.Provinces.OrderBy(b => b.ProvinceName);
         return View("Privilege", model);
      }

      [HttpGet]
      public IActionResult LoadPrivilege(PrivilegeDTO model)
      {

         model.Privileges = this._context.Privileges
                                 .Include(s => s.Merchant)
                                 .Include(s => s.MerchantCategory)
                                 .Include(s => s.PrivilegeImages)
                                 .Include(s => s.PrivilegeCustomerClasses)
                                 .Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         //if (model.LastPrivilegeID.HasValue)
         //   model.Privileges = model.Privileges.Where(w => w.PrivilegeID < model.LastPrivilegeID);

         if (model.LastIndex.HasValue)
            model.Privileges = model.Privileges.Where(w => w.Index > model.LastIndex);

         if (model.CategoryID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.CategoryID == model.CategoryID);

         if (!string.IsNullOrEmpty(model.Outlets))
            model.Privileges = model.Privileges.Where(w => (!string.IsNullOrEmpty(w.Allowable_Outlet) && w.Allowable_Outlet.ToLower().Contains(model.Outlets.ToLower())) | (!string.IsNullOrEmpty(w.Merchant.MerchantName) && w.Merchant.MerchantName.ToLower().Contains(model.Outlets.ToLower())));

         if (model.ProvinceID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.Merchant.ProvinceID == model.ProvinceID);

         if (model.CustomerClassID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.PrivilegeCustomerClasses.Any(s => s.CustomerClassID == model.CustomerClassID));

         model.Privileges = model.Privileges.OrderBy(c => c.Index).ThenByDescending(o => o.PrivilegeID).Take(12);

         return PartialView("_Privilegies", model);
      }

      [HttpGet]
      public IActionResult PrivilegeInfo(int? id)
      {
         ViewData["FBAppID"] = this._conf.FBAppID;
         var model = this._context.Privileges.Include(i => i.PrivilegeImages).Include(i => i.MerchantCategory).Include(i => i.Merchant).Where(w => w.Status == StatusType.Active & w.PrivilegeID == id).FirstOrDefault();
         if (model == null)
            return RedirectToAction("Privilege");
         return View(model);
      }
      [HttpGet]
      public IActionResult About()
      {
         var model = this._context.AboutUss.FirstOrDefault();
         if (model == null)
            model = new AboutUs();
         return View(model);
      }

      [HttpGet]
      public IActionResult NewsActivity(NewsActivityDTO model)
      {
         model.NewsActivities = this._context.NewsActivities.Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));
         if (model.GroupID.HasValue)
            model.NewsActivities = model.NewsActivities.Where(w => w.GroupID == model.GroupID);

         model.NewsActivities = model.NewsActivities.OrderBy(o => o.Index);
         var favorities = this._context.NewsActivities.Where(w => w.IsFavorite).OrderBy(o => o.Index).Take(3);
         ViewBag.ListGroup = this._context.NewsActivityGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         if (favorities.Count() == 0)
            favorities = this._context.NewsActivities.Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date)).OrderByDescending(o => o.Update_On).Take(3);
         ViewBag.Favorities = favorities;

         return View(model);
      }
      [HttpGet]
      public IActionResult NewsActivityInfo(int? id)
      {
         ViewData["FBAppID"] = this._conf.FBAppID;

         var model = this._context.NewsActivities.Include(i => i.NewsActivityImages).Where(w => w.Status == StatusType.Active & w.ID == id).FirstOrDefault();
         var favorities = this._context.NewsActivities.Where(w => w.IsFavorite).OrderBy(o => o.Index).Take(3);
         if (favorities.Count() == 0)
            favorities = this._context.NewsActivities.Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date)).OrderByDescending(o => o.Update_On).Take(3);
         ViewBag.Favorities = favorities;
         return View(model);
      }

      [HttpGet]
      public IActionResult Question(QuestionDTO model)
      {
         model.Questions = this._context.Questions.Where(w => w.Status == StatusType.Active & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date) & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));

         if (model.GroupID.HasValue)
            model.Questions = model.Questions.Where(w => w.QuestionGroupID == model.GroupID);
         if (!string.IsNullOrEmpty(model.search_text))
            model.Questions = model.Questions
               .Where(w => (!string.IsNullOrEmpty(w.Title) && w.Title.ToLower().Contains(model.search_text.ToLower()))
               | (!string.IsNullOrEmpty(w.Description) && w.Description.ToLower().Contains(model.search_text.ToLower())));

         model.Questions = model.Questions.OrderBy(o => o.Index);
         ViewBag.ListGroups = this._context.QuestionGroups.OrderBy(o => o.Name);
         return View(model);
      }

      [HttpGet]
      public IActionResult Subscribe(string email)
      {
         if (string.IsNullOrEmpty(email))
         {
            return Json(new { result = 1, desc = "กรุณาระบุอีเมล" });
         }

         var subscriber = this._context.Subscribers.Where(w => w.Email == email).FirstOrDefault();
         if (subscriber == null)
         {
            subscriber = new Subscriber();
            subscriber.Email = email;
            subscriber.Create_By = email;
            subscriber.Create_On = DateUtil.Now();
            this._context.Subscribers.Add(subscriber);
            this._context.SaveChanges();
            return Json(new { result = 1 });
         }
         return Json(new { result = 1 });
      }

      public IActionResult Error(ErrorViewModel model)
      {
         model.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
         return View(model);
      }

      [HttpGet]
      public async Task<IActionResult> PrivacyPolicy()
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
         var filename = webRoot + "privacy_policy.pdf";

         IFileProvider provider = new PhysicalFileProvider(webRoot);
         if (System.IO.File.Exists(filename))
         {
            var memory = new MemoryStream();
            using (var stream = new FileStream(filename, FileMode.Open))
            {
               await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var mimeType = "application/pdf";
            return File(memory, mimeType, Path.GetFileName(filename));
         }
         return null;
      }

   }
}
