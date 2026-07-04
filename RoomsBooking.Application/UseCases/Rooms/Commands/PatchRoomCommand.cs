using MediatR;

namespace RoomsBooking.Application.UseCases.Rooms.Commands;

public record PatchRoomCommand(Guid Id, string? Number, string? Description, bool IsDescriptionSet, short? Capacity, short? Floor)
    : IRequest<Unit>;