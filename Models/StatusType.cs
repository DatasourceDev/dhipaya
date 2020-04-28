using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public enum StatusType
   {
      InActive = 0,
      Active = 1,
   }

   public enum UserStatusType
   {
      InActive = 0,
      Active = 1,
      BlockAward = 2,
      BlockReward = 3
   }

   public enum UserLevelType
   {
      Member = 0,
      Admin = 1,
   }
   //public enum CustomerType
   //{
   //  Silver = 0,
   //  Gold = 1,
   //  GoldLady = 2
   //}
   public enum CustomerChanal
   {
      TIP = 0,
      Mobile = 1,
      MobileImport = 2,
      ShareHolderImport = 3,
      AmazonImport = 4,
      INTIntersect = 5,
      DhiMemberImport = 6,
      TipInsure = 7,
      TipInsureImport = 8
   }
   public enum ChannelType
   {
      IIA = 0,
      Online = 1,
      Other = 99
   }

   public enum MediaType
   {
      Image = 0,
      Video = 1,
   }
   public enum PointType
   {
      Fix = 0,
      Percentage = 1,
      Tier = 2,
      Calculate = 3,
   }
   public enum Period
   {
      None = 0,
      Once = 1,
      Day = 2,
      Week = 3,
      Month = 4,
   }

   public enum LimitType
   {
      PerMerchant = 0,
      PerPerson = 1
   }

   public enum NumberType
   {
      Amount = 0,
      Percent = 1
   }
   public enum TransacionTypeID
   {
      Register = 1,
      Update = 2,
      BuyInsure = 3,
      ClaimInsure = 4,
      InviteFriend = 5,
      ShareFacebook = 6,
      Other = 7,
      CarInspection = 8,
      AddPolicy = 9,
      Renew = 10,
      Repay = 11,
      Paybill = 12,
      DOB = 13,
      Login  = 14,
      None = 0
   }

   public static class OutletCode
   {
      public static string TipInsureWeb = "151";
      public static string MobileApplication = "888";
      public static string Other = "0";

   }
   public static class OutletCodeName
   {
      public static string TipInsureWeb = "Website Tip Insure";
      public static string MobileApplication = "Moblie Application";
      public static string Other = "อื่นๆ";

   }
   public static class TransacionTypeCode
   {
      public static string Register = "RIGISTER";
      public static string Update = "UPDATE";
      public static string BuyInsure = "BUYINSURE";
      public static string ClaimInsure = "CLAIMINSURE";
      public static string InviteFriend = "INVITEFRIEND";
      public static string ShareFacebook = "FACEBOOK";
      public static string CarInspection = "CarInspection";
      public static string Renew = "Renew";
      public static string Repay = "Repay";
      public static string Paybill = "Paybill";
      public static string AddPolicy = "AddPolicy";
      public static string Other = "OTHER";
      public static string DOB = "DOB";
      public static string Login = "LOGIN";
   }

   public static class MBSource
   {
      public static string imobile_register = "imobile_register";
      public static string imobile_purchase = "imobile_purchase";
      public static string paybill = "paybill";
      public static string repay = "repay";
      public static string carinspection = "carinspection";
      public static string renew = "renew";
      public static string add_policy = "add_policy";

      public static string tipsociety_register = "tipsociety_register";

   }

   public static class RoleName
   {
      public static string Admin = "ผู้ดูแลระบบ";
      public static string Staff = "ผู้ใช้งาน";
      public static string Member = "สมาชิก";
      public static string Merchant = "ร้านค้า";

   }

   public static class PageCode
   {
      public static string Category = "Category";
      public static string Privilege = "Privilege";
      public static string Merchant = "Merchant";
      public static string Product = "Product";
      public static string Limit = "Limit";
      public static string PointAdjust = "PointAdjust";
      public static string Condition = "Condition";
      public static string Customer = "Customer";
      public static string UserRole = "UserRole";
      public static string User = "User";
      public static string Report = "Report";
      public static string Banner = "Banner";
      public static string NewsActivityGroup = "NewsActivityGroup";
      public static string NewsActivity = "NewsActivity";
      public static string AboutUs = "AboutUs";
      public static string Question = "Question";
      public static string QuestionGroup = "QuestionGroup";
      public static string CustomerClass = "CustomerClass";
      public static string Configuration = "Configuration";
      public static string Gallery = "Gallery";

   }

   public enum PrivilegeCodeType
   {
      Random = 0,
      Custom = 1,
   }
   public static class PrivilegeTab
   {
      public static string privilege = "privilege";
      public static string condition = "condition";
      public static string code = "code";
      public static string image = "image";
   }
   public enum RedeemType
   {
      Redeem = 0,
      Delivery = 1,
   }
   public static class ConfigurationeCode
   {
      public static string SentMailInviteFriend = "SentMailInviteFriend";
   }
  
}
