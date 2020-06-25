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
using System.Collections.ObjectModel;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace Dhipaya.Controllers.Admin
{
   public class PrivilegeController : ControllerBase
   {
      private string[] extensionList = new string[] { ".jpg", ".gif", ".png", ".jpeg", ".bpm" };
      public PrivilegeController(IPrivilegeRepository priRepo, ICustomerRepository cusRepo, IReportRepository rptRepo, ChFrontContext context, IOptions<SystemConf> conf, ILogger<PrivilegeController> logger, IOptions<TIPMobile> _mobile, IOptions<Smtp> smtp, IOptions<IIA> _iia, ILoginServices loginServices) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._context = context;
         this._mobile = _mobile.Value;
         this._logger = logger;
         this._conf = conf.Value;
         this._smtp = smtp.Value;
         this._loginServices = loginServices;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
         this._priRepo = priRepo;
      }


      #region Privilege
      [HttpGet]
      public IActionResult Index(PrivilegeDTO model)
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

         model.Privileges = this._context.Privileges
                      .Include(s => s.Merchant)
                      .Include(s => s.MerchantCategory)
                      .Include(s => s.Redeems)
                      .OrderBy(c => c.Index).ThenByDescending(o => o.PrivilegeID);

         if (model.CategoryID.HasValue)
            model.Privileges = model.Privileges.Where(w => w.CategoryID == model.CategoryID);
         if (!string.IsNullOrEmpty(model.search_text))
         {
            var text = model.search_text.ToLower();
            model.Privileges = model.Privileges
               .Where(c => (!string.IsNullOrEmpty(c.PrivilegeName) && c.PrivilegeName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Merchant.MerchantName) && c.Merchant.MerchantName.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.Allowable_Outlet) && c.Allowable_Outlet.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeCondition) && c.PrivilegeCondition.ToLower().Contains(text))
                  | (!string.IsNullOrEmpty(c.PrivilegeDesc) && c.PrivilegeDesc.ToLower().Contains(text))
               );
         }

         ViewBag.ItemCount = model.Privileges.Count();
         ViewBag.PageLength = (ViewBag.ItemCount / 25);
         if (ViewBag.ItemCount % 25 > 0)
            ViewBag.PageLength += 1;
         ViewBag.PageNo = pageno;
         model.Privileges = model.Privileges.Skip(skipRows).Take(25);
         model.MerchantCategorys = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         return View("Privilege", model);
      }

      public IActionResult ReOrder(int? pno, int index, int id, string search_text, int? CategoryID)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         if (index > 0 && id > 0)
         {
            var reindex = this._context.Privileges.Where(a => a.PrivilegeID == id).FirstOrDefault();
            if (reindex == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (index > this._context.Privileges.Count())
                  index = this._context.Privileges.Count();
               reindex.Index = index;
               this._context.SaveChanges();
               var i = 1;
               foreach (var item in this._context.Privileges.OrderBy(o => o.Index))
               {
                  if (i == index && item.PrivilegeID != id)
                  {
                     i++;
                     item.Index = i;
                     i++;

                  }
                  else if (item.PrivilegeID != id)
                  {
                     item.Index = i;
                     i++;

                  }
                  else if (item.PrivilegeID == id)
                  {

                  }
                  else
                  {
                     i++;
                  }
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index", new { id = 1, pno = pno, search_text = search_text, CategoryID = CategoryID });
      }

      public IActionResult MoveUp()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Privilege model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Privileges.Where(a => a.PrivilegeID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Privileges.Where(w => w.Index < model.Index).OrderByDescending(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Privileges.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.PrivilegeID == item.PrivilegeID)
                  {
                     latestindex.Index = i + 1;
                  }
                  else if (latestindex != null && model.PrivilegeID == item.PrivilegeID)
                  {
                     item.Index = i;
                     i += 2;
                  }
                  else
                  {
                     item.Index = i;
                     i++;
                  }
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }
      public IActionResult MoveDown()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Privilege model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Privileges.Where(a => a.PrivilegeID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               var latestindex = this._context.Privileges.Where(w => w.Index > model.Index).OrderBy(o => o.Index).FirstOrDefault();
               var i = 1;
               foreach (var item in this._context.Privileges.OrderBy(o => o.Index))
               {
                  if (latestindex != null && latestindex.PrivilegeID == item.PrivilegeID)
                  {
                     latestindex.Index = i;
                     i += 2;
                  }
                  else if (latestindex != null && model.PrivilegeID == item.PrivilegeID)
                  {
                     item.Index = i + 1;
                  }
                  else
                  {
                     item.Index = i;
                     i++;
                  }
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }

      [HttpGet]
      public IActionResult Create(int? mid)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = new Privilege();
         if (mid.HasValue)
            model.MerchantID = mid;
         model.NextInit = PrivilegeTab.condition;
         model.Status = StatusType.Active;
         model.CustomerClassList = new List<PrivilegeCustomerClass>();
         ViewBag.ListMerchant = this._context.Merchants.Include(i => i.MerchantCategories).Where(w => w.Status == StatusType.Active).OrderBy(o => o.MerchantName);
         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);

         ViewBag.ListCustomerClass = this._context.CustomerClasses;

         return View("PrivilegeInfo", model);
      }

      [HttpGet]
      public async Task<IActionResult> Update(int? id, string tab, string code)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var model = this._context.Privileges
                        .Include(i => i.Merchant)
                        .Include(i => i.PrivilegeImages)
                        .Include(i => i.Merchant.MerchantCategories)
                        .Include(i => i.PrivilegeCustomerClasses)
                        .Include(i => i.PrivilegeCodes)
                        .Where(w => w.PrivilegeID == id).FirstOrDefault();
         if (model != null)
         {
            if (model.CustomerClassList.Count <= 0)
               model.PrivilegeCustomerClasses = new List<PrivilegeCustomerClass>();

            int pageno = 1;
            if (this.RouteData.Values["pno"] != null)
            {
               pageno = NumUtil.ParseInteger(this.RouteData.Values["pno"].ToString());
               if (pageno == 0)
                  pageno = 1;
            }
            var modelcode = new ModelReportBaseDTO();
            modelcode.search_code = code;
            modelcode.pno = pageno;
            modelcode.pmax = 10;
            modelcode.search_privilege = model.PrivilegeID;
            var codes = await _priRepo.ListCode(modelcode);

            ViewBag.ItemCount = modelcode.totalrow;
            ViewBag.PageLength = (ViewBag.ItemCount / 10);
            if (ViewBag.ItemCount % 10 > 0)
               ViewBag.PageLength += 1;
            ViewBag.PageNo = pageno;
            model.PrivilegeCodes = codes;


            //if (model.PrivilegeCodeList.Count <= 0)
            //   model.PrivilegeCodes = new Collection<PrivilegeCode>() { new PrivilegeCode() { Status = StatusType.Active } };

            model.sDate = DateUtil.ToDisplayDate(model.StartDate);
            model.eDate = DateUtil.ToDisplayDate(model.EndDate);
            model.tab = tab;
         }
         ViewBag.ListMerchant = this._context.Merchants.Include(i => i.MerchantCategories).OrderBy(o => o.MerchantName);
         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         ViewBag.TotalQuantity = this._context.Redeems.Where(w => w.PrivilegeID == id).Count();
         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         return View("PrivilegeInfo", model);
      }


      [HttpGet]
      public bool ImageDelete(int id)
      {
         if (id > 0)
         {
            var img = this._context.PrivilegeImages.Where(w => w.PrivilegeImageID == id).FirstOrDefault();
            if (img != null)
            {
               var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
               if (!string.IsNullOrEmpty(img.Url))
               {
                  var filename = img.Url.Replace("~", webRoot);
                  filename = filename.Replace("/", "\\");
                  if (System.IO.File.Exists(filename))
                  {
                     System.IO.File.Delete(filename);
                  }
               }
               this._context.PrivilegeImages.Remove(img);
               this._context.SaveChanges();
               return true;

            }
         }
         return false;
      }

      [HttpPost]
      public async Task<JsonResult> Image(Privilege model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return Json(new
            {
               responseCode = "-403",
               responseDesc = "คุณไม่มีสิทธิ์เข้าถึงระบบ",
            });
         }
         if (file != null && model.PrivilegeID > 0)
         {
            var pri = this._context.Privileges.Where(w => w.PrivilegeID == model.PrivilegeID).FirstOrDefault();
            if (pri != null)
            {
               pri.Inited = true;
               var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
               var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\" + model.PrivilegeID + "\\";
               if (!Directory.Exists(webRoot))
               {
                  Directory.CreateDirectory(webRoot);
               }
               if (file.Length > 0)
               {
                  var img = new PrivilegeImage()
                  {
                     PrivilegeID = model.PrivilegeID,
                  };
                  this._context.PrivilegeImages.Add(img);
                  this._context.SaveChanges();
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + img.PrivilegeImageID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  img.Url = filename;
                  this._context.SaveChanges();

               }
            }
         }
         return Json(new
         {
            responseCode = "200",
            responseDesc = "SUCCESS",
         });
      }

      [HttpPost]
      public async Task<IActionResult> Modify(Privilege model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }

         var dateformat = DateUtil.ToInternalDateTime(DateUtil.Now());
         if (ModelState.IsValid)
         {
            model.StartDate = DateUtil.ToDate(model.sDate);
            model.EndDate = DateUtil.ToDate(model.eDate);
            model.Update_On = DateUtil.Now();
            model.Update_By = this.HttpContext.User.Identity.Name;
            if (model.PrivilegeID <= 0)
            {
               model.Create_On = DateUtil.Now();
               model.Create_By = this.HttpContext.User.Identity.Name;
               if (model.CustomerClassList != null)
               {
                  foreach (var item in model.CustomerClassList.Where(w => w.CustomerClassID > 0))
                  {
                     model.PrivilegeCustomerClasses.Add(item);
                  }
               }
               model.Index = 1;
               var i = 1;
               foreach (var item in this._context.Privileges.OrderBy(o => o.Index))
               {
                  i++;
                  item.Index = i;
               }
               this._context.Privileges.Add(model);
               this._context.SaveChanges();
               if (file != null)
               {
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.MerchantID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.ImgUrl = filename;
                  this._context.SaveChanges();
               }
               if (!string.IsNullOrEmpty(model.tab))
                  return RedirectToAction("Update", new { id = model.PrivilegeID, tab = model.tab });
               else
                  return RedirectToAction("Index");
            }
            else
            {

               if (file != null)
               {
                  if (!string.IsNullOrEmpty(model.ImgUrl))
                  {
                     var mwebRoot = Directory.GetCurrentDirectory() + "\\wwwroot";
                     var mfilename = model.ImgUrl.Replace("~", mwebRoot);
                     mfilename = mfilename.Replace("/", "\\");
                     if (System.IO.File.Exists(mfilename))
                     {
                        System.IO.File.Delete(mfilename);
                     }
                  }
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\";
                  string extension = Path.GetExtension(file.FileName);
                  var filename = webRoot + model.MerchantID + dateformat + extension;
                  using (var fileStream = new FileStream(Path.Combine(webRoot, filename), FileMode.Create))
                  {
                     await file.CopyToAsync(fileStream);
                  }
                  filename = filename.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                  filename = filename.Replace("\\", "/");
                  model.ImgUrl = filename;
               }

               this._context.Update(model);
               /*Customer Class*/
               IEnumerable<int> itemIDs = Enumerable.Empty<int>();
               if (model.CustomerClassList != null)
               {
                  itemIDs = model.CustomerClassList.Where(w => w.ID > 0 & w.CustomerClassID > 0).Select(s => s.ID);
               }
               foreach (var item in this._context.PrivilegeCustomerClasses.Where(w => w.PrivilegeID == model.PrivilegeID))
               {
                  if (!itemIDs.Contains(item.ID))
                  {
                     this._context.PrivilegeCustomerClasses.Remove(item);
                     var tmpitem = model.PrivilegeCustomerClasses.Where(w => w.ID == item.ID).FirstOrDefault();
                     if (tmpitem != null)
                        model.PrivilegeCustomerClasses.Remove(tmpitem);
                  }
               }

               foreach (var item in model.CustomerClassList)
               {
                  item.PrivilegeID = model.PrivilegeID;
                  if (item.ID <= 0)
                     this._context.PrivilegeCustomerClasses.Add(item);

               }
               ///*Privilege Code*/

               //if (model.PrivilegeCodeList != null)
               //{
               //   itemIDs = model.PrivilegeCodeList.Where(w => w.ID > 0).Select(s => s.ID);
               //}
               //foreach (var item in this._context.PrivilegeCodes.Where(w => w.PrivilegeID == model.PrivilegeID))
               //{
               //   if (!itemIDs.Contains(item.ID))
               //   {
               //      this._context.PrivilegeCodes.Remove(item);
               //      var tmpitem = model.PrivilegeCodes.Where(w => w.ID == item.ID).FirstOrDefault();
               //      if (tmpitem != null)
               //         model.PrivilegeCodes.Remove(tmpitem);
               //   }
               //}
               //if (model.PrivilegeCodeList != null)
               //{
               //   foreach (var item in model.PrivilegeCodeList)
               //   {
               //      item.PrivilegeID = model.PrivilegeID;
               //      item.EffectiveDate = DateUtil.ToDate(item.effDate);
               //      item.ExpiryDate = DateUtil.ToDate(item.expDate);
               //      if (item.ID <= 0)
               //      {
               //         item.Create_On = DateUtil.Now();
               //         this._context.PrivilegeCodes.Add(item);
               //      }
               //      else
               //         this._context.Update(item);
               //   }
               //}
               if (!model.Inited)
               {
                  if (model.NextInit == PrivilegeTab.condition)
                     model.NextInit = PrivilegeTab.image;
                  else if (model.NextInit == PrivilegeTab.image)
                     model.NextInit = PrivilegeTab.code;
                  else if (model.NextInit == PrivilegeTab.code)
                     model.Inited = true;
               }
               this._context.SaveChanges();
               if (!model.Inited)
                  return RedirectToAction("Update", new { id = model.PrivilegeID, tab = model.NextInit });
               else
                  return RedirectToAction("Index");
            }
         }
         else
         {
            ModelState.AddModelError("Error", "มีข้อผิดพลาดไม่สามารถบันทึกข้อมูล");
         }
         ViewBag.ListCustomerClass = this._context.CustomerClasses;
         ViewBag.ListMerchant = this._context.Merchants.OrderBy(o => o.MerchantName);
         ViewBag.ListType = this._context.MerchantCategories.Where(w => w.Status == StatusType.Active).OrderBy(o => o.Index);
         if (model.PrivilegeID > 0)
            ViewBag.TotalQuantity = this._context.Redeems.Where(w => w.PrivilegeID == model.PrivilegeID).Count();
         else
            ViewBag.TotalQuantity = 0;
         return View("PrivilegeInfo", model);

      }

      public IActionResult Delete()
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         string idParam = this.RouteData.Values["id"].ToString();
         Privilege model = null;
         if (idParam != null && idParam != string.Empty)
         {
            int recordId = Int32.Parse(idParam);
            model = this._context.Privileges.Include(i => i.PrivilegeCodes).Include(i => i.PrivilegeCustomerClasses).Include(i => i.PrivilegeImages).Where(a => a.PrivilegeID == recordId).FirstOrDefault();
            if (model == null)
            {
               ModelState.AddModelError("Error", "ไม่พบข้อมูล");
            }
            else
            {
               if (model.PrivilegeImages.Count() > 0)
               {
                  this._context.PrivilegeImages.RemoveRange(model.PrivilegeImages);
                  var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\" + model.PrivilegeID + "\\";
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
               if (model.PrivilegeCustomerClasses.Count() > 0)
               {
                  this._context.PrivilegeCustomerClasses.RemoveRange(model.PrivilegeCustomerClasses);
               }
               if (model.PrivilegeCodes.Count() > 0)
               {
                  this._context.PrivilegeCodes.RemoveRange(model.PrivilegeCodes);
               }
               this._context.Privileges.Remove(model);
               this._context.SaveChanges();
               var i = 1;
               foreach (var item in this._context.Privileges.OrderBy(o => o.Index))
               {
                  item.Index = i;
                  i++;
               }
               this._context.SaveChanges();
            }
         }
         return RedirectToAction("Index");
      }

      [HttpPost]
      public async Task<JsonResult> ValidateCode(int? PrivilegeID, IFormFile excelfile)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return Json(new
            {
               responseCode = "-403",
               responseDesc = "คุณไม่มีสิทธิ์เข้าถึงระบบ",
            });
         }
         if (excelfile == null)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = "ไม่พบข้อมูลไฟล์นำเข้า",
            });
         }
         if (!PrivilegeID.HasValue || PrivilegeID == 0)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = "ไม่พบข้อมูลสิทธิพิเศษ",
            });
         }
         var pID = PrivilegeID;
         var pri = this._context.Privileges.Where(w => w.PrivilegeID == PrivilegeID).FirstOrDefault();
         if (pri == null)
         {
            return Json(new
            {
               responseCode = "-1",
               responseDesc = "ไม่พบข้อมูลสิทธิพิเศษ",
            });
         }

         var codelist = new List<PrivilegeCode>();
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

                        var cformat = worksheet.Cells["C" + row].Style.Numberformat.Format;
                        var eformat = worksheet.Cells["D" + row].Style.Numberformat.Format;
                        worksheet.Cells["C" + row].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        worksheet.Cells["D" + row].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

                        var code = worksheet.Cells["A" + row].Value != null ? worksheet.Cells["A" + row].Value.ToString() : "";
                        var maxuse = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Value.ToString() : "";
                        var eff = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Value : null;
                        var exp = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Value : null;
                        var status = worksheet.Cells["E" + row].Value != null ? worksheet.Cells["E" + row].Value.ToString() : "";


                        var pcode = new PrivilegeCode();
                        pcode.PrivilegeID = pri.PrivilegeID;
                        pcode.Code = code;
                        pcode.MaxUse = NumUtil.ParseInteger(maxuse);
                        if (pcode.MaxUse == 0)
                           pcode.MaxUse = null;

                        if (!string.IsNullOrEmpty(cformat) && cformat[0].ToString().ToLower() == "m")
                        {
                           pcode.EffectiveDate = (DateTime)eff;
                        }
                        else
                        {
                           pcode.EffectiveDate = DateUtil.ToDate(eff.ToString());
                        }

                        if (!string.IsNullOrEmpty(eformat) && eformat[0].ToString().ToLower() == "m")
                        {
                           pcode.ExpiryDate = (DateTime)exp;
                        }
                        else
                        {
                           pcode.ExpiryDate = DateUtil.ToDate(exp.ToString());
                        }

                        if (eff != null)
                        {
                           if (pcode.EffectiveDate == null)
                              pcode.EffectiveDate = DateTime.FromOADate((double)NumUtil.ParseDecimal(eff));
                        }
                        if (exp != null)
                        {
                           if (pcode.ExpiryDate == null)
                              pcode.ExpiryDate = DateTime.FromOADate((double)NumUtil.ParseDecimal(exp));
                        }
                        pcode.effDate = DateUtil.ToDisplayDate(pcode.EffectiveDate);
                        pcode.expDate = DateUtil.ToDisplayDate(pcode.ExpiryDate);
                        pcode.Status = StatusType.InActive;
                        if (status == "Y")
                           pcode.Status = StatusType.Active;
                        codelist.Add(pcode);

                     }
                     catch (Exception ex)
                     {

                     }

                  }
               }
            }
         }


         return Json(new
         {
            responseCode = "1",
            responseDesc = "SUCCESS",
            data = codelist,
         });
      }


      [HttpGet]
      public IActionResult PrivilegeCode(PrivilegeCodeValidate model)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         model.PrivilegeCodes = new List<PrivilegeCode>();
         return View("PrivilegeCode", model);
      }

      [HttpPost]
      public async Task<IActionResult> ModifyPrivilegeCode(PrivilegeCodeValidate model, IFormFile file)
      {
         if (!_loginServices.isInAdminRoles(this.GetRoles()))
         {
            return RedirectToAction("Login", "Accounts");
         }
         var pri = this._context.Privileges.Where(w => w.PrivilegeID == model.PrivilegeID).FirstOrDefault();
         if (pri == null)
         {
            ModelState.AddModelError("Error", "ไม่พบข้อมูลสิทธิพิเศษ");
         }
         if (ModelState.IsValid)
         {
            if (model.valid)
            {
               if (model.PrivilegeCodes != null && model.PrivilegeCodes.Count() > 0)
               {
                  foreach (var item in model.PrivilegeCodes)
                  {
                     item.PrivilegeID = model.PrivilegeID;
                     item.EffectiveDate = DateUtil.ToDate(item.effDate);
                     item.ExpiryDate = DateUtil.ToDate(item.expDate);
                     item.Status = item.Status;
                     item.MaxUse = item.MaxUse;
                     item.Create_On = DateUtil.Now();
                     this._context.PrivilegeCodes.Add(item);
                  }
                  this._context.SaveChanges();
                  return RedirectToAction("PrivilegeCode", new { result = 1, PrivilegeID = model.PrivilegeID });
               }
            }
            else
            {
               if (file == null)
                  ModelState.AddModelError("Error", "ไม่พบข้อมูลไฟล์นำเข้า");
               else
               {

                  model.PrivilegeCodes = new List<PrivilegeCode>();
                  model.PrivilegeCodeFails = new List<PrivilegeCodeFail>();

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
                              try
                              {
                                 var cformat = worksheet.Cells["C" + row].Style.Numberformat.Format;
                                 var eformat = worksheet.Cells["D" + row].Style.Numberformat.Format;
                                 worksheet.Cells["C" + row].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                                 worksheet.Cells["D" + row].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

                                 var code = worksheet.Cells["A" + row].Value != null ? worksheet.Cells["A" + row].Value.ToString() : "";
                                 var maxuse = worksheet.Cells["B" + row].Value != null ? worksheet.Cells["B" + row].Value.ToString() : "";
                                 var eff = worksheet.Cells["C" + row].Value != null ? worksheet.Cells["C" + row].Value : null;
                                 var exp = worksheet.Cells["D" + row].Value != null ? worksheet.Cells["D" + row].Value : null;
                                 var status = worksheet.Cells["E" + row].Value != null ? worksheet.Cells["E" + row].Value.ToString() : "";

                                 var msg = new StringBuilder();
                                 if (string.IsNullOrEmpty(code))
                                    msg.AppendLine("ไม่พบข้อมูล Code");
                                 else
                                 {
                                    var pricode = this._context.PrivilegeCodes.Where(w => w.Code == code).FirstOrDefault();
                                    if (pricode != null)
                                       msg.AppendLine("Code ซ้ำในระบบ");
                                 }
                                 var pcode = new PrivilegeCode();
                                 pcode.PrivilegeID = pri.PrivilegeID;
                                 pcode.Code = code;
                                 pcode.MaxUse = NumUtil.ParseInteger(maxuse);
                                 if (pcode.MaxUse == 0)
                                    pcode.MaxUse = 1;

                                 if (!string.IsNullOrEmpty(cformat) && cformat[0].ToString().ToLower() == "m")
                                    pcode.EffectiveDate = (DateTime)eff;
                                 else
                                    pcode.EffectiveDate = DateUtil.ToDate(eff.ToString());

                                 if (!string.IsNullOrEmpty(eformat) && eformat[0].ToString().ToLower() == "m")
                                    pcode.ExpiryDate = (DateTime)exp;
                                 else
                                    pcode.ExpiryDate = DateUtil.ToDate(exp.ToString());

                                 if (eff != null)
                                 {
                                    if (pcode.EffectiveDate == null)
                                       pcode.EffectiveDate = DateTime.FromOADate((double)NumUtil.ParseDecimal(eff));
                                 }
                                 if (exp != null)
                                 {
                                    if (pcode.ExpiryDate == null)
                                       pcode.ExpiryDate = DateTime.FromOADate((double)NumUtil.ParseDecimal(exp));
                                 }
                                 pcode.effDate = DateUtil.ToDisplayDate(pcode.EffectiveDate);
                                 pcode.expDate = DateUtil.ToDisplayDate(pcode.ExpiryDate);
                                 pcode.Status = StatusType.InActive;
                                 if (status == "Y")
                                    pcode.Status = StatusType.Active;
                                 if (string.IsNullOrEmpty(msg.ToString()))
                                 {
                                    model.PrivilegeCodes.Add(pcode);
                                 }
                                 else
                                 {
                                    model.PrivilegeCodes.Clear();
                                    var fail = new PrivilegeCodeFail();
                                    fail.Code = code;
                                    fail.EffectiveDate = pcode.effDate;
                                    fail.ExpiryDate = pcode.expDate;
                                    fail.MaxUse = pcode.MaxUse;
                                    fail.message = msg.ToString();
                                    fail.Status = pcode.Status.toStatusName();
                                    fail.row = row;
                                    model.PrivilegeCodeFails.Add(fail);
                                 }
                              }
                              catch (Exception ex)
                              {
                                 ModelState.AddModelError("Error", ex.Message);
                              }

                           }

                        }
                        else
                           ModelState.AddModelError("Error", "รูปแบบไฟล์ไม่ถูกต้องหรือไม่มีข้อมูล");
                     }
                  }
               }
            }
         }

         return View("PrivilegeCode", model);
      }

      //public void DeletePrivilegeCode()
      //{
      //   if (!_loginServices.isInAdminRoles(this.GetRoles()))
      //   {
      //      return RedirectToAction("Login", "Accounts");
      //   }
      //   string idParam = this.RouteData.Values["id"].ToString();
      //   Privilege model = null;
      //   if (idParam != null && idParam != string.Empty)
      //   {
      //   }
      //}



      [HttpGet]
      public async Task<IActionResult> TemplateCode()
      {
         var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\";
         var filename = webRoot + "privilege_code_template.xlsx";

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

      #endregion

      #region Redeem
      [HttpGet]
      public IActionResult Redeem(ReportDTO model)
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

         model.Redeems = this._context.Redeems
            .Include(i => i.Customer)
            .Include(i => i.Customer.User)
            .Include(i => i.Customer.CustomerClass)
            .Include(i => i.Privilege)
            .Include(i => i.Privilege.Merchant);

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
         var merchant = _context.Merchants.Where(w => w.UserID == model.search_privilege).FirstOrDefault();
         if (merchant != null)
            ViewBag.MerchantName = merchant.MerchantName;
         return View("Redeem", model);
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