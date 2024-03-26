using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class InsertUserRequest
    {
        [Required(ErrorMessage = "Username must be provided")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password must be provided")]
        public string Password { get; set; } = string.Empty;
        public DateTime RegistrationDate = DateTime.UtcNow;
        public DateTime UpdateDate = DateTime.UtcNow;
        public bool Status = true;
    }
}