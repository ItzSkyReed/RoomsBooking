using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.User;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id)
        : base($"Пользователь c Id: {id} не найден.")
    {
    }

    public UserNotFoundException()
        : base("Пользователь не найден.")
    {
    }

    public UserNotFoundException(string email)
        : base($"Пользователь {email} не найден.")
    {
    }
}