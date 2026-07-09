using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public class BookingForbiddenAccessException(Guid bookingId)
    : ForbiddenException($"Недостаточно прав для изменения бронирования {bookingId}.");