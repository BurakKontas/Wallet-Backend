using Microsoft.EntityFrameworkCore;
using Wallet.DataAccess.Contracts;
using Wallet.Domain.Entities;
using Wallet.Infrastructure;

public class WalletRepository(WalletContext context) : IWalletRepository
{
    private readonly WalletContext _context = context;

    public async Task<Wallets> GetAsync(int id)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == id);
        return wallet!;
    }

    public async Task<decimal?> GetBalanceAsync(int id)
    {
        var wallet = await GetAsync(id);
        var balance = wallet.Balance;
        return balance;
    }

    public async Task<Wallets> UpdateBalance(int id, decimal amount)
    {
        var wallet = await GetAsync(id);
        wallet.Balance = amount;
        await _context.SaveChangesAsync();
        return wallet;
    }
}
