using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Contracts;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;
using Wallet.Infrastructure;

namespace Wallet.DataAccess.Repositories;

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
        Expression<Func<Transaction, bool>>? filter = mode switch
        {
            TransactionMode.Receive => (x => x.Receiverid == user.Id && x.Senderid != user.Id),
            TransactionMode.Send => (x => x.Senderid == user.Id && x.Receiverid != user.Id),
            TransactionMode.Deposit => (x => x.Senderid == user.Id && x.Receiverid == user.Id && x.Amount > 0),
            TransactionMode.Withdraw => (x => x.Senderid == user.Id && x.Receiverid == user.Id && x.Amount < 0),
            TransactionMode.All => (x => x.Senderid == user.Id || x.Receiverid == user.Id),
            _ => null
        };

        if (filter == null) return null!;
        
        var transactions = await _context.Transactions
            .Where(filter)
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .ToListAsync();

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
