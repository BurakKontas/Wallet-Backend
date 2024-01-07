using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.DTOs;
using Wallet.Domain.Entities.Response;
using Wallet.Domain.Enums;
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

        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _authService.Login(loginDTO.Username, loginDTO.Password);
            return token;
        }

        [HttpPost("register")]
        public async Task<RegisterResponse> Register([FromBody] RegisterDTO registerDTO)
        {
            var token = await _authService.Register(registerDTO.Phone, registerDTO.Username, registerDTO.Password);
            return token;
        }

        [HttpPost("resetPassword")]
        public async Task<object> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            await _authService.ResetPassword(phone, resetPasswordDTO.Password, resetPasswordDTO.CurrentPassword);
            return Task.CompletedTask;
        }

        [HttpPost("refreshToken")]
        public async Task<RefreshTokenResponse> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            return await _authService.RefreshToken(refreshTokenDTO.RefreshToken);
        }

        [HttpPost("validateToken")]
        public bool ValidateToken([FromBody] ValidateTokenDTO validateTokenDTO)
        {
            var principal = _tokenService.ValidateToken(validateTokenDTO.Token);
            return principal != null;
        }
    }
}
