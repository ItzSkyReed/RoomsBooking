using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.User;

public class UserAlreadyExistsException : ConflictException
{
    public UserAlreadyExistsException(string email)
        : base($"Пользователь с Email '{email}' уже существует.", "email") { }

    public UserAlreadyExistsException()
        : base("Пользователь с таким Email уже существует.", "email") { }
}