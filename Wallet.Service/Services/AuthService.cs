using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Contracts;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities;
using Wallet.Domain.Entities.DTOs;
using Wallet.Domain.Entities.Response;
using Wallet.Domain.Enums;

namespace Wallet.Service.Services
{
    public class AuthService(TokenService JWTService, PasswordHashService passwordHashService, UserRepository userRepository, WalletRepository walletRepository)
    {
        private readonly TokenService _JWTService = JWTService;
        private readonly PasswordHashService _passwordHashService = passwordHashService;
        private readonly UserRepository _usersRepository = userRepository;
        private readonly WalletRepository _walletRepository = walletRepository;

        public async Task<LoginResponse> Login(string username, string password)
        {
            var user = await this._usersRepository.GetByUsername(username);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!_passwordHashService.VerifyPassword(password, user.Hashpassword))
            {
                throw new Exception("Invalid password");
            }
            var token = this._JWTService.GenerateToken(user.Id.ToString(), user.Phone);
            var refreshToken = this._JWTService.GenerateRefreshToken(user.Id.ToString(), user.Phone);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Phone = user.Phone,
                Username = user.Username!
            };
        }

        public async Task<RegisterResponse> Register(string phone,string username, string password)
        {
            var user = await this._usersRepository.GetAsync(phone);
            if (user != null)
            {
                throw new Exception("User already exists");
            }
            var wallet = await this._walletRepository.CreateAsync();
            var newUser = new User
            {
                Phone = phone,
                Username = username,
                Hashpassword = this._passwordHashService.HashPassword(password),
                Walletid = wallet.Id
            };
            await this._usersRepository.AddAsync(newUser);
            var token = this._JWTService.GenerateToken(newUser.Id.ToString(), newUser.Phone);
            var refreshToken = this._JWTService.GenerateRefreshToken(newUser.Id.ToString(), newUser.Phone);
            return new RegisterResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Phone = phone
            };
        }

        public async Task<object> ResetPassword(string phone, string password, string currentPassword)
        {
            var user = await this._usersRepository.GetAsync(phone) ?? throw new Exception("User not found");
            var ifPassword = this._passwordHashService.VerifyPassword(currentPassword, user.Hashpassword);
            if (!ifPassword)
            {
                throw new Exception("Invalid password");
            }
            user.Hashpassword = this._passwordHashService.HashPassword(password);
            await this._usersRepository.UpdateAsync(user);
            return Task.CompletedTask;
        }

        public async Task<RefreshTokenResponse> RefreshToken(string refreshToken)
        {
            var type = _JWTService.GetTokenType(refreshToken);
            if (type != TokenType.RefreshToken)
            {
                throw new Exception("Invalid token");
            }
            var phone = _JWTService.GetPhone(refreshToken);
            var user = await _usersRepository.GetAsync(phone);
            var newToken = _JWTService.GenerateToken(user.Id.ToString(), user.Phone);
            return new RefreshTokenResponse
            {
                Token = newToken,
                Phone = phone,
                RefreshToken = refreshToken,
                Username = user.Username!
            };
        }

    }
}
