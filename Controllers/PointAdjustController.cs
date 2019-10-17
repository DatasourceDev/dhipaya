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
using System.Text;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers.Admin
{
   public class PointAdjustController : ControllerBase
   {
      public PointAdjustController(ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<PointAdjustController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
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
      public IActionResult Index(PointAdjustDTO model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         model.PointAdjusts = new List<PointAdjust>();
         return View("PointAdjust", model);
      }

      [HttpGet]
      public async Task<IActionResult> Template()
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
         var filename = webRoot + "point_adjust_template.xlsx";

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

      [HttpPost]
      public async Task<IActionResult> Modify(PointAdjustDTO model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         if (model.valid)
         {
            if (model.PointAdjusts != null && model.PointAdjusts.Count() > 0)
            {
               foreach (var item in model.PointAdjusts)
               {
                  var point = new CustomerPoint();
                  point.CustomerID = item.CustomerID;
                  point.Code = item.ConditionCode;
                  point.Name = item.Name;
                  point.Point = item.Point;
                  point.PurchaseAmt = item.PurchaseAmt;
                  point.TransacionTypeID = item.TransacionTypeID;
                  point.CustomerChanal = item.CustomerChanal;
                  point.Source = "tipsociety-adjust";

                  point.Create_On = item.Create_On;
                  point.Create_By = item.Create_By;
                  point.Update_On = item.Update_On;
                  point.Update_By = item.Update_By;
                  this._context.CustomerPoints.Add(point);
               }
               this._context.PointAdjusts.AddRange(model.PointAdjusts);
               this._context.SaveChanges();
               return RedirectToAction("Index", new { result = 1 });
            }
         }
         else
         {
            if (file == null)
               ModelState.AddModelError("Error", "ไม่พบข้อมูลไฟล์นำเข้า");
            else
            {
               model.PointAdjusts = new List<PointAdjust>();
               model.PointAdjustFails = new List<PointAdjustFail>();

               ModelState.Clear();
               using (var memoryStream = new MemoryStream())
               {
                  await file.CopyToAsync(memoryStream).ConfigureAwait(false);

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
                           if (colCount >= 2)
                           {
                              var username = worksheet.Cells["A" + row].Value.ToString();
                              var conditioncode = worksheet.Cells["B" + row].Value.ToString();
                              var purchaseAmt = NumUtil.ParseDecimal( worksheet.Cells["C" + row].Value);
                              var condition = this._context.PointConditions.Where(w => w.ConditionCode == conditioncode).FirstOrDefault();
                              var customer = this._context.Customers.Include(i => i.User).Where(w => w.User.UserName == username).FirstOrDefault();
                              if (customer != null && condition != null)
                              {
                                 var adjust = new PointAdjust();
                                 adjust.Customer = customer;
                                 adjust.ConditionCode = condition.ConditionCode;
                                 adjust.CustomerChanal = customer.Channel;
                                 adjust.TransacionTypeID = condition.TransacionTypeID;
                                 adjust.CustomerID = customer.ID;
                                 adjust.Point = GetPoint(condition, customer, purchaseAmt, false);
                                 adjust.Name = condition.Name;
                                 adjust.Create_On = DateUtil.Now();
                                 adjust.Create_By = this.HttpContext.User.Identity.Name;
                                 adjust.Update_On = DateUtil.Now();
                                 adjust.Update_By = this.HttpContext.User.Identity.Name;
                                 model.PointAdjusts.Add(adjust);
                              }
                              else
                              {
                                 var msg = new StringBuilder();
                                 if (condition == null)
                                    msg.AppendLine("ไม่พบข้อมูลเงื่อนไขการสะสมคะแนน");
                                 if (customer == null)
                                    msg.AppendLine("ไม่พบข้อมูลสมาชิก");
                                 var fail = new PointAdjustFail();
                                 fail.username = worksheet.Cells["A" + row].Value;
                                 fail.conditioncode = worksheet.Cells["B" + row].Value;
                                 fail.message = msg.ToString();
                                 fail.row = row +1;
                                 model.PointAdjustFails.Add(fail);
                              }
                           }
                           else
                              ModelState.AddModelError("Error", "รูปแบบไฟล์ไม่ถูกต้อง");
                        }
                     }
                     else
                        ModelState.AddModelError("Error", "รูปแบบไฟล์ไม่ถูกต้องหรือไม่มีข้อมูล");

                  }
               }
            }
         }
         return View("PointAdjust", model);
      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
             return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Product model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Products.Where(a => a.ProductID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               this._context.Products.Remove(model);
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }


   }
}