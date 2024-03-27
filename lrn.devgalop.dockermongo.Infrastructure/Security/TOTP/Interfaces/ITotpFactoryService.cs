using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Security.TOTP.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.TOTP.Interfaces
{
    public interface ITotpFactoryService
    {
        TotpResponse Compute();
        TotpResponse ComputeEncrypted();
    }
}