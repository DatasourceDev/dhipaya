using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class Privilege
   {
      [Key]
      public int PrivilegeID { get; set; }
      public string PrivilegeName { get; set; }
      public string PrivilegeDesc { get; set; }
      public string PrivilegeCondition { get; set; }
      public string Youtube { get; set; }
      public string ImgUrl { get; set; }
      public string LogoUrl { get; set; }

      public int Index { get; set; }
      public bool Inited { get; set; }
      public string NextInit { get; set; }

      public int? CreditPoint { get; set; }

      [NotMapped]
      public string tab { get; set; }

      [NotMapped]
      public string sDate { get; set; }
      [NotMapped]
      public string eDate { get; set; }

      public DateTime? StartDate { get; set; }

      [DataType(DataType.Date)]
      [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
      public DateTime? EndDate { get; set; }

      public StatusType Status { get; set; }

      public int? CategoryID { get; set; }
      public int? MerchantID { get; set; }
      public int? PrivilegeTypeID { get; set; }

      public int? Quantity { get; set; }

      public Period PerPrivilegePeriod { get; set; }
      public Period PerPersonPeriod { get; set; }

      public int? PerPrivilegeLimitedDay { get; set; }
      public int? PerPrivilegeLimitedWeek { get; set; }
      public int? PerPrivilegeLimitedMonth { get; set; }

      public int? PerPersonLimitedDay { get; set; }
      public int? PerPersonLimitedWeek { get; set; }
      public int? PerPersonLimitedMonth { get; set; }

      public string Allowable_Outlet { get; set; }
      public bool Silver { get; set; }
      public bool Gold { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public PrivilegeCodeType PrivilegeCodeType { get; set; }
      public RedeemType RedeemType { get; set; }

      public virtual Merchant Merchant { get; set; }
      public virtual MerchantCategory MerchantCategory { get; set; }
      public virtual ICollection<Redeem> Redeems { get; set; }
      public virtual ICollection<PrivilegeMemberLevel> PrivilegeMemberLevels { get; set; }
      public virtual ICollection<PrivilegeImage> PrivilegeImages { get; set; }

      public virtual ICollection<PrivilegeCustomerClass> PrivilegeCustomerClasses { get; set; }
      public IEnumerable<PrivilegeCode> PrivilegeCodes { get; set; }

      [NotMapped]
      public IList<PrivilegeCustomerClass> CustomerClassList
      {
         get
         {
            return this.PrivilegeCustomerClasses != null ? this.PrivilegeCustomerClasses.ToList() : null;
         }
         set
         {
            this.PrivilegeCustomerClasses = value;
         }
      }
     
   }

   public class PrivilegeCodeValidate
   {
      public bool valid { get; set; }
      public int result { get; set; }
      public int PrivilegeID { get; set; }
      public IList<PrivilegeCode> PrivilegeCodes { get; set; }
      public IList<PrivilegeCodeFail> PrivilegeCodeFails { get; set; }

   }
   public class PrivilegeCode
   {
      [Key]
      public int ID { get; set; }
      public int PrivilegeID { get; set; }
      public string Code { get; set; }

      public int Used { get; set; }
      public int? MaxUse { get; set; }
      public DateTime? Create_On { get; set; }
      public DateTime? EffectiveDate { get; set; }
      public DateTime? ExpiryDate { get; set; }
      public StatusType Status { get; set; }

      [NotMapped]
      public string effDate { get; set; }
      [NotMapped]
      public string expDate { get; set; }

   }
   public class PrivilegeCodeFail
   {
      public string Code { get; set; }
      public int? MaxUse { get; set; }
      public string EffectiveDate { get; set; }
      public string ExpiryDate { get; set; }
      public string message { get; set; }
      public int row { get; set; }
      public string Status { get; set; }
   }
   public class PrivilegeCustomerClass
   {
      [Key]
      public int ID { get; set; }
      public int PrivilegeID { get; set; }
      public int CustomerClassID { get; set; }
   }

 
}
