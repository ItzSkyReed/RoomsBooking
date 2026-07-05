using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Authentication;

public class InvalidRefreshTokenException() : UnauthorizedException("Недействительный или просроченный Refresh Token.");