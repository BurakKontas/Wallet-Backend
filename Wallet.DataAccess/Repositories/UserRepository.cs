using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Contracts;
using Wallet.Domain.Entities;
using Wallet.Infrastructure;

namespace Wallet.DataAccess.Repositories;

public class UserRepository(WalletContext context) : IUserRepository
{
    private readonly WalletContext _context = context;

    public async Task<object> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Task.CompletedTask;
    }

    public async Task<object> DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Task.CompletedTask;
    }

    public async Task<User> GetAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user!;
    }

    public async Task<User> GetByUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        return user!;
    }

    public async Task<User> GetAsync(string phone)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == phone);
        return user!;
    }

    public async Task<User> GetByWalletId(int walletId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Walletid == walletId);
        return user!;
    }

    public async Task<object> UpdateAsync(User user)
    {
        var oldUser = await GetAsync(user.Id);

        oldUser.Phone = user.Phone;
        oldUser.Username = user.Username;
        oldUser.Walletid = user.Walletid;
        oldUser.Hashpassword = user.Hashpassword;

        await _context.SaveChangesAsync();
        return Task.CompletedTask;
    }

    public async Task<List<string>> CheckUsers(List<string> phones)
    {
        var users = await _context.Users.Where(x => phones.Contains(x.Phone)).ToListAsync();
        return users.Select(x => x.Phone).ToList();
    }
}
