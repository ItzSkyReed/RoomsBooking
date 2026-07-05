using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Authentication;

public class InvalidAccessTokenException() : UnauthorizedException("Недействительный или просроченный Access Token.");