using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Enums
{
    public enum TokenType
    {
        RefreshToken = 0,
        AccessToken = 1,
        Error = 2
    }
}
