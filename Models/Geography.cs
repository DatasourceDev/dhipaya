using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class Geography
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GeographyID { get; set; }
        public string GeographyName { get; set; }

        //public virtual ICollection<Province> Provinces { get; set; }
        //public virtual ICollection<Aumphur> Aumphurs { get; set; }
        //public virtual ICollection<Tumbon> Tumbons { get; set; }

    }
}

