using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Models
{
    public class CryptResponse : BaseResponse
    {
        public string? Text { get; set; }
    }
}