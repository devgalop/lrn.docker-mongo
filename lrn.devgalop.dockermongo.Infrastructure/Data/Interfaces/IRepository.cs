using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces
{
    public interface IRepository
    {
        Task<BaseResponse> InsertAsync(User user, CancellationToken ct = default);
    }
}