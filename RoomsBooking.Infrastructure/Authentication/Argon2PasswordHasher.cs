
using RoomsBooking.Application.Interfaces; // Твой интерфейс из Application
using Isopoh.Cryptography.Argon2;

namespace RoomsBooking.Infrastructure.Authentication;

public class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return Argon2.Hash(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return Argon2.Verify(hashedPassword, password);
    }
}