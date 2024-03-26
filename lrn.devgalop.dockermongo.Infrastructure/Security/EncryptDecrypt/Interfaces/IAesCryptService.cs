using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Interfaces
{
    public interface IAesCryptService : ICryptService<AesCryptType>
    {
        string GenerateKey();
    }
}