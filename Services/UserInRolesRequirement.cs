using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Dhipaya.Services
{
    public class UserInRolesRequirement : IAuthorizationRequirement
    {
        public string[] Roles { get; private set; }

        public UserInRolesRequirement(string[] roles)
        {
            this.Roles = roles;
        }
    }
}
