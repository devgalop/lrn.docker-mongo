using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Interfaces
{
    public interface ICryptService<CT> 
        where CT : ICryptType
    {
        CryptResponse Encrypt(string inputText, CT cryptParams);
        CryptResponse Decrypt(string textEncrypted, CT cryptParams);
    }
}