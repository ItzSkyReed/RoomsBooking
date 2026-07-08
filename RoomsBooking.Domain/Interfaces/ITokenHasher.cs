namespace RoomsBooking.Domain.Interfaces;

public interface ITokenHasher
{
    string Hash(string token);
    bool Verify(string token, string hashedToken);
}