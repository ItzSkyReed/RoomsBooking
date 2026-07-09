using MediatR;

namespace RoomsBooking.Application.UseCases.Bookings.Commands;

public record PatchBookingCommand(Guid Id, Guid UserId, Guid? RoomId, DateTimeOffset? StartTime, DateTimeOffset? EndTime) : IRequest<Unit>;