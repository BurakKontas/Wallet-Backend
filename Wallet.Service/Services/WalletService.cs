using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.Response;

namespace Wallet.Service.Services;

public class WalletService(WalletRepository walletRepository)
{
    private readonly WalletRepository _walletRepository = walletRepository;

    public async Task<BalanceResponse> GetBalance(int walletId)
    {
        var wallet = await this._walletRepository.GetAsync(walletId) ?? throw new Exception("Wallet not found");
        return new BalanceResponse
        {
            Balance = wallet.Balance
        };
}

}
