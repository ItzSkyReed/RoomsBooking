using MediatR;

namespace RoomsBooking.Application.UseCases.Bookings.Dtos;

public record BookingDto(Guid Id, Guid RoomId, Guid UserId, string UserName, DateTimeOffset StartTime, DateTimeOffset EndTime) : IRequest<BookingDto>;