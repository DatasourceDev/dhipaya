using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class CustomerExport
   {
      [Key]
      public int ID { get; set; }

      public string RefCode { get; set; }
      
      public string NameTh { get; set; }
      public string SurNameTh { get; set; }
      public string UserName { get; set; }
      public string MoblieNo { get; set; }
      public string IDCard { get; set; }
      public string CustomerClass { get; set; }
      public string Channel { get; set; }
      public string Address { get; set; }
      public string Province { get; set; }
      public string Email { get; set; }
      public string Create_On { get; set; }
      public string Point { get; set; }  
      public string DOB { get; set; }
      public string DOBYear { get; set; }
      public string DOBMonth { get; set; }
   }
}
