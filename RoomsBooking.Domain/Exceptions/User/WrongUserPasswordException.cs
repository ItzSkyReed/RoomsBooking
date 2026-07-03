using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.User;

public class WrongUserPasswordException(string user) : UnauthorizedException($"Неправильный пароль пользователя {user}.");