using MediatR;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.Application.UseCases.Rooms.Commands;

public record CreateRoomCommand(string Number, string? Description, short Capacity, short Floor) : IRequest<RoomDto>;