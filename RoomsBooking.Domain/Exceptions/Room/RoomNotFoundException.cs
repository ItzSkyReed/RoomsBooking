using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Room;

public class RoomNotFoundException : NotFoundException
{
    public RoomNotFoundException(Guid id)
        : base($"Комната с Id {id} не найдена.")
    {
    }

    public RoomNotFoundException(string number)
        : base($"Комната с номером {number} не найдена.")
    {
    }
}