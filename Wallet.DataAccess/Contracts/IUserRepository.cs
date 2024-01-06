using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;

namespace Wallet.DataAccess.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetAsync(int id);
        Task<User> GetAsync(string phone);
        Task<User> GetByWalletId(int walletId);
        Task<object> AddAsync(User user);
        Task<object> UpdateAsync(User user);
        Task<object> DeleteAsync(User user);
        Task<List<string>> CheckUsers(List<string> phones);
    }
}
