using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.DTOs;
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
        private readonly WalletRepository _walletRepository;
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;

        public TransactionsController(ILogger<TransactionsController> logger, WalletContext context, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = new UserRepository(context);
            _walletRepository = new WalletRepository(context);
            var transactionRepository = new TransactionRepository(context);

            _transactionService = new TransactionsService(_walletRepository, transactionRepository, _userRepository);
            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
        }

        [HttpPost("sendmoney")]
        public async Task<object> SendMoney([FromBody] SendMoneyDTO sendMoneyDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var senderId = _tokenService.GetUserId(token);
            var recipient = await _userRepository.GetAsync(sendMoneyDTO.RecipientPhone);

            _transactionService.SendMoney(senderId, recipient.Id, sendMoneyDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpPost("withdraw")]
        public async Task<object> Withdraw([FromBody] WithdrawDTO withdrawDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            var user = await _userRepository.GetAsync(phone);

            _transactionService.Withdraw(user.Walletid, withdrawDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpPost("deposit")]
        public async Task<object> Deposit([FromBody] DepositDTO depositDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            var user = await _userRepository.GetAsync(phone);

            _transactionService.Deposit(user.Walletid, depositDTO.Amount);
            return Task.CompletedTask;
        }

        [HttpGet("transactions/{id}")]
        public async Task Get(TransactionMode id, [FromQuery] int limit = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userId = _tokenService.GetUserId(token);

            _transactionService.GetTransactionsByUserAsync(userId, limit, id);
        }
    }
}
