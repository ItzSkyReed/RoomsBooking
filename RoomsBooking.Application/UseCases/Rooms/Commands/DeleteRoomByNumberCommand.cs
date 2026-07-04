using MediatR;

namespace RoomsBooking.Application.UseCases.Rooms.Commands;

public record DeleteRoomByNumberCommand(string Number) : IRequest<Unit>;