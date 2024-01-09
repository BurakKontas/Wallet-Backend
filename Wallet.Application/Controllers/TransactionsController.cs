using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities;
using Wallet.Domain.Entities.DTOs;
using Wallet.Domain.Entities.Response;
using Wallet.Domain.Enums;
using Wallet.Infrastructure;
using Wallet.Service.Services;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {

        private readonly ILogger<TransactionsController> _logger;
        private readonly TransactionsService _transactionService;
        private readonly TokenService _tokenService;

        public TransactionsController(ILogger<TransactionsController> logger, WalletContext context, IConfiguration configuration)
        {
            _logger = logger;
            var userRepository = new UserRepository(context);
            var walletRepository = new WalletRepository(context);
            var transactionRepository = new TransactionRepository(context);

            _transactionService = new TransactionsService(walletRepository, transactionRepository, userRepository);
            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
        }

        [HttpPost("sendmoney")]
        public async Task<object> SendMoney([FromBody] SendMoneyDTO sendMoneyDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];

            var senderId = _tokenService.GetUserId(token);

            await _transactionService.SendMoney(senderId, sendMoneyDTO.RecipientPhone, sendMoneyDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpPost("withdraw")]
        public async Task<object> Withdraw([FromBody] WithdrawDTO withdrawDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);

            await _transactionService.Withdraw(phone, withdrawDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpPost("deposit")]
        public async Task<object> Deposit([FromBody] DepositDTO depositDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);

            await _transactionService.Deposit(phone, depositDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpPost("transactions")]
        public async Task<GetTransactionsResponse> Get([FromBody] GetTransactionsDTO getTransactionsDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            var userId = _tokenService.GetUserId(token);

            var transactions = await _transactionService.GetTransactionsByUserAsync(userId, getTransactionsDTO.limit, getTransactionsDTO.mode);
            return transactions;
        }
    }
}
