using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Contracts;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;
using Wallet.Infrastructure;

namespace Wallet.DataAccess.Repositories
{
    public class TransactionRepository(WalletContext context): ITransactionRepository
    {
        private readonly WalletContext _context = context;

        public Task AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            return _context.SaveChangesAsync();
        }

        public Task<Transaction> GetAsync(int id)
        {
            var transaction = _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            return transaction!;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(User user, int limit = 10, TransactionMode mode = TransactionMode.All)
        {
            List<Transaction> transactions;
            switch(mode)
            {
                case TransactionMode.Deposit:
                    transactions = await _context.Transactions
                        .Where(x => x.Receiverid == user.Id)
                        .ToListAsync();
                    break;
                case TransactionMode.Withdraw:
                    transactions = await _context.Transactions
                        .Where(x => x.Senderid == user.Id)
                        .ToListAsync();
                    break;
                case TransactionMode.All:
                    transactions = await _context.Transactions
                        .Where(x => (x.Senderid == user.Id) || (x.Receiverid == user.Id))
                        .ToListAsync();
                    break;
                default:
                    return null!;
            }
            return transactions;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            var oldTransaction = await GetAsync(transaction.Id);

            oldTransaction.Senderid = transaction.Senderid;
            oldTransaction.Receiverid = transaction.Receiverid;
            oldTransaction.Amount = transaction.Amount;

            await _context.SaveChangesAsync();
        }
    }
}
