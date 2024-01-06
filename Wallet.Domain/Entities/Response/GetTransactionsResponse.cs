using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Enums;

namespace Wallet.Domain.Entities.Response
{
    public class GetTransaction
    {
        public string SenderPhone { get; set; }

        public string ReceiverPhone { get; set; }

        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }

    }

    public class GetTransactionsResponse
    {
        public IEnumerable<GetTransaction> Transactions { get; set; } = new List<GetTransaction>();
    }
}
