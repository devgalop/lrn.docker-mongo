using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Models;

namespace lrn.devgalop.dockermongo.Core.Interfaces
{
    public interface IUserManagementService
    {
        Task<BaseResponse> InsertUserAsync(InsertUserRequest request);
    }
}