using Dhipaya.Extensions;
using Dhipaya.Models;
using Dhipaya.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.DTO
{
   public class ContactDTO
   {
      [Required(ErrorMessage = "กรุณาระบุชื่อ")]
      public string Name { get; set; }

      [Phone]
      [DataType(DataType.PhoneNumber)]
      [Required(ErrorMessage = "กรุณาระบุเบอร์ติดต่อ")]
      public string ContactNo { get; set; }

      [EmailAddress]
      [DataType(DataType.EmailAddress)]
      [Required(ErrorMessage = "กรุณาระบุอีเมล")]
      public string Email { get; set; }

      [Required(ErrorMessage = "กรุณาระบุหัวข้อติดต่อ")]
      public string Title { get; set; }

      [Required(ErrorMessage = "กรุณาระบุเรื่องที่ต้องการสอบถาม")]
      public string Information { get; set; }
   }

}
