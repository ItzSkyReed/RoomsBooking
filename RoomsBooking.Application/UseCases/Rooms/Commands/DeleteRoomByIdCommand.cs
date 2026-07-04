using MediatR;

namespace RoomsBooking.Application.UseCases.Rooms.Commands;

public record DeleteRoomByIdCommand(Guid Id) : IRequest<Unit>;