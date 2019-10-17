using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class ShareholderImpt
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public string AccountID { get; set; }
    
      public string PrefixTh { get; set; }
      public string NameTh { get; set; }
      public string SurNameTh { get; set; }
      public string Building { get; set; }
      public string Lane { get; set; }
      public string Road { get; set; }
      public string Province { get; set; }
      public string Aumper { get; set; }
      public string Tumbon { get; set; }
      public string ZipCode { get; set; }
      public string Email { get; set; }
      public string IDCard { get; set; }
      public string MoblieNo { get; set; }
      public string HTelNo { get; set; }
      public string WTelNo { get; set; }
      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
   }
}
