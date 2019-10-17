using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinceID { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
      public string ProvinceNameEn { get; set; }

      //public virtual Geography Geography { get; set; }
      //public virtual ICollection<Aumphur> Aumphurs { get; set; }
      //public virtual ICollection<Tumbon> Tumbons { get; set; }
   }
}
