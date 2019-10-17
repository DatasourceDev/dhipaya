using Dhipaya.Extensions;
using Dhipaya.Models;
using Dhipaya.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.Models
{
   public class ContactBlock
   {
      [Key]
      public int ID { get; set; }
   
      public string Email { get; set; }

      public string Description { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
   }

}
