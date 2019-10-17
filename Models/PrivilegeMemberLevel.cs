using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class PrivilegeMemberLevel
    {
        [Key]
        public int PrivilegeMemberLevelID { get; set; }
        public string MemberLevel { get; set; }

        public decimal? Percent { get; set; }
        public StatusType Status { get; set; }

        public int? PrivilegeID { get; set; }
        public virtual Privilege Privilege { get; set; }
    }
}
