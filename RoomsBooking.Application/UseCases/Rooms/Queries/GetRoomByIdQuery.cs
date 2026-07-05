using MediatR;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.Application.UseCases.Rooms.Queries;

public record GetRoomByIdQuery(Guid Id) : IRequest<RoomDto>;