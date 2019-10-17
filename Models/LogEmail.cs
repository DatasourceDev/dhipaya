using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class LogEmail
    {
        [Key]
        public int LogID { get; set; }
        public string Email { get; set; }
        public string Msg { get; set; }
        public DateTime? SentDateTime { get; set; }

    }
}
