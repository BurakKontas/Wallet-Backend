using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.DTOs;
using Wallet.Domain.Entities.Response;
using Wallet.Infrastructure;
using Wallet.Service.Services;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UsersService _usersService;
        private readonly TokenService _tokenService;

        public UserController(ILogger<UserController> logger, WalletContext context, IConfiguration configuration)
        {
            var usersRepository = new UserRepository(context);
            _usersService = new UsersService(usersRepository);
            _tokenService = new TokenService(configuration["JWT:SecretKey"]!, configuration["JWT:Issuer"]!, configuration["JWT:Audience"]!);
            _logger = logger;
        }

        [HttpPost("checkcontacts")]
        public async Task<CheckContactsResponse> CheckContacts([FromBody] CheckContactsDTO checkContactsDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            _tokenService.ValidateToken(token);
            //format phones
            checkContactsDTO.Contacts = checkContactsDTO.Contacts.Select(c => c.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")).ToList();

            var availableContacts = await _usersService.CheckContacts(checkContactsDTO.Contacts);
            return availableContacts;

        }

        [HttpPost("getusername")]
        public async Task<GetUsernameResponse> GetUsernameByPhone([FromBody] GetUsernameDTO getUsernameDTO)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1];
            _tokenService.ValidateToken(token);
            return await _usersService.GetUsernameByPhone(getUsernameDTO.Phone);
        }
    }
}
