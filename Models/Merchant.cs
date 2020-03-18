using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class Merchant
   {
      [Key]
      public int MerchantID { get; set; }

      public string MerchantName { get; set; }
      public string Url { get; set; }
      public string Youtube { get; set; }
      public StatusType Status { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public int? CategoryID { get; set; }
      public int? ProvinceID { get; set; }
      public int? UserID { get; set; }

      [NotMapped]
      [Required]
      public string UserName { get; set; }

      [NotMapped]
      [DataType(DataType.Password)]
      //[RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)[A-Za-z\\d]{8,12}$", ErrorMessage = "รหัสผ่านต้องประกอบด้วยตัวเลข ตัวอักษรใหญ่ และตัวอักษรเล็ก")]
      [StringLength(12, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
      public string Password { get; set; }



      public virtual User User { get; set; }
      public virtual MerchantCategory MerchantCategories { get; set; }
      public virtual ICollection<Privilege> Privileges { get; set; }

   }
}
