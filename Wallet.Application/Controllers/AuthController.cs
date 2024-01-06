using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.DTOs;
using Wallet.Infrastructure;
using Wallet.Service.Services;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private readonly TokenService _tokenService;
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthController> logger, WalletContext context, IConfiguration configuration)
        {
            _logger = logger;

            var walletRepository = new WalletRepository(context);
            var userRepository = new UserRepository(context);

            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
            var passwordHashService = new PasswordHashService(configuration["Hasher:SecretKey"]!);
            _authService = new AuthService(_tokenService, passwordHashService, userRepository, walletRepository);
        }

        [HttpPost("/login")]
        public async Task<string> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _authService.Login(loginDTO.Phone, loginDTO.Password);
            return token;
        }

        [HttpPost("/register")]
        public async Task<string> Register([FromBody] RegisterDTO registerDTO)
        {
            var token = await _authService.Register(registerDTO.Phone, registerDTO.Username, registerDTO.Password);
            return token;
        }

        [HttpPost("resetPassword")]
        public async Task<object> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            _authService.ResetPassword(phone, resetPasswordDTO.Password);
            return Task.CompletedTask;
        }
    }
}
