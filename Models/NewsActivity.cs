using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class NewsActivityGroup
   {
      [Key]
      public int ID { get; set; }
      public int Index { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      public StatusType Status { get; set; }
   }


   public class NewsActivity
   {
      [Key]
      public int ID { get; set; }
      public int? GroupID { get; set; }

      public string Title { get; set; }
      public bool IsFavorite { get; set; }

      //[AllowHtml]
      public string Description { get; set; }
      public MediaType MediaType { get; set; }
      public string ImgUrl { get; set; }
      public string VideoUrl { get; set; }
      public StatusType Status { get; set; }
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

      public virtual ICollection<NewsActivityImage> NewsActivityImages { get; set; }
    public virtual NewsActivityGroup NewsActivityGroup { get; set; }


  }
}
