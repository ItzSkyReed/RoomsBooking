using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Room;

public class RoomHasActiveBookingsException : ConflictException
{
    public RoomHasActiveBookingsException(Guid id)
        : base($"Комната с Id '{id}' не может быть удалена, так как имеет активные бронирования.")
    {
    }

    public RoomHasActiveBookingsException(string number)
        : base($"Комната с номером '{number}' не может быть удалена, так как имеет активные бронирования.")
    {
    }
}