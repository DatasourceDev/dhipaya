using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dhipaya.Models
{
   public class CustomerPoint
   {
      [Key]
      public int ID { get; set; }
      public int Point { get; set; }
      public string Code { get; set; }
      public string Name { get; set; }
      public CustomerChanal CustomerChanal { get; set; }
      public int? ProductID { get; set; }
      public int CustomerID { get; set; }

      public int TransacionTypeID { get; set; }

      public ChannelType ChannelType { get; set; }

      public string Package { get; set; }
      public string Source { get; set; }
      public decimal PurchaseAmt { get; set; }
      public string CustomerClassName { get; set; }

      public string ProjectCode { get; set; }
      public string ProjectName { get; set; }
      public string PolicyNo { get; set; }
      public string OrderNo { get; set; }
      public string PreviousPolicyNo { get; set; }
      public string OutletCode { get; set; }
      public string InsuranceClass { get; set; }
      public string Subclass { get; set; }
      public DateTime? EffectiveDate { get; set; }
      public DateTime? ExpiryDate { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }
      public virtual Customer Customer { get; set; }

   }

   public class MobilePoint
   {
      [Key]
      public int MobilePointID { get; set; }

      [Required(ErrorMessage = "กรุณาระบุรหัสผู้ใช้")]
      public int CustomerID { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Product")]
      public string Product { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Package")]
      public string Package { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Channel")]
      public string Channel { get; set; }
      public string PolicyNo { get; set; }
      public string OrderNo { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Source")]
      public string Source { get; set; }

      public string IDCard { get; set; }
      public string Passport { get; set; }
      public decimal Point { get; set; }
      public decimal PurchaseAmt { get; set; }



      public DateTime? Create_On { get; set; }
   }
}
