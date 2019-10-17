using Dhipaya.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Services
{
   public interface ILoginServices
   {
      bool isAuthen();
      bool isInAdminRoles(string[] roles);
      bool isInRoles(string[] roles);
      void Login(User user, bool isPersistent);
      void Logout();
   }
}
