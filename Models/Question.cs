using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhipaya.Models
{
   public class QuestionGroup
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

   public class Question
   {
      [Key]
      public int ID { get; set; }

      public int? QuestionGroupID { get; set; }

      public int Index { get; set; }

      public string Title { get; set; }
      public string Description { get; set; }

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
      public virtual QuestionGroup QuestionGroup { get; set; }
  }
}
