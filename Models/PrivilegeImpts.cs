using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class PrivilegeImpts
   {
        [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int No { get; set; }
        public string PrivilegeType { get; set; }
        public string MerchantName { get; set; }
        public string PrivilegeName { get; set; }
        public string ProvinceName { get; set; }
      public string Silver { get; set; }
      public string Gold { get; set; }
      public string Condition { get; set; }
      public DateTime? PeriodFrom { get; set; }
      public DateTime? PeriodTo { get; set; }
      public string Limit { get; set; }
      public string LimitPeriod { get; set; }
      public string LimitPerPerson { get; set; }
      public string LimitPerPersonPeriod { get; set; }
      public string Outlets { get; set; }
      public string Youtube { get; set; }
   }
}

