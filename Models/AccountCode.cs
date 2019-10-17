using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.Models
{
   public class AccountCode
   {
      [Key]
      public int ID { get; set; }

      public string Code { get; set; }
      public int? CustomerID{ get; set; }

      public StatusType Status { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
   }
}
