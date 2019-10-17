using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class Configuration
   {
      [Key]
      public int ID { get; set; }
      public string Name { get; set; }
      public string Value { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }
   }
}
