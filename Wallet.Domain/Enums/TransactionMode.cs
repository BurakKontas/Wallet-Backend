using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Enums
{
    public enum TransactionMode
    {
        Send = 1,
        Receive = 2,
        Withdraw = 3,
        Deposit = 4,
        All = 5
    }
}
