using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Auth.Requirements
{
    public class RolePolicyRequirement : IAuthorizationRequirement
    {
        public string Roles { get; set; }
        public string[] RolesSplitted => Roles.Split(",");
        public RolePolicyRequirement(string roles)
        {
            Roles = roles;
        }
    }
}