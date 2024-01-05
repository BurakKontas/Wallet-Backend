using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;

namespace Wallet.DataAccess.Contracts
{
    public interface IWalletRepository
    {
        public Task<Wallets> GetAsync(int id);
        public Task<decimal?> GetBalanceAsync(int id);
        public Task<Wallets> UpdateBalance(int id, decimal amount);
    }
}
