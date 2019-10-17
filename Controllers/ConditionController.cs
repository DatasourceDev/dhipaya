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
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers.Admin
{
   public class ConditionController : ControllerBase
   {
      public ConditionController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<ConditionController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
      public IActionResult Index(PointConditionDTO model)
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
         model.PointConditions = this._context.PointConditions
                               .Include(s => s.PointConditionProducts)
                               .OrderBy(c => c.TransacionTypeID).ThenBy(o => o.OutletCode).ThenBy(o=>o.ConditionCode);


         ViewBag.ItemCount = model.PointConditions.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 100);
         if (ViewBag.ItemCount % 100 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.PointConditions = model.PointConditions.Skip(skipRows).Take(100);

         return View("Condition", model);
      }

      [HttpGet]
      public IActionResult Create()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = new PointCondition();
         model.Status = StatusType.Active;
         model.IsAllDay = true;
         model.Gold = true;
         model.Silver = true;
         ViewBag.ListTransacionType = this._context.PointTransacionTypes.OrderBy(o => o.TransacionTypeID);
         ViewBag.ListProduct = this._context.Products.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ProductCode);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         model.ProductList = new List<PointConditionProduct>();
         model.CustomerClassList = new List<PointConditionCustomerClass>();

         return View("ConditionInfo", model);
      }

      [HttpGet]
      public IActionResult Update(int? id)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.PointConditions.Include(s => s.PointConditionCustomerClasses).Include(s => s.PointConditionProducts).Include(s => s.PointConditionTiers).Where(w => w.ConditionID == id).FirstOrDefault();
         if (model != null)
         {
            if (model.ProductList.Count <= 0)
               model.PointConditionProducts = new Collection<PointConditionProduct>() { new PointConditionProduct() };

            if (model.TierList.Count <= 0)
               model.PointConditionTiers = new Collection<PointConditionTier>() { new PointConditionTier() };

            if (model.CustomerClassList.Count <= 0)
               model.PointConditionCustomerClasses = new Collection<PointConditionCustomerClass>() { new PointConditionCustomerClass() };

            model.sDate = DateUtil.ToDisplayDate(model.StartDate);
            model.eDate = DateUtil.ToDisplayDate(model.EndDate);

            model.TierList = model.TierList.OrderBy(o => o.PurchaseAmtFrom).ToList();
         }

         ViewBag.ListTransacionType = this._context.PointTransacionTypes.OrderBy(o => o.TransacionTypeID);
         ViewBag.ListProduct = this._context.Products.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ProductCode);
         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("ConditionInfo", model);
      }

      [HttpPost]
      public IActionResult Modify(PointCondition model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         if (ModelState.IsValid)
         {
            model.StartDate = DateUtil.ToDate(model.sDate);
            model.EndDate = DateUtil.ToDate(model.eDate);
            if (!model.IsAllDay)
            {
               if (!model.IsSun & !model.IsMon & !model.IsTue & !model.IsWed & !model.IsThu & !model.IsFri & !model.IsSat)
                  model.IsAllDay = true;
            }
            using (var dbContextTransaction = this._context.Database.BeginTransaction())
            {
               try
               {
                  if (model.ConditionID <= 0)
                  {
                     model.Create_On = DateUtil.Now();
                     model.Create_By = this.HttpContext.User.Identity.Name;
                     model.Update_On = DateUtil.Now();
                     model.Update_By = this.HttpContext.User.Identity.Name;
                     if (model.TransacionTypeID == (int)TransacionTypeID.BuyInsure | model.TransacionTypeID == (int)TransacionTypeID.Renew)
                     {
                        foreach (var item in model.ProductList.Where(w => w.ProductID > 0))
                        {
                           model.PointConditionProducts.Add(item);
                        }
                     }
                     if (model.PointType == PointType.Tier)
                     {
                        foreach (var item in model.TierList)
                        {
                           model.PointConditionTiers.Add(item);
                        }
                     }
                     foreach (var item in model.CustomerClassList.Where(w => w.CustomerClassID > 0))
                     {
                        model.CustomerClassList.Add(item);
                     }
                     this._context.PointConditions.Add(model);
                     this._context.SaveChanges();
                     dbContextTransaction.Commit();
                     return RedirectToAction("Index");
                  }
                  else
                  {
                     model.Update_On = DateUtil.Now();
                     model.Update_By = this.HttpContext.User.Identity.Name;
                     this._context.Update(model);

                     /*Product*/
                     IEnumerable<int> itemIDs = Enumerable.Empty<int>();
                     if ((model.TransacionTypeID == (int)TransacionTypeID.BuyInsure | model.TransacionTypeID == (int)TransacionTypeID.Renew) && model.ProductList != null)
                     {
                        itemIDs = model.ProductList.Where(w => w.ID > 0 & w.ProductID > 0).Select(s => s.ID);
                     }
                     foreach (var item in this._context.PointConditionProducts.Where(w => w.ConditionID == model.ConditionID))
                     {
                        if (!itemIDs.Contains(item.ID))
                        {
                           this._context.PointConditionProducts.Remove(item);
                           var tmpitem = model.PointConditionProducts.Where(w => w.ID == item.ID).FirstOrDefault();
                           if (tmpitem != null)
                              model.PointConditionProducts.Remove(tmpitem);
                        }
                     }

                     if (model.TransacionTypeID == (int)TransacionTypeID.BuyInsure | model.TransacionTypeID == (int)TransacionTypeID.Renew)
                     {
                        foreach (var item in model.ProductList)
                        {
                           item.ConditionID = model.ConditionID;
                            if (item.ID <= 0)
                              this._context.PointConditionProducts.Add(item);

                        }
                     }
                     this._context.SaveChanges();

                     /*Tier*/
                     itemIDs = Enumerable.Empty<int>();

                     if (model.PointType == PointType.Tier && model.TierList != null)
                     {
                        itemIDs = model.TierList.Where(w => w.ID > 0).Select(s => s.ID);
                     }
                     foreach (var item in this._context.PointConditionTiers.Where(w => w.ConditionID == model.ConditionID))
                     {
                        if (!itemIDs.Contains(item.ID))
                        {
                           this._context.PointConditionTiers.Remove(item);
                           var tmpitem = model.PointConditionTiers.Where(w => w.ID == item.ID).FirstOrDefault();
                           if (tmpitem != null)
                              model.PointConditionTiers.Remove(tmpitem);
                        }
                     }
                     if (model.PointType == PointType.Tier && model.TierList != null)
                     {
                        foreach (var item in model.TierList)
                        {
                           item.ConditionID = model.ConditionID;
                           if (item.ID <= 0)
                              this._context.PointConditionTiers.Add(item);
                           else
                              this._context.Update(item);
                        }
                     }
                     this._context.SaveChanges();

                     /*Customer Class*/
                     itemIDs = Enumerable.Empty<int>();

                     if ( model.CustomerClassList != null)
                     {
                        itemIDs = model.CustomerClassList.Where(w => w.ID > 0).Select(s => s.ID);
                     }
                     foreach (var item in this._context.PointConditionCustomerClasses.Where(w => w.ConditionID == model.ConditionID))
                     {
                        if (!itemIDs.Contains(item.ID))
                        {
                           this._context.PointConditionCustomerClasses.Remove(item);
                           var tmpitem = model.PointConditionCustomerClasses.Where(w => w.ID == item.ID).FirstOrDefault();
                           if (tmpitem != null)
                              model.PointConditionCustomerClasses.Remove(tmpitem);
                        }
                     }
                     if ( model.CustomerClassList != null)
                     {
                        foreach (var item in model.CustomerClassList)
                        {
                           item.ConditionID = model.ConditionID;
                           if (item.ID <= 0)
                              this._context.PointConditionCustomerClasses.Add(item);
                           else
                              this._context.Update(item);
                        }
                     }
                     this._context.SaveChanges();

                     this._context.PointConditionProducts.RemoveRange(this._context.PointConditionProducts.Where(w => w.ProductID <= 0));
                     this._context.SaveChanges();
                     dbContextTransaction.Commit();

                     return RedirectToAction("Index");
                  }
               }
               catch (Exception ex)
               {
                  Console.WriteLine(ex.ToString());
                  dbContextTransaction.Rollback();
                  ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล: " + ex.Message);
               }

             
            }
         }
         else
         {
            ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล");
         }
         ViewBag.ListTransacionType = this._context.PointTransacionTypes.OrderBy(o => o.TransacionTypeID);
         ViewBag.ListProduct = this._context.Products.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ProductCode);
         if (model.ProductList == null)
            model.ProductList = new List<PointConditionProduct>();
         return View("ConditionInfo", model);
      }


      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         PointCondition model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.PointConditions.Include(i => i.PointConditionCustomerClasses).Include(i => i.PointConditionProducts).Include(i => i.PointConditionTiers).Where(a => a.ConditionID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (model.PointConditionProducts.Count > 0)
               {
                  this._context.PointConditionProducts.RemoveRange(model.PointConditionProducts);
               }
               if (model.PointConditionTiers.Count > 0)
               {
                  this._context.PointConditionTiers.RemoveRange(model.PointConditionTiers);
               }
               if (model.PointConditionCustomerClasses.Count > 0)
               {
                  this._context.PointConditionCustomerClasses.RemoveRange(model.PointConditionCustomerClasses);
               }
               this._context.PointConditions.Remove(model);
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }


   }
}