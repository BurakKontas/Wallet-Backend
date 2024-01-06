using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Wallet.Service.Services;

public class PasswordHashService(string secretKey)
{
    private readonly string _secretKey = secretKey;

    public string HashPassword(string password)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
