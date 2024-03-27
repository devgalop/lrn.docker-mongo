using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Models
{
    public class BaseResponse
    {
        public bool IsSucceed { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDescription { get; set; }
    }
}