using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class CustomerPrivilege
    {
        [Key]
        public int CustomerPrivilegeID { get; set; }
        public int? CustomerID { get; set; }
        public int? PrivilegeID { get; set; }
        public decimal? CreditPoint { get; set; }

        public string Chanel { get; set; }
        public DateTime? SubmitDate { get; set; }

        public string Create_By { get; set; }
        public DateTime? Create_On { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_On { get; set; }

    }
}
