using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities;

namespace Wallet.Service.Services
{
    internal class AuthService(TokenService JWTService, PasswordHashService passwordHashService, UserRepository userRepository, WalletRepository walletRepository)
    {
        private readonly TokenService _JWTService = JWTService;
        private readonly PasswordHashService _passwordHashService = passwordHashService;
        private readonly UserRepository _usersService = userRepository;
        private readonly WalletRepository _walletService = walletRepository;

        public async Task<string> Login(string phone, string password)
        {
            var user = await this._usersService.GetAsync(phone);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!_passwordHashService.VerifyPassword(password, user.Hashpassword))
            {
                throw new Exception("Invalid password");
            }
            return this._JWTService.GenerateToken(user.Id.ToString(), user.Phone);
        }

        public async Task<string> Register(string phone, string password)
        {
            var user = await this._usersService.GetAsync(phone);
            if (user != null)
            {
                throw new Exception("User already exists");
            }
            var wallet = await this._walletService.CreateAsync();
            var newUser = new User
            {
                Phone = phone,
                Hashpassword = this._passwordHashService.HashPassword(password),
                Walletid = wallet.Id
            };
            await this._usersService.AddAsync(newUser);
            return this._JWTService.GenerateToken(newUser.Id.ToString(), newUser.Phone);
        }

        public async void ResetPassword(string phone, string password)
        {
            var user = await this._usersService.GetAsync(phone) ?? throw new Exception("User not found");
            user.Hashpassword = this._passwordHashService.HashPassword(password);
            await this._usersService.UpdateAsync(user);
        }

    }
}
