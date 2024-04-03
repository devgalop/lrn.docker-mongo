using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class ModifyPasswordRequest
    {
        [Required(ErrorMessage ="Username cannot be null or empty")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage ="ActualPassword cannot be null or empty")]
        public string ActualPassword { get; set; } = string.Empty;

        [Required(ErrorMessage ="NewPassword cannot be null or empty")]
        public string NewPassword { get; set; } = string.Empty;
    }
}