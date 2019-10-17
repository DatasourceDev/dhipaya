using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class ShareholderNoSendMailImpt
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public string AccountID { get; set; }
      public string Name { get; set; }

   }
}
