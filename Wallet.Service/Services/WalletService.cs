using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;

namespace Wallet.Service.Services;

public class WalletService(WalletRepository walletRepository)
{
    private readonly WalletRepository _walletRepository = walletRepository;

    public async Task<decimal> GetBalance(int walletId)
    {
        var wallet = await this._walletRepository.GetAsync(walletId) ?? throw new Exception("Wallet not found");
        return wallet.Balance;
    }

}
