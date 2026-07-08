using Riok.Mapperly.Abstractions;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Bookings.Mappers;

[Mapper]
public static partial class BookingMapper
{
    public static partial Booking ToBookingEntity(this BookRoomCommand command);


    [MapperIgnoreSource(nameof(Booking.User))]
    [MapperIgnoreSource(nameof(Booking.Room))]
    public static partial BookingDto ToBookingDto(this Booking entity, string userName);

    public static partial IQueryable<BookingDto> ProjectToDto(this IQueryable<Booking> query);

    // Существует для использования ProjectToDto
    [MapProperty("User.Name", "UserName")]
    [MapperIgnoreSource(nameof(Booking.Room))]
    public static partial BookingDto ToBookingDto(this Booking entity);
}