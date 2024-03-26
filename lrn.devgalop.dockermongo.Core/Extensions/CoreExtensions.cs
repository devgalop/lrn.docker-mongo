using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace lrn.devgalop.dockermongo.Core.Extensions
{
    public static class CoreExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IUserManagementService, UserManagementService>();
        }
        
    }
}