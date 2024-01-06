using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Entities.DTOs
{
    public class SendMoneyDTO
    {
        public string RecipientPhone { get; set; }
        public decimal Amount { get; set; }
    }
}
