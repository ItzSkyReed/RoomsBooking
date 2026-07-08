using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Bookings;

public class BookingNotFoundException(Guid id) : NotFoundException($"Бронирование с Id {id} не найдено.");