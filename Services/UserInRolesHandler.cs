using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Dhipaya.Services
{
    public class UserInRolesHandler : AuthorizationHandler<UserInRolesRequirement>
    {
        ILoginServices loginServices;

        public UserInRolesHandler(ILoginServices loginServices)
        {
            this.loginServices = loginServices;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInRolesRequirement requirement)
        {
            if (!this.loginServices.isAuthen())
            {
                return Task.CompletedTask;
            }
            else
            {
                var roles = requirement.Roles;
                if (this.loginServices.isInRoles(roles))
                {
                    context.Succeed(requirement);
                }
            }            
            return Task.CompletedTask;
        }
    }
}
