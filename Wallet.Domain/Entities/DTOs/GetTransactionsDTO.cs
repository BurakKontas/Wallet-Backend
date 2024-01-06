using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;

namespace Wallet.Domain.Entities.DTOs
{
    public class GetTransactionsDTO
    {
        public int limit { get; set; } = 10;
        public TransactionMode mode { get; set; } = TransactionMode.All;
    }
}
