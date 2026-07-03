using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.User;

public class UserNotFoundException(string? user = null)
    : NotFoundException(string.IsNullOrWhiteSpace(user) ? "Пользователь не найден." : $"Пользователь {user} не найден.");