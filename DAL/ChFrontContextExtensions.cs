using System;
using System.Collections.Generic;
using System.Linq;
using Dhipaya.Models;
using Dhipaya.Services;
using Microsoft.EntityFrameworkCore;
using Dhipaya.Extensions;
using System.IO;
using Dhipaya.DTO;
using BCrypt.Net;

namespace Dhipaya.DAL
{
   public static class ChFrontContextExtensions
   {


      public static void EnsureSeedData(this ChFrontContext context)
      {
         SeedMasterData(context);
      }

      public static void SeedMasterData(ChFrontContext context)
      {
         if (context.Configurations != null && !context.Configurations.Any())
         {
            var ListConfiguration = new List<Configuration>
                {
                    new Configuration { Name = ConfigurationeCode.SentMailInviteFriend ,Value = "1" },
                };
            ListConfiguration.ForEach(s => context.Configurations.Add(s));
            context.SaveChanges();
         }
         if (context.UserRoles != null && !context.UserRoles.Any())
         {
            var ListUserRole = new List<UserRole>
                {
                    new UserRole { RoleName ="ผู้ดูแลระบบ" ,UnEditable = true },
                    new UserRole { RoleName ="สมาชิก",UnEditable = true },
                    new UserRole { RoleName ="ร้านค้า" ,UnEditable = true},
                };
            ListUserRole.ForEach(s => context.UserRoles.Add(s));
            context.SaveChanges();
         }
         if (context.Pages != null && !context.Pages.Any())
         {
            var ListPage = new List<Page>
                {
                    new Page {PageCode  =PageCode.Category , PageName = "กลุ่มสิทธิพิเศษ" },
                    new Page {PageCode  =PageCode.Merchant , PageName = "ร้านค้า/บริการ" },
                    new Page {PageCode  =PageCode.Privilege , PageName = "สิทธิพิเศษ" },
                    new Page {PageCode  =PageCode.Product , PageName = "ประกันภัย" },
                    new Page {PageCode  =PageCode.Limit , PageName = "การจำกัดคะแนน" },
                    new Page {PageCode  =PageCode.Condition , PageName = "เงื่อนไขการสะสมคะแนน" },
                    new Page {PageCode  =PageCode.Customer , PageName = "สมาชิก" },
                    new Page {PageCode  =PageCode.PointAdjust , PageName = "ปรังปรุงคะแนน" },
                    new Page {PageCode  =PageCode.UserRole , PageName = "สิทธิ์การเข้าถึง" },
                    new Page {PageCode  =PageCode.User , PageName = "ผู้ใช้งาน" },
                    new Page {PageCode  =PageCode.Report , PageName = "รายงาน" },
                    new Page {PageCode  =PageCode.Banner , PageName = "แบนเนอร์" },
                    new Page {PageCode  =PageCode.NewsActivityGroup , PageName = "กลุ่มข่าวสารและกิจกรรม" },
                    new Page {PageCode  =PageCode.NewsActivity , PageName = "ข่าวสารและกิจกรรม" },
                    new Page {PageCode  =PageCode.AboutUs , PageName = "เกี่ยวกับเรา" },
                    new Page {PageCode  =PageCode.Question , PageName = "กลุ่มคำถามที่พบบ่อย" },
                    new Page {PageCode  =PageCode.QuestionGroup , PageName = "คำถามที่พบบ่อย" },
                };
            ListPage.ForEach(s => context.Pages.Add(s));
            context.SaveChanges();
         }

         if (context.Pages != null && context.Pages.Count() == 17)
         {
            var ListPage = new List<Page>{
             new Page {PageCode  =PageCode.CustomerClass , PageName = "ประเภทสมาชิก" }};
            ListPage.ForEach(s => context.Pages.Add(s));
            context.SaveChanges();
         }
         if (context.Pages != null && context.Pages.Count() == 18)
         {
            var ListPage = new List<Page>{
             new Page {PageCode  =PageCode.Configuration , PageName = "กำหนดค่า" }};
            ListPage.ForEach(s => context.Pages.Add(s));
            context.SaveChanges();
         }
         if (context.Pages != null && context.Pages.Count() == 19)
         {
            var ListPage = new List<Page>{
             new Page {PageCode  =PageCode.Gallery , PageName = "รูปภาพ" }};
            ListPage.ForEach(s => context.Pages.Add(s));
            context.SaveChanges();
         }

         if (context.PointTransacionTypes != null && !context.PointTransacionTypes.Any())
         {
            var ListPointTransacionType = new List<PointTransacionType>{
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Register , Name = "สมัครสมาชิก", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Update , Name = "อัพเดตข้อมูล", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.BuyInsure, Name = "ซื้อประกันภัย", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.ClaimInsure, Name = "เคลมประกันภัย", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.InviteFriend, Name = "Invite friend", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.ShareFacebook, Name = "แชร์เฟสบุ๊ค", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Other, Name = "อื่นๆ", Locked  =false },
        };
            ListPointTransacionType.ForEach(s => context.PointTransacionTypes.Add(s));
            context.SaveChanges();
         }
         if (context.CustomerClasses != null && !context.CustomerClasses.Any())
         {
            var ListCustomerClass = new List<CustomerClass>{
            new CustomerClass { Name = "TIP Silver", Prefix  ="S" ,ProjectCode = "Silver", ProjectName = "Silver", Status = StatusType.Active, UnEditable = true , Create_By = "Init", Create_On = DateUtil.Now(), Update_By = "Init", Update_On = DateUtil.Now()},
            new CustomerClass { Name = "TIP Gold", Prefix  ="G" ,ProjectCode = "Gold", ProjectName = "Gold", Status = StatusType.Active, UnEditable = true , Create_By = "Init", Create_On = DateUtil.Now(), Update_By = "Init", Update_On = DateUtil.Now()}
            };
            ListCustomerClass.ForEach(s => context.CustomerClasses.Add(s));
            context.SaveChanges();
         }
         if (context.PointTransacionTypes != null && context.PointTransacionTypes.Count() == 7)
         {
            var ListPointTransacionType = new List<PointTransacionType>{
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.CarInspection, Name = "ตรวจสภาพรถยนต์", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.AddPolicy, Name = "ผูกกรมธรรม์", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Renew, Name = "ต่ออายุกรมธรรม์", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Repay, Name = "ชำระเงินผ่านช่องทาง Repay", Locked  =true },
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Paybill, Name = "ชำระเงินผ่านช่องทาง Paybill", Locked  =true }};
            ListPointTransacionType.ForEach(s => context.PointTransacionTypes.Add(s));
            context.SaveChanges();
         }
         if (context.PointTransacionTypes != null && context.PointTransacionTypes.Count() == 12)
         {
            var ListPointTransacionType = new List<PointTransacionType>{
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.DOB, Name = "วันเกิด", Locked  =true }};
            ListPointTransacionType.ForEach(s => context.PointTransacionTypes.Add(s));
            context.SaveChanges();
         }
         if (context.PointTransacionTypes != null && context.PointTransacionTypes.Count() == 13)
         {
            var ListPointTransacionType = new List<PointTransacionType>{
            new PointTransacionType {TransacionTypeCode  =TransacionTypeCode.Login, Name = "เข้าสู่ระบบ", Locked  =true }};
            ListPointTransacionType.ForEach(s => context.PointTransacionTypes.Add(s));
            context.SaveChanges();
         }
         if (context.CustomerPrefixs != null && !context.CustomerPrefixs.Any())
         {
            var ListPrefix = new List<CustomerPrefix>
                {
                    new CustomerPrefix { Name="นาย", NameEng = "Mr." },
                    new CustomerPrefix { Name="นางสาว",  NameEng = "Ms."},
                    new CustomerPrefix { Name="นาง",  NameEng = "Mrs."},
                    new CustomerPrefix { Name="เด็กหญิง"},
                    new CustomerPrefix { Name="เด็กชาย"},
            };
            ListPrefix.ForEach(s => context.CustomerPrefixs.Add(s));
            context.SaveChanges();
            ListPrefix = new List<CustomerPrefix>
                {
                    new CustomerPrefix { Name="หม่อมหลวง"},
                    new CustomerPrefix { Name="หม่อมราชวงศ์"},
                    new CustomerPrefix { Name="หม่อมเจ้า"},
                    new CustomerPrefix { Name="ศาสตราจารย์เกียรติคุณ"},
                    new CustomerPrefix { Name="ศาสตราจารย์"},
                    new CustomerPrefix { Name="ผู้ช่วยศาสตราจารย์"},
                    new CustomerPrefix { Name="รองศาสตราจารย์"},
                    new CustomerPrefix { Name="คุณหญิง"},
                    new CustomerPrefix { Name="ดร."},
                    new CustomerPrefix { Name="พลเอก"},
                    new CustomerPrefix { Name="พลโท"},
                    new CustomerPrefix { Name="พลตรี"},
                    new CustomerPrefix { Name="พันเอก"},
                    new CustomerPrefix { Name="พันโท"},
                    new CustomerPrefix { Name="พันตรี"},
                    new CustomerPrefix { Name="ร้อยเอก"},
                    new CustomerPrefix { Name="ร้อยโท"},
                    new CustomerPrefix { Name="ร้อยตรี"},
                    new CustomerPrefix { Name="จ่าสิบเอก"},
                    new CustomerPrefix { Name="จ่าสิบโท"},
                    new CustomerPrefix { Name="จ่าสิบตรี"},
                    new CustomerPrefix { Name="สิบเอก"},
                    new CustomerPrefix { Name="สิบโท"},
                    new CustomerPrefix { Name="สิบตรี"},
                    new CustomerPrefix { Name="พลทหาร"},
                    new CustomerPrefix { Name="พันเอกพิเศษ"},
                    new CustomerPrefix { Name="พันเอกพิเศษหญิง"},
                    new CustomerPrefix { Name="พันเอกหญิง"},
                    new CustomerPrefix { Name="ร้อยเอกหญิง"},
                    new CustomerPrefix { Name="ร้อยโทหญิง"},
                    new CustomerPrefix { Name="ร้อยตรีหญิง"},
                    new CustomerPrefix { Name="จ่าสิบเอกหญิง"},
                    new CustomerPrefix { Name="จ่าสิบโทหญิง"},
                    new CustomerPrefix { Name="จ่าสิบตรีหญิง"},
                    new CustomerPrefix { Name="สิบเอกหญิง"},
                    new CustomerPrefix { Name="สิบโทหญิง"},
                    new CustomerPrefix { Name="สิบตรีหญิง"},
                    new CustomerPrefix { Name="พลเรือเอก"},
                    new CustomerPrefix { Name="พลเรือโท"},
                    new CustomerPrefix { Name="พลเรือตรี"},
                    new CustomerPrefix { Name="นาวาเอก"},
                    new CustomerPrefix { Name="นาวาโท"},
                    new CustomerPrefix { Name="นาวาตรี"},
                    new CustomerPrefix { Name="เรือเอก"},
                    new CustomerPrefix { Name="เรือโท"},
                    new CustomerPrefix { Name="เรือตรี"},
                    new CustomerPrefix { Name="พันจ่าเอก"},
                    new CustomerPrefix { Name="พันจ่าโท"},
                    new CustomerPrefix { Name="พันจ่าตรี"},
                    new CustomerPrefix { Name="จ่าเอก"},
                    new CustomerPrefix { Name="จ่าโท"},
                    new CustomerPrefix { Name="จ่าตรี"},
                    new CustomerPrefix { Name="พลเรือโทหญิง"},
                    new CustomerPrefix { Name="นาวาเอกพิเศษ"},
                    new CustomerPrefix { Name="นาวาเอกพิเศษหญิง"},
                    new CustomerPrefix { Name="นาวาเอกหญิง"},
                    new CustomerPrefix { Name="นาวาโทหญิง"},
                    new CustomerPrefix { Name="นาวาตรีหญิง"},
                    new CustomerPrefix { Name="พลอากาศเอก"},
                    new CustomerPrefix { Name="พลอากาศโท"},
                    new CustomerPrefix { Name="พลอากาศตรี"},
                    new CustomerPrefix { Name="นาวาอากาศเอก"},
                    new CustomerPrefix { Name="นาวาอากาศโท"},
                    new CustomerPrefix { Name="นาวาอากาศตรี"},
                    new CustomerPrefix { Name="เรืออากาศเอก"},
                    new CustomerPrefix { Name="เรืออากาศโท"},
                    new CustomerPrefix { Name="เรืออากาศตรี"},
                    new CustomerPrefix { Name="พันจ่าอากาศเอก"},
                    new CustomerPrefix { Name="พันจ่าอากาศโท"},
                    new CustomerPrefix { Name="พันจ่าอากาศตรี"},
                    new CustomerPrefix { Name="จ่าอากาศเอก"},
                    new CustomerPrefix { Name="จ่าอากาศโท"},
                    new CustomerPrefix { Name="จ่าอากาศตรี"},
                    new CustomerPrefix { Name="พลอากาศเอกหญิง"},
                    new CustomerPrefix { Name="พลอากาศโทหญิง"},
                    new CustomerPrefix { Name="พลอากาศตรีหญิง"},
                    new CustomerPrefix { Name="นาวาอากาศเอกพิเศษ"},
                    new CustomerPrefix { Name="นาวาอากาศเอกพิเศษหญิง"},
                    new CustomerPrefix { Name="นาวาอากาศโทหญิง"},
                    new CustomerPrefix { Name="นาวาอากาศตรีหญิง"},
                    new CustomerPrefix { Name="เรืออากาศเอกหญิง"},
                    new CustomerPrefix { Name="เรืออากาศโทหญิง"},
                    new CustomerPrefix { Name="เรืออากาศตรีหญิง"},
                    new CustomerPrefix { Name="พันจ่าอากาศเอกหญิง"},
                    new CustomerPrefix { Name="พันจ่าอากาศโทหญิง"},
                    new CustomerPrefix { Name="พันจ่าอากาศตรีหญิง"},
                    new CustomerPrefix { Name="จ่าอากาศเอกหญิง"},
                    new CustomerPrefix { Name="จ่าอากาศโทหญิง"},
                    new CustomerPrefix { Name="จ่าอากาศตรีหญิง"},
                    new CustomerPrefix { Name="พลตำรวจเอก"},
                    new CustomerPrefix { Name="พลตำรวจโท"},
                    new CustomerPrefix { Name="พลตำรวจตรี"},
                    new CustomerPrefix { Name="พันตำรวจเอก"},
                    new CustomerPrefix { Name="พันตำรวจโท"},
                    new CustomerPrefix { Name="พันตำรวจตรี"},
                    new CustomerPrefix { Name="ร้อยตำรวจเอก"},
                    new CustomerPrefix { Name="ร้อยตำรวจโท"},
                    new CustomerPrefix { Name="ร้อยตำรวจตรี"},
                    new CustomerPrefix { Name="นายดาบตำรวจ"},
                    new CustomerPrefix { Name="จ่าสิบตำรวจ"},
                    new CustomerPrefix { Name="สิบตำรวจเอก"},
                    new CustomerPrefix { Name="สิบตำรวจโท"},
                    new CustomerPrefix { Name="สิบตำรวจตรี"},
                    new CustomerPrefix { Name="พลตำรวจ"},
                    new CustomerPrefix { Name="ดาบตำรวจหญิง"},
                    new CustomerPrefix { Name="พลตำรวจเอกหญิง"},
                    new CustomerPrefix { Name="พลตำรวจโทหญิง"},
                    new CustomerPrefix { Name="พลตำรวจตรี"},
                    new CustomerPrefix { Name="พันตำรวจเอกพิเศษ"},
                    new CustomerPrefix { Name="พันตำรวจเอกพิเศษหญิง"},
                    new CustomerPrefix { Name="พันตำรวจโทหญิง"},
                    new CustomerPrefix { Name="พันตำรวจตรีหญิง"},
                    new CustomerPrefix { Name="ร้อยตำรวจเอกหญิง"},
                    new CustomerPrefix { Name="ร้อยตำรวจโทหญิง"},
                    new CustomerPrefix { Name="จ่าสิบตำรวจหญิง"},
                    new CustomerPrefix { Name="สิบตำรวจเอกหญิง"},
                    new CustomerPrefix { Name="สิบตำรวจโทหญิง"},
                    new CustomerPrefix { Name="สิบตำรวจตรีหญิง"},
                    new CustomerPrefix { Name="พลตำรวจหญิง"},
                    new CustomerPrefix { Name="พลตำรวจพิเศษ"},
                    new CustomerPrefix { Name="พลตำรวจพิเศษหญิง"},
                    new CustomerPrefix { Name="จ่าสิบเอกพิเศษ"},
                    new CustomerPrefix { Name="นายแพทย์"},
                    new CustomerPrefix { Name="แพทย์หญิง"},
                    new CustomerPrefix { Name="ทันตแพทย์"},
                    new CustomerPrefix { Name="ทันตแพทย์หญิง"},
                    new CustomerPrefix { Name="เภสัชกร"},
                    new CustomerPrefix { Name="เภสัชกรหญิง"},
                    new CustomerPrefix { Name="นายสัตวแพทย์"},
                    new CustomerPrefix { Name="สัตวแพทย์หญิง"},
                };
            ListPrefix.ForEach(s => context.CustomerPrefixs.Add(s));
            context.SaveChanges();

         }
         //if (context.Users != null && !context.Users.Any())
         //{
         //    var role = context.UserRoles.Where(r => r.RoleName ==RoleName.Admin).FirstOrDefault();
         //    if (role != null)
         //    {
         //        var admin = new User()
         //        {
         //            UserName ="admin",
         //            Password = DataEncryptor.Encrypt("admin123"),
         //            Email ="admintipfanclub@dhipaya.co.th",
         //            UserRoleID = role.UserRoleID,
         //        };
         //        //Customers = new List<Customer>();
         //        //admin.Customers.Add(new Customer() { NameTh ="admin", SurNameTh ="admin", Email ="admintipfanclub@dhipaya.co.th", UserLevel = UserLevelType.Admin, Status = StatusType.Active });
         //        var ListUser = new List<User>
         //        {
         //           admin
         //        };
         //        ListUser.ForEach(s => context.Users.Add(s));
         //        context.SaveChanges();
         //    }
         //}


         if (context.MerchantCategories != null && !context.MerchantCategories.Any())
         {
            var List = new List<MerchantCategory> {
                    new MerchantCategory { CategoryName ="อร่อยฟิน กินกระจาย", RedWord ="อร่อยฟิน [กิน]กระจาย",Description = "ฟินกับความอร่อยจากร้านเด่น ร้านดัง ร้านในตำนาน ที่ต้องไปลองซักครั้งในชีวิต มีให้เลือกครบทั้งคาว หวาน บุกไปกินได้อย่างทั่วถึง" , Status = StatusType.Active},
                    new MerchantCategory { CategoryName ="พักกาย สบายใจ", RedWord ="[พัก]กาย สบายใจ",Description ="สบายกาย ใจเป็นสุข กับที่พักหลากหลาย โรงแรม รีสอร์ท โฮมสเตย์ บรรยากาศดี เติมเต็มความสุข ให้ทริปพักผ่อนสมบูรณ์แบบ", Status = StatusType.Active },
                    new MerchantCategory { CategoryName ="รักรถ เข้าใจรถ" ,  RedWord ="รัก[รถ] เข้าใจรถ" ,Description ="ดูแล รักษารถ อย่างเข้าใจ จาก Car Care คุณภาพ ตกแต่งรถยอดฝีมือแห่งวงการ อุปกรณ์รักษาเครื่องยนต์ ถนอมสีรถ เพื่อรถที่คุณรัก", Status = StatusType.Active},
                    new MerchantCategory { CategoryName ="บ้านแสนสุข", RedWord ="[บ้าน]แสนสุข",  Description ="สร้างสรรค์บ้านให้น่าอยู่ สะอาด อบอุ่น ด้วยของตกแต่งบ้านตามสไตล์ที่คุณชอบ บริษัททำความสะอาด และกำจัดสัตว์ไม่พึงประสงค์ของบ้านคุณ",Status = StatusType.Active },
                    new MerchantCategory { CategoryName ="ฟิตเฟิร์ม เพิ่มพลัง",  RedWord ="[ฟิตเฟิร์ม] เพิ่มพลัง",Description ="ใส่ใจดูแลสุขภาพ ออกกำลังกาย ฟิตเฟิร์มทุกสัดส่วน กับฟิตเนส โยคะ สปา มาครบ เพื่อความแข็งแรง หุ่นดี มีสไตล์",Status = StatusType.Active },
                    new MerchantCategory { CategoryName ="จัดให้ ได้สวย" , RedWord ="จัดให้ ได้[สวย]" ,Description ="สวยได้ในชาตินี้ ไม่ต้องรอชาติหน้า สวยใสสไตล์เกาหลี ผิวเด้ง หน้าเป๊ะปัง กับสถานเสริมความงามที่มีความเชี่ยวชาญและประสบการณ์สูง ", Status = StatusType.Active},
                    new MerchantCategory { CategoryName ="เลือกที่ชอบ ตอบโจทย์ที่ใช่", RedWord ="เลือกที่ชอบ ตอบโจทย์ที่[ใช่]", Description ="ความสนุก และความสุขที่คุณเลือกได้ หลายหลายไลฟ์สไตล์ที่บอกความเป็นคุณ ชอบเที่ยว ใฝ่เรียนรู้ รักธรรมชาติ และอื่นๆ จัดให้ครบเลือกได้เยอะ",Status = StatusType.Active },
                };
            List.ForEach(s => context.MerchantCategories.Add(s));
            context.SaveChanges();
         }
         if (context.Merchants != null && !context.Merchants.Any())
         {
            var List = new List<Merchant>();
            foreach (var imp in context.PrivilegeImpts.OrderBy(o => o.MerchantName))
            {
               if (string.IsNullOrEmpty(imp.Youtube))
                  imp.Youtube = "https://www.youtube.com";

               var m = new Merchant()
               {
                  MerchantName = imp.MerchantName.Trim(),
                  Status = StatusType.Active,
                  Create_On = DateUtil.Now(),
                  Create_By = "Migrate",
                  Youtube = imp.Youtube,
               };
               var cate = context.MerchantCategories.Where(w => w.CategoryName == imp.PrivilegeType.Trim()).FirstOrDefault();
               if (cate != null)
                  m.CategoryID = cate.CategoryID;
               if (!string.IsNullOrEmpty(imp.ProvinceName))
               {
                  var province = context.Provinces.Where(w => w.ProvinceName == imp.ProvinceName.Trim()).FirstOrDefault();
                  if (province != null)
                     m.ProvinceID = province.ProvinceID;
               }

               var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Merchant\\";
               if (Directory.Exists(webRoot))
               {
                  var name = m.MerchantName.Replace("\n", " ").Replace(" ", "").ToLower();
                  var files = Directory.GetFiles(webRoot, "*.*", SearchOption.AllDirectories)
                              .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                              .Select(s => new { filepath = s, filename = s.Replace(webRoot, "").Replace("_", "&").Replace("\n", "").Replace(" ", "").Replace(".jpg", "").Replace(".gif", "").Replace(".jpeg", "").Replace(".png", "").ToLower() });

                  foreach (var file in files.Where(s => s.filename == name))
                  {
                     var filename = file.filepath.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                     filename = filename.Replace("\\", "/");
                     m.Url = filename;
                  }
               }
               List.Add(m);
            }

            List.ForEach(s => context.Merchants.Add(s));
            context.SaveChanges();
         }
         if (context.Privileges != null && !context.Privileges.Any())
         {
            var webRoot = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\Privilege\\";
            var directories = Directory.GetDirectories(webRoot, "*.*", SearchOption.AllDirectories)
                                          .Select(s => new { filepath = s, foldername = s.Replace(webRoot, "").Replace("_", "&").Replace("\n", "").Replace(" ", "").ToLower() });

            var List = new List<Privilege>();
            foreach (var imp in context.PrivilegeImpts)
            {
               if (imp.PeriodTo.HasValue && imp.PeriodTo.Value.Year > 2500)
                  imp.PeriodTo = DateUtil.ToDate(imp.PeriodTo.Value.Day, imp.PeriodTo.Value.Month, imp.PeriodTo.Value.Year - 543).Value;
               var cate = context.MerchantCategories.Where(w => w.CategoryName == imp.PrivilegeType.Trim()).FirstOrDefault();
               var m = context.Merchants.Where(w => w.MerchantName == imp.MerchantName.Trim()).FirstOrDefault();
               var pri = new Privilege()
               {
                  PrivilegeName = imp.PrivilegeName.Trim(),
                  CategoryID = cate.CategoryID,
                  MerchantID = m.MerchantID,
                  PrivilegeCondition = imp.Condition,
                  Allowable_Outlet = imp.Outlets,
                  StartDate = imp.PeriodFrom,
                  EndDate = imp.PeriodTo,
                  Status = StatusType.Active,
                  Create_On = DateUtil.Now(),
                  Create_By = "Migrate",
                  PrivilegeImages = new List<PrivilegeImage>(),
               };

               if (imp.Silver == "Y")
                  pri.Silver = true;
               if (imp.Gold == "Y")
                  pri.Gold = true;


               if (Directory.Exists(webRoot))
               {
                  var name = m.MerchantName.Replace("\n", " ").Replace(" ", "").ToLower();
                  foreach (var directory in directories.Where(s => s.foldername == name))
                  {
                     var i = 0;
                     foreach (var file in Directory.GetFiles(directory.filepath, "*.*", SearchOption.AllDirectories).Where(s => (s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".png", StringComparison.OrdinalIgnoreCase))))
                     {
                        var filename = file.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "~");
                        filename = filename.Replace("\\", "/");
                        var img = new PrivilegeImage();
                        img.Url = filename;
                        pri.PrivilegeImages.Add(img);
                     }
                  }
               }
               List.Add(pri);
            }
            List.ForEach(s => context.Privileges.Add(s));
            context.SaveChanges();
         }

         if (context.Provinces != null && !context.Provinces.Any())
         {
            var List = new List<Province> { new Province { ProvinceID = 1, ProvinceCode = "01", ProvinceName = "กระบี่" }, new Province { ProvinceID = 209, ProvinceCode = "00", ProvinceName = "กรุงเทพมหานคร" }, new Province { ProvinceID = 2, ProvinceCode = "02", ProvinceName = "กาญจนบุรี" }, new Province { ProvinceID = 3, ProvinceCode = "03", ProvinceName = "กาฬสินธุ์" }, new Province { ProvinceID = 4, ProvinceCode = "04", ProvinceName = "กำแพงเพชร" }, new Province { ProvinceID = 5, ProvinceCode = "05", ProvinceName = "ขอนแก่น" }, new Province { ProvinceID = 6, ProvinceCode = "06", ProvinceName = "จันทบุรี" }, new Province { ProvinceID = 7, ProvinceCode = "07", ProvinceName = "ฉะเชิงเทรา" }, new Province { ProvinceID = 8, ProvinceCode = "08", ProvinceName = "ชลบุรี" }, new Province { ProvinceID = 9, ProvinceCode = "09", ProvinceName = "ชัยนาท" }, new Province { ProvinceID = 10, ProvinceCode = "10", ProvinceName = "ชัยภูมิ" }, new Province { ProvinceID = 11, ProvinceCode = "11", ProvinceName = "ชุมพร" }, new Province { ProvinceID = 14, ProvinceCode = "14", ProvinceName = "ตรัง" }, new Province { ProvinceID = 15, ProvinceCode = "15", ProvinceName = "ตราด" }, new Province { ProvinceID = 16, ProvinceCode = "16", ProvinceName = "ตาก" }, new Province { ProvinceID = 17, ProvinceCode = "17", ProvinceName = "นครนายก" }, new Province { ProvinceID = 18, ProvinceCode = "18", ProvinceName = "นครปฐม" }, new Province { ProvinceID = 19, ProvinceCode = "19", ProvinceName = "นครพนม" }, new Province { ProvinceID = 20, ProvinceCode = "20", ProvinceName = "นครราชสีมา" }, new Province { ProvinceID = 21, ProvinceCode = "21", ProvinceName = "นครศรีธรรมราช" }, new Province { ProvinceID = 22, ProvinceCode = "22", ProvinceName = "นครสวรรค์" }, new Province { ProvinceID = 23, ProvinceCode = "23", ProvinceName = "นนทบุรี" }, new Province { ProvinceID = 24, ProvinceCode = "24", ProvinceName = "นราธิวาส" }, new Province { ProvinceID = 25, ProvinceCode = "25", ProvinceName = "น่าน" }, new Province { ProvinceID = 694, ProvinceCode = "76", ProvinceName = "บึงกาฬ" }, new Province { ProvinceID = 26, ProvinceCode = "26", ProvinceName = "บุรีรัมย์" }, new Province { ProvinceID = 27, ProvinceCode = "27", ProvinceName = "ปทุมธานี" }, new Province { ProvinceID = 28, ProvinceCode = "28", ProvinceName = "ประจวบคีรีขันธ์" }, new Province { ProvinceID = 29, ProvinceCode = "29", ProvinceName = "ปราจีนบุรี" }, new Province { ProvinceID = 30, ProvinceCode = "30", ProvinceName = "ปัตตานี" }, new Province { ProvinceID = 31, ProvinceCode = "31", ProvinceName = "พระนครศรีอยุธยา" }, new Province { ProvinceID = 71, ProvinceCode = "71", ProvinceName = "พะเยา" }, new Province { ProvinceID = 32, ProvinceCode = "32", ProvinceName = "พังงา" }, new Province { ProvinceID = 33, ProvinceCode = "33", ProvinceName = "พัทลุง" }, new Province { ProvinceID = 34, ProvinceCode = "34", ProvinceName = "พิจิตร" }, new Province { ProvinceID = 35, ProvinceCode = "35", ProvinceName = "พิษณุโลก" }, new Province { ProvinceID = 39, ProvinceCode = "39", ProvinceName = "ภูเก็ต" }, new Province { ProvinceID = 40, ProvinceCode = "40", ProvinceName = "มหาสารคาม" }, new Province { ProvinceID = 72, ProvinceCode = "72", ProvinceName = "มุกดาหาร" }, new Province { ProvinceID = 43, ProvinceCode = "43", ProvinceName = "ยะลา" }, new Province { ProvinceID = 42, ProvinceCode = "42", ProvinceName = "ยโสธร" }, new Province { ProvinceID = 45, ProvinceCode = "45", ProvinceName = "ระนอง" }, new Province { ProvinceID = 46, ProvinceCode = "46", ProvinceName = "ระยอง" }, new Province { ProvinceID = 47, ProvinceCode = "47", ProvinceName = "ราชบุรี" }, new Province { ProvinceID = 44, ProvinceCode = "44", ProvinceName = "ร้อยเอ็ด" }, new Province { ProvinceID = 48, ProvinceCode = "48", ProvinceName = "ลพบุรี" }, new Province { ProvinceID = 49, ProvinceCode = "49", ProvinceName = "ลำปาง" }, new Province { ProvinceID = 50, ProvinceCode = "50", ProvinceName = "ลำพูน" }, new Province { ProvinceID = 52, ProvinceCode = "52", ProvinceName = "ศรีสะเกษ" }, new Province { ProvinceID = 53, ProvinceCode = "53", ProvinceName = "สกลนคร" }, new Province { ProvinceID = 54, ProvinceCode = "54", ProvinceName = "สงขลา" }, new Province { ProvinceID = 55, ProvinceCode = "55", ProvinceName = "สตูล" }, new Province { ProvinceID = 56, ProvinceCode = "56", ProvinceName = "สมุทรปราการ" }, new Province { ProvinceID = 57, ProvinceCode = "57", ProvinceName = "สมุทรสงคราม" }, new Province { ProvinceID = 58, ProvinceCode = "58", ProvinceName = "สมุทรสาคร" }, new Province { ProvinceID = 59, ProvinceCode = "59", ProvinceName = "สระบุรี" }, new Province { ProvinceID = 73, ProvinceCode = "73", ProvinceName = "สระแก้ว" }, new Province { ProvinceID = 60, ProvinceCode = "60", ProvinceName = "สิงห์บุรี" }, new Province { ProvinceID = 62, ProvinceCode = "62", ProvinceName = "สุพรรณบุรี" }, new Province { ProvinceID = 63, ProvinceCode = "63", ProvinceName = "สุราษฎร์ธานี" }, new Province { ProvinceID = 64, ProvinceCode = "64", ProvinceName = "สุรินทร์" }, new Province { ProvinceID = 61, ProvinceCode = "61", ProvinceName = "สุโขทัย" }, new Province { ProvinceID = 65, ProvinceCode = "65", ProvinceName = "หนองคาย" }, new Province { ProvinceID = 75, ProvinceCode = "75", ProvinceName = "หนองบัวลำภู" }, new Province { ProvinceID = 74, ProvinceCode = "74", ProvinceName = "อำนาจเจริญ" }, new Province { ProvinceID = 67, ProvinceCode = "67", ProvinceName = "อุดรธานี" }, new Province { ProvinceID = 68, ProvinceCode = "68", ProvinceName = "อุตรดิตถ์" }, new Province { ProvinceID = 69, ProvinceCode = "69", ProvinceName = "อุทัยธานี" }, new Province { ProvinceID = 70, ProvinceCode = "70", ProvinceName = "อุบลราชธานี" }, new Province { ProvinceID = 66, ProvinceCode = "66", ProvinceName = "อ่างทอง" }, new Province { ProvinceID = 12, ProvinceCode = "12", ProvinceName = "เชียงราย" }, new Province { ProvinceID = 13, ProvinceCode = "13", ProvinceName = "เชียงใหม่" }, new Province { ProvinceID = 36, ProvinceCode = "36", ProvinceName = "เพชรบุรี" }, new Province { ProvinceID = 37, ProvinceCode = "37", ProvinceName = "เพชรบูรณ์" }, new Province { ProvinceID = 51, ProvinceCode = "51", ProvinceName = "เลย" }, new Province { ProvinceID = 38, ProvinceCode = "38", ProvinceName = "แพร่" }, new Province { ProvinceID = 41, ProvinceCode = "41", ProvinceName = "แม่ฮ่องสอน" }, };
            List.ForEach(s => context.Provinces.Add(s));
            context.SaveChanges();
         }

         if (context.Aumphurs != null && !context.Aumphurs.Any())
         {
            var List = ChFrontContextExtensions2.AumphurList();
            List.ForEach(s => context.Aumphurs.Add(s));
            context.SaveChanges();
         }
         if (context.Tumbons != null && !context.Tumbons.Any())
         {
            var List = ChFrontContextExtensions2.TumbonList();
            List.ForEach(s => context.Tumbons.Add(s));
            context.SaveChanges();
         }
      }
   }
}
