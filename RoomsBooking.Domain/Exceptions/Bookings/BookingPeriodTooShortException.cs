using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public sealed class BookingPeriodTooShortException(int minutes)
    : DomainBadRequestException($"Интервал бронирования слишком короткий. Минимальное время: {minutes} минут.", "endTime");