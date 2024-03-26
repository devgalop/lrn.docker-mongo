using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Models
{
    public class UserAuth
    {
        [BsonElement("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [BsonElement("exp_refresh_token")]
        public DateTime ExpirationRefreshToken { get; set; }

        [BsonElement("totp_code")]
        public int TotpCode { get; set; }

        [BsonElement("exp_totp_code")]
        public DateTime ExpirationTotpCode { get; set; }
    }
}