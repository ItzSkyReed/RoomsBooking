using System.Security.Cryptography;
using System.Text;
using RoomsBooking.Domain.Interfaces;

namespace RoomsBooking.Infrastructure.Authentication;

public class Sha256TokenHasher : ITokenHasher
{
    public string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public bool Verify(string token, string hashedToken)
    {
        return Hash(token) == hashedToken;
    }
}