using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Infrastructure;
using Wallet.Service.Services;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletController : ControllerBase
    {

        private readonly ILogger<WalletController> _logger;
        private readonly WalletRepository _walletRepository;
        private readonly WalletService _walletService;
        private readonly TokenService _tokenService;

        public WalletController(ILogger<WalletController> logger, WalletContext context, IConfiguration configuration)
        {
            _walletRepository = new WalletRepository(context);
            _walletService = new WalletService(_walletRepository);
            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
            _logger = logger;
        }

        [HttpGet("balance")]
        public async Task<decimal> GetBalance()
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userId = _tokenService.GetUserId(token);

            var wallet = await _walletRepository.GetAsync(userId);
            return await _walletService.GetBalance(wallet.Id);
        }
    }
}
