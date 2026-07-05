using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Dtos;
using RoomsBooking.Application.UseCases.Rooms.Mappers;
using RoomsBooking.Application.UseCases.Rooms.Queries;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class GetRoomByIdQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetRoomByIdQuery, RoomDto>
{
    public async Task<RoomDto> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var roomEntity = await dbContext.Rooms.AsNoTracking().SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return roomEntity is null
            ? throw new RoomNotFoundException(request.Id)
            : roomEntity.ToRoomDto();
    }
}