using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.DataAccess.Repositories;
using Wallet.Domain.Entities.Response;

namespace Wallet.Service.Services;

public class UsersService(UserRepository userRepository)
{
    private readonly UserRepository _userRepository = userRepository;

    public async Task<CheckContactsResponse> CheckContacts(List<string> contacts)
    {
        var users = await this._userRepository.CheckUsers(contacts);
        return new CheckContactsResponse
        {
            Contacts = users
        };
    }

    public async Task<GetUsernameResponse> GetUsernameByPhone(string phone)
    {
        var user = await this._userRepository.GetAsync(phone);
        return new GetUsernameResponse
        {
            Username = user.Username
        };
    }

}
