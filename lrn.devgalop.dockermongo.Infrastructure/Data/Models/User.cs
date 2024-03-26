using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("user")]
        [Required(ErrorMessage = "Username field is mandatory")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        [Required(ErrorMessage ="Password field is required")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("registration_date")]
        public DateTime RegistrationDate { get; set; }

        [BsonElement("update_date")]
        public DateTime UpdateDate { get; set; }

        [BsonElement("status")]
        public bool Status { get; set; } = true;

        [BsonElement("role")]
        public ERole RoleId { get; set; } = ERole.BASIC_AUTH;

        [BsonElement("user_auth")]
        public UserAuth? Auth { get; set; }
    }
}