using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Room;

public class RoomAlreadyExistsException : ConflictException
{
    public RoomAlreadyExistsException(string number)
        : base($"Комната с таким номером '{number}' уже существует.", "number")
    {
    }

    public RoomAlreadyExistsException()
        : base("Комната с таким номером уже существует.", "number")
    {
    }
}