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
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private readonly TokenService _tokenService;
        private readonly AuthService _authService;
        private readonly UserRepository _userRepository;

        public AuthController(ILogger<AuthController> logger, WalletContext context, IConfiguration configuration)
        {
            _logger = logger;

            var walletRepository = new WalletRepository(context);
            _userRepository = new UserRepository(context);

            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
            var passwordHashService = new PasswordHashService(configuration["Hasher:SecretKey"]!);
            _authService = new AuthService(_tokenService, passwordHashService, _userRepository, walletRepository);
        }

        [HttpPost("login")]
        public async Task<string> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _authService.Login(loginDTO.Phone, loginDTO.Password);
            return token;
        }

        [HttpPost("register")]
        public async Task<string> Register([FromBody] RegisterDTO registerDTO)
        {
            var token = await _authService.Register(registerDTO.Phone, registerDTO.Username, registerDTO.Password);
            return token;
        }

        [HttpPost("resetPassword")]
        public object ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            _authService.ResetPassword(phone, resetPasswordDTO.Password);
            return Task.CompletedTask;
        }

        [HttpPost("newToken")]
        public async Task<string> NewToken([FromBody] NewTokenDTO newTokenDTO)
        {
            var type = _tokenService.GetTokenType(newTokenDTO.RefreshToken);
            if (type != TokenType.RefreshToken)
            {
                throw new Exception("Invalid token type");
            }
            var phone = _tokenService.GetPhone(newTokenDTO.RefreshToken);
            var user = await _userRepository.GetAsync(phone);
            var newToken = _tokenService.GenerateToken(user.Id.ToString(), user.Phone);
            return newToken;
        }

        [HttpGet("refreshToken")]
        public async Task<string> RefreshToken()
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var phone = _tokenService.GetPhone(token);
            var user = await _userRepository.GetAsync(phone);
            var newToken = _tokenService.GenerateToken(user.Id.ToString(), user.Phone, 60*24*7); // 7 days
            return newToken;
        }

        [HttpPost("validateToken")]
        public bool ValidateToken([FromBody] ValidateTokenDTO validateTokenDTO)
        {
            var principal = _tokenService.ValidateToken(validateTokenDTO.Token);
            return principal != null;
        }
    }
}
