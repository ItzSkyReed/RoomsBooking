using MediatR;
using RoomsBooking.Application.UseCases.Bookings.Dtos;

namespace RoomsBooking.Application.UseCases.Bookings.Commands;

public record BookRoomCommand(Guid RoomId, Guid UserId, DateTimeOffset StartTime, DateTimeOffset EndTime) : IRequest<BookingDto>;