using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Room;

public class RoomNotFoundException(Guid Id) : NotFoundException($"Комната c {Id} не найдена.");