using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhipaya.Models
{
   public class CustomerClass
   {
      [Key]
      public int ID { get; set; }
      public bool UnEditable { get; set; }
      public string Name { get; set; }
      public string Prefix { get; set; }
      public string Description { get; set; }
      public string ProjectCode { get; set; }
      public string ProjectName { get; set; }
      public StatusType Status { get; set; }
      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

   }

   public class CustomerClassChange
   {
      [Key]
      public int ID { get; set; }
      public int? CustomerID { get; set; }
      public int? FromID { get; set; }
      public int? ToID { get; set; }

      public string From { get; set; }
      public string To { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public virtual Customer Customer { get; set; }

   }

   public class TerminateCustomerClassChange
   {
      [Key]
      public int ID { get; set; }
      public int? CustomerID { get; set; }
      public int? FromID { get; set; }
      public int? ToID { get; set; }

      public string From { get; set; }
      public string To { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }

   }
}
