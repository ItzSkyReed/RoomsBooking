using MediatR;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.Application.UseCases.Rooms.Commands;

public record GetRoomByIdCommand(Guid Id) : IRequest<RoomDto>;