using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Models
{
    public class UserResponse : BaseResponse
    {
        public User? Result { get; set; }
    }
}