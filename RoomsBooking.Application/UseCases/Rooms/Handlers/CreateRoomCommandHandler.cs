using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Application.UseCases.Rooms.Dtos;
using RoomsBooking.Application.UseCases.Rooms.Mappers;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class CreateRoomCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<CreateRoomCommand, RoomDto>
{
    public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var roomExists = await dbContext.Rooms
            .AnyAsync(r => r.Number == request.Number.Trim(), cancellationToken);

        if (roomExists)
            throw new RoomAlreadyExistsException(request.Number);

        var roomEntity = request.ToRoomEntity();

        dbContext.Rooms.Add(roomEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return roomEntity.ToRoomDto();
    }
}