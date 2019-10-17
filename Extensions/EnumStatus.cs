using Dhipaya.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dhipaya.Extensions
{
   public static class EnumStatus
   {
      public static IDictionary<int, string> ToReverseDictionary(this StatusType statusType, bool revesrse = true)
      {
         var arrStatus = Enum.GetValues(typeof(StatusType));
         Array.Sort(arrStatus);
         if (revesrse)
         {
            Array.Reverse(arrStatus);
         }
         IDictionary<int, string> statusDict = arrStatus
            .Cast<StatusType>()
            .ToDictionary(t => (int)t, t =>
            {
               switch (t)
               {
                  case StatusType.InActive:
                     return "ระงับการใช้งาน";
                  case StatusType.Active:
                     return "ใช้งานได้";
                  default:
                     return "";
               }
            });
         return statusDict;
      }

      public static IDictionary<int, string> ToReverseDictionary(this UserStatusType statusType, bool revesrse = true)
      {
         var arrStatus = Enum.GetValues(typeof(UserStatusType));

         IDictionary<int, string> statusDict = arrStatus
            .Cast<UserStatusType>()
            .ToDictionary(t => (int)t, t =>
            {
               switch (t)
               {
                  case UserStatusType.InActive:
                     return "ระงับการใช้งาน";
                  case UserStatusType.Active:
                     return "ใช้งานได้";
                  case UserStatusType.BlockAward:
                     return "ไม่สามารถสะสมคะแนนได้";
                  case UserStatusType.BlockReward:
                     return "ไม่สามารถแลกของรางวัลได้";
                  default:
                     return "";
               }
            });
         return statusDict;
      }

      public static StatusType toStatus(this string text)
      {
         StatusType status = StatusType.Active;
         switch (text)
         {
            case "InActive":
               status = StatusType.InActive;
               break;
            case "Active":
               status = StatusType.Active;
               break;
            case "ระงับการใช้งาน":
               status = StatusType.InActive;
               break;
            case "ใช้งานได้":
               status = StatusType.Active;
               break;
            default:
               break;
         }
         return status;
      }

      public static UserStatusType toUserStatus(this string text)
      {
         UserStatusType status = UserStatusType.Active;
         switch (text)
         {
            case "InActive":
               status = UserStatusType.InActive;
               break;
            case "Active":
               status = UserStatusType.Active;
               break;
            case "BlockAward":
               status = UserStatusType.BlockAward;
               break;
            case "BlockReward":
               status = UserStatusType.BlockReward;
               break;
            case "ระงับการใช้งาน":
               status = UserStatusType.InActive;
               break;
            case "ใช้งานได้":
               status = UserStatusType.Active;
               break;
            case "ไม่สามารถสะสมคะแนนได้":
               status = UserStatusType.BlockAward;
               break;
            case "ไม่สามารถแลกของรางวัลได้":
               status = UserStatusType.BlockReward;
               break;
            default:
               break;
         }
         return status;
      }
   

      public static string toStatusName(this StatusType statusType)
      {
         string status = "";
         switch (statusType)
         {
            case StatusType.InActive:
               status = "ถูกระงับ";
               break;
            case StatusType.Active:
               status = "ใช้งานได้";
               break;
            default:
               break;
         }
         return status;
      }

      public static string toStatusName(this UserStatusType userStatusType)
      {
         string status = "";
         switch (userStatusType)
         {
            case UserStatusType.InActive:
               status = "ถูกระงับ";
               break;
            case UserStatusType.Active:
               status = "ใช้งานได้";
               break;
            case UserStatusType.BlockAward:
               return "ไม่สามารถสะสมคะแนนได้";
            case UserStatusType.BlockReward:
               return "ไม่สามารถแลกของรางวัลได้";
            default:
               break;
         }
         return status;
      }
      public static string toStatusNameEn(this UserStatusType userStatusType)
      {
         string status = "";
         switch (userStatusType)
         {
            case UserStatusType.InActive:
               status = "InActive";
               break;
            case UserStatusType.Active:
               status = "Active";
               break;
            case UserStatusType.BlockAward:
               return "BlockAward";
            case UserStatusType.BlockReward:
               return "BlockAward";
            default:
               break;
         }
         return status;
      }

      public static string toName(this CustomerChanal chanal)
      {
         string status = "";
         switch (chanal)
         {
            case CustomerChanal.TIP:
               status = "TIP Society";
               break;
            case CustomerChanal.Mobile:
               status = "Mobile Application";
               break;
            case CustomerChanal.MobileImport:
               status = "Mobile Application นำเข้าอัตโนมัติ";
               break;
            case CustomerChanal.ShareHolderImport:
               status = "ผู้ถือหุ้น นำเข้าอัตโนมัติ";
               break;
            case CustomerChanal.AmazonImport:
               status = "Cafe Amazon นำเข้าอัตโนมัติ";
               break;
            case CustomerChanal.INTIntersect:
               status = "INT Intersect";
               break;
            case CustomerChanal.DhiMemberImport:
               status = "ลูกค้ารายย่อย นำเข้าอัตโนมัติ";
               break;
            case CustomerChanal.TipInsure:
               status = "TIP Insure";
               break;
            case CustomerChanal.TipInsureImport:
               status = "TIP Insure นำเข้าอัตโนมัติ";
               break;
            default:
               break;
         }
         return status;
      }
   }
}
