using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Models
{
    public class ClaimsResponse : BaseResponse
    {
        public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();
    }
}