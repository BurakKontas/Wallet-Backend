using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities;
using Wallet.Domain.Entities.Response;
using Wallet.Domain.Enums;

namespace Wallet.Service.Services;

public class TransactionsService(WalletRepository walletRepository, TransactionRepository transactionRepository, UserRepository userRepository)
{
    private readonly WalletRepository _walletRepository = walletRepository;
    private readonly TransactionRepository _transactionRepository = transactionRepository;
    private readonly UserRepository _userRepository = userRepository;

    public async Task<Wallets> SendMoney(int senderId, string receiverPhone, decimal amount)
    {
        var sender = await this._walletRepository.GetAsync(senderId) ?? throw new Exception("Sender not found");
        var receiver = await this._userRepository.GetAsync(receiverPhone) ?? throw new Exception("Receiver not found");
        var receiverWallet = await this._walletRepository.GetAsync(receiver.Id) ?? throw new Exception("Receiver not found");
        if (sender.Balance < amount)
        {
            throw new Exception("Insufficient funds");
        }
        var wallet = await this._walletRepository.UpdateBalance(senderId, sender.Balance - amount);
        await this._walletRepository.UpdateBalance(receiver.Id, receiverWallet.Balance + amount);
        var transaction = new Transaction
        {
            Senderid = senderId,
            Receiverid = receiver.Id,
            Amount = amount,
            Date = DateTime.Now
        };
        await this._transactionRepository.AddAsync(transaction);
        return wallet;
    }

    public async Task<Wallets> Deposit(string phone, decimal amount)
    {
        var user = await this._userRepository.GetAsync(phone) ?? throw new Exception("User not found");
        var wallet = await this._walletRepository.GetAsync(user.Walletid) ?? throw new Exception("Wallet not found");
        wallet = await this._walletRepository.UpdateBalance(user.Walletid, wallet.Balance + amount);
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

    public async Task<Wallets> Withdraw(string phone, decimal amount)
    {
        var user = await this._userRepository.GetAsync(phone) ?? throw new Exception("User not found");
        var wallet = await this._walletRepository.GetAsync(user.Walletid) ?? throw new Exception("Wallet not found");
        if (wallet.Balance < amount)
        {
            throw new Exception("Insufficient funds");
        }
        wallet = await this._walletRepository.UpdateBalance(user.Walletid, wallet.Balance - amount);
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

    public async Task<GetTransactionsResponse> GetTransactionsByUserAsync(int userId, int limit = 10, TransactionMode mode = TransactionMode.All)
    {
        var user = await this._userRepository.GetAsync(userId) ?? throw new Exception("User not found");
        var transactions = await this._transactionRepository.GetTransactionsByUserAsync(user, limit, mode);
        var returnData = new List<GetTransaction>();

        foreach (var transaction in transactions)
        {
            TransactionMode transactionMode;

            if(transaction.Senderid == transaction.Receiverid)
            {
                if(transaction.Amount < 0)
                {
                    transactionMode = TransactionMode.Withdraw;
                }
                else
                {
                    transactionMode = TransactionMode.Deposit;
                }
            } 
            else if(transaction.Senderid == user.Id)
            {
                transactionMode = TransactionMode.Send;
            } 
            else
            {
                transactionMode = TransactionMode.Receive;
            }

            returnData.Add(new GetTransaction
            {
                SenderPhone = transaction.Sender!.Phone,
                ReceiverPhone = transaction.Receiver!.Phone,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Mode = transactionMode,
            });
        }
        return new GetTransactionsResponse
        {
            Transactions = returnData
        };
    }
}
