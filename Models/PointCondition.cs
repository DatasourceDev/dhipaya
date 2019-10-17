using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhipaya.Models
{
   public class PointLimit
   {
      [Key]
      public int ID { get; set; }

      public Period Period { get; set; }
      public int? LimitedOnce { get; set; }
      public int? LimitedDay { get; set; }
      public int? LimitedWeek { get; set; }
      public int? LimitedMonth { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

   }

   public class PointCondition
   {
      [Key]
      public int ConditionID { get; set; }

      public int TransacionTypeID { get; set; }

      public string ConditionCode { get; set; }

      public string Name { get; set; }
      public string Description { get; set; }
      public string OutletCode { get; set; }
      public PointType PointType { get; set; }
      public Period Period { get; set; }
      public int? LimitedOnce { get; set; }
      public int? LimitedDay { get; set; }
      public int? LimitedWeek { get; set; }
      public int? LimitedMonth { get; set; }

      public int? Point { get; set; }

      public decimal? Percent { get; set; }
      public decimal? CalPointPurchaseAmt { get; set; }
      public int? CalPoint { get; set; }      


      public bool Silver { get; set; }
      public bool Gold { get; set; }

      public bool IsForBirthday { get; set; }
      public bool IsAllDay { get; set; }
      public bool IsMon { get; set; }
      public bool IsTue { get; set; }
      public bool IsWed { get; set; }
      public bool IsThu { get; set; }
      public bool IsFri { get; set; }
      public bool IsSat { get; set; }
      public bool IsSun { get; set; }

      public ChannelType ChannelType { get; set; }

      public DateTime? StartDate { get; set; }

      public DateTime? EndDate { get; set; }

      public StatusType Status { get; set; }
      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      [NotMapped]
      public string sDate { get; set; }
      [NotMapped]
      public string eDate { get; set; }

      public virtual PointTransacionType PointTransacionType { get; set; }
      public virtual ICollection<PointConditionProduct> PointConditionProducts { get; set; }
      public virtual ICollection<PointConditionTier> PointConditionTiers { get; set; }
      public virtual ICollection<PointConditionCustomerClass> PointConditionCustomerClasses { get; set; }

      [NotMapped]
      public IList<PointConditionCustomerClass> CustomerClassList
      {
         get
         {
            return this.PointConditionCustomerClasses != null ? this.PointConditionCustomerClasses.ToList() : null;
         }
         set
         {
            this.PointConditionCustomerClasses = value;
         }
      }

      [NotMapped]
      public IList<PointConditionProduct> ProductList 
      {
         get
         {
            return this.PointConditionProducts != null ? this.PointConditionProducts.ToList() : null;
         }
         set
         {
            this.PointConditionProducts = value;
         }
      }
      [NotMapped]
      public IList<PointConditionTier> TierList
      {
         get
         {
            return this.PointConditionTiers != null ? this.PointConditionTiers.ToList() : null;
         }
         set
         {
            this.PointConditionTiers = value;
         }
      }

   }

   public class PointConditionCustomerClass
   {
      [Key]
      public int ID { get; set; }
      public int CustomerClassID { get; set; }
      public int ConditionID { get; set; }
   }


   public class PointTransacionType
   {
      [Key]
      public int TransacionTypeID { get; set; }

      public string TransacionTypeCode { get; set; }

      public string Name { get; set; }

      public string Description { get; set; }

      public bool Locked { get; set; }
   }

   public class PointConditionProduct
   {
      [Key]
      public int ID { get; set; }
      public int ProductID { get; set; }
      public int ConditionID { get; set; }
    //public virtual Product Product { get; set; }
    //public virtual PointCondition PointCondition { get; set; }


  }

  public class PointConditionTier
   {
      [Key]
      public int ID { get; set; }
      public int ConditionID { get; set; }
      public NumberType NumberType { get; set; }

      public decimal? PurchaseAmtFrom { get; set; }
      public decimal? PurchaseAmtTo { get; set; }
      public int? Point { get; set; }
      public decimal? Percent { get; set; }

    //public virtual PointCondition PointCondition { get; set; }

  }

}
