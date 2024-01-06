using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;

namespace Wallet.Service.Services;

public class TransactionsService(WalletRepository walletRepository, TransactionRepository transactionRepository, UserRepository userRepository)
{
    private readonly WalletRepository _walletRepository = walletRepository;
    private readonly TransactionRepository _transactionRepository = transactionRepository;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<Wallets> SendMoney(int senderId, int receiverId, decimal amount)
    {
        var sender = await this._walletRepository.GetAsync(senderId) ?? throw new Exception("Sender not found");
        var receiver = await this._walletRepository.GetAsync(receiverId) ?? throw new Exception("Receiver not found");
        if (sender.Balance < amount)
        {
            throw new Exception("Insufficient funds");
        }
        var wallet = await this._walletRepository.UpdateBalance(senderId, sender.Balance - amount);
        await this._walletRepository.UpdateBalance(receiverId, receiver.Balance + amount);
        var transaction = new Transaction
        {
            Senderid = senderId,
            Receiverid = receiverId,
            Amount = amount,
            Date = DateTime.Now
        };
        await this._transactionRepository.AddAsync(transaction);
        return wallet;
    }

    public async Task<Wallets> Deposit(int walletId, decimal amount)
    {
        var wallet = await this._walletRepository.GetAsync(walletId) ?? throw new Exception("Wallet not found");
        var user = await this._userRepository.GetByWalletId(wallet.Id) ?? throw new Exception("User not found");
        wallet = await this._walletRepository.UpdateBalance(walletId, wallet.Balance + amount);
        var transaction = new Transaction
        {
            Senderid = user.Id,
            Receiverid = user.Id,
            Amount = amount,
            Date = DateTime.Now
        };
        await this._transactionRepository.AddAsync(transaction);
        return wallet;
    }

    public async Task<Wallets> Withdraw(int walletId, decimal amount)
    {
        var wallet = await this._walletRepository.GetAsync(walletId) ?? throw new Exception("Wallet not found");
        var user = await this._userRepository.GetByWalletId(wallet.Id) ?? throw new Exception("User not found");
        if (wallet.Balance < amount)
        {
            throw new Exception("Insufficient funds");
        }
        wallet = await this._walletRepository.UpdateBalance(walletId, wallet.Balance - amount);
        var transaction = new Transaction
        {
            Senderid = user.Id,
            Receiverid = user.Id,
            Amount = -amount,
            Date = DateTime.Now
        };
        await this._transactionRepository.AddAsync(transaction);
        return wallet;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(int userId, int limit = 10, TransactionMode mode = TransactionMode.All)
    {
        var user = await this._userRepository.GetAsync(userId) ?? throw new Exception("User not found");
        var transactions = await this._transactionRepository.GetTransactionsByUserAsync(user, limit, mode);
        return transactions;
    }
}
