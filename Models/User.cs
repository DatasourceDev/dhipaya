using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhipaya.Models
{
   public class User
   {
      public int ID { get; set; }
      public int UserRoleID { get; set; }

      [Required]
      [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "เฉพาะตัวเลขและตัวอักษรภาษาอังกฤษ")]
      public string UserName { get; set; }

      public string FirstName { get; set; }

      public string LastName { get; set; }

      public string Password { get; set; }

      [NotMapped]
      [DataType(DataType.Password)]
      [Compare("ConfirmPassword", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
      public string ConfirmPassword { get; set; }

      public string PhoneNumber { get; set; }

      [Required]
      [EmailAddress(ErrorMessage = "อีเมลไม่ถูกต้อง")]
      public string Email { get; set; }

      [DefaultValue(UserStatusType.Active)]
      public UserStatusType Status { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public virtual UserRole UserRole { get; set; }

      public virtual ICollection<Customer> Customers { get; set; }
   }
}
