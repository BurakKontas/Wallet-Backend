using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;

namespace Wallet.DataAccess.Contracts
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetAsync(int id);
        Task AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(User user, int limit = 10, TransactionMode mode = TransactionMode.All);
    }
}
