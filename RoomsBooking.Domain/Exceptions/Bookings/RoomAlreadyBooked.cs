using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public class RoomAlreadyBooked(Guid id) : ConflictException($"Комната c Id '{id}' уже зарезервирована на данное время.");