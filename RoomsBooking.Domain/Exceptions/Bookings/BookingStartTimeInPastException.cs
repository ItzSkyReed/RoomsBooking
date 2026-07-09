using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public sealed class BookingStartTimeInPastException()
    : DomainBadRequestException("Время начала бронирования должно быть в будущем.", "startTime");