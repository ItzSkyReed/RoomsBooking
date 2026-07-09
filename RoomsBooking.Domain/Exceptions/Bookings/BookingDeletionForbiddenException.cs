using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public class BookingDeletionForbiddenException(Guid BookingId) : ForbiddenException($"Недостаточно прав для удаления бронирования {BookingId}.");