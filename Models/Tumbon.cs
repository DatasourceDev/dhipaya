using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class Tumbon
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int TumbonID { get; set; }
      public string TumbonCode { get; set; }
      public string TumbonName { get; set; }
      public string TumbonNameEn { get; set; }
      public int ProvinceID { get; set; }
      public int AumphurID { get; set; }
      public string PostalCode { get; set; }

      public virtual Province Province { get; set; }
      public virtual Aumphur Aumphur { get; set; }
   }
}
