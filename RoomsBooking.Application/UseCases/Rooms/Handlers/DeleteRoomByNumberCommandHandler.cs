using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class DeleteRoomByNumberCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteRoomByNumberCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomByNumberCommand request, CancellationToken cancellationToken)
    {
        var rows = await dbContext.Rooms.Where(r => r.Number == request.Number.Trim()).ExecuteDeleteAsync(cancellationToken);
        return rows == 0
            ? throw new RoomNotFoundException(request.Number) // Если мы хотим идемпотентности, то независимо от результата возвращаем Unit
            : Unit.Value;
    }
}