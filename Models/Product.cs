using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dhipaya.Models
{
   public class Product
   {
      [Key]
      public int ProductID { get; set; }

      public string ProductCode { get; set; }
      public string SubProductCode { get; set; }

      public int TransacionTypeID { get; set; }

      public string ProductName { get; set; }

      public string Description { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public StatusType Status { get; set; }

   }
}
