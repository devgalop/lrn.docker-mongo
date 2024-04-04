using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Auth.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Auth.Handlers
{
    public class RolePolicyHandler : AuthorizationHandler<RolePolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolePolicyRequirement requirement)
        {
            if(context is null) return Task.CompletedTask;

            if (context.User is null || context.User.Identity?.IsAuthenticated == false)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            
            var roleClaims = context.User.Claims.Where(c => c.Type=="custom-role").Select(c => c.Value).FirstOrDefault();
            
            if (roleClaims is null || !requirement.RolesSplitted.Contains(roleClaims))
            {
                context.Fail();
                return Task.CompletedTask;    
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}