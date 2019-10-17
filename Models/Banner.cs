using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhipaya.Models
{
   public class Banner
   {
      [Key]
      public int ID { get; set; }

      public string Url { get; set; }

      public string MobileUrl { get; set; }

      public int Index { get; set; }

      [NotMapped]
      public string sDate { get; set; }
      [NotMapped]
      public string eDate { get; set; }

      public DateTime? StartDate { get; set; }

      [DataType(DataType.Date)]
      [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
      public DateTime? EndDate { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public StatusType Status { get; set; }

   }
}
