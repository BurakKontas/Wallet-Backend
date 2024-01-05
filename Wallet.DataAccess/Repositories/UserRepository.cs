using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Contracts;
using Wallet.Domain.Entities;
using Wallet.Infrastructure;

namespace Wallet.DataAccess.Repositories
{
    public class UserRepository(WalletContext context) : IUserRepository
    {
        private readonly WalletContext _context = context;

        public Task AddAsync(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync();
        }

        public async Task<User> GetAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user!;
        }

        public async Task<User> GetAsync(string phone)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == phone);
            return user!;
        }

        public async Task UpdateAsync(User user)
        {
            var oldUser = await GetAsync(user.Id);

            oldUser.Phone = user.Phone;
            oldUser.Username = user.Username;
            oldUser.Walletid = user.Walletid;
            oldUser.Hashpassword = user.Hashpassword;

            await _context.SaveChangesAsync();
        }
    }
}
