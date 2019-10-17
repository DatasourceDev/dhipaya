using Dhipaya.Extensions;
using Dhipaya.Models;
using Dhipaya.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.DTO
{
   public class UserDTO : ModelBaseDTO
   {
      public IEnumerable<User> Users { get; set; }
   }

   public class HomeDTO
   {
      public string CustomerClass { get; set; }

   }

}
