using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}