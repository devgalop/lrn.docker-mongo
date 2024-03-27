using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class BasicUserResponse : BaseResponse
    {
        public string? Username { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}