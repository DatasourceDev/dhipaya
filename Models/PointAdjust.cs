using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class PointAdjust
   {
      [Key]
      public int ID { get; set; }
      public string ConditionCode { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public CustomerChanal CustomerChanal { get; set; }
      public int TransacionTypeID { get; set; }
      public int CustomerID { get; set; }
      public int Point { get; set; }
      public decimal PurchaseAmt { get; set; }
      
      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public virtual Customer Customer { get; set; }

   }
}
