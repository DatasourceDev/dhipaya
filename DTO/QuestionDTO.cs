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
   public class QuestionDTO
   {
      public string search_text { get; set; }
      public int? GroupID { get; set; }
      public IEnumerable<Question> Questions { get; set; }
   }
   public class NewsActivityDTO
   {
      public string search_text { get; set; }
      public int? GroupID { get; set; }
      public IEnumerable<NewsActivity> NewsActivities { get; set; }
   }
}
