using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public class RoomAlreadyBooked : ConflictException
{
    public RoomAlreadyBooked(Guid id)
        : base($"Комната c Id '{id}' уже зарезервирована на данное время.", "id")
    {
    }

    public RoomAlreadyBooked()
        : base("Выбранное время для этой комнаты пересекается с уже существующим бронированием.", "id")
    {
    }
}