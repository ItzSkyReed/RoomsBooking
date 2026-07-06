using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class PatchRoomCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<PatchRoomCommand, Unit>
{
    public async Task<Unit> Handle(PatchRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await dbContext.Rooms
            .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (room is null)
            throw new RoomNotFoundException(request.Id);

        if (request.Number != null && !request.Number.Trim().Equals(room.Number, StringComparison.CurrentCultureIgnoreCase))
        {
            var isNumberUnique = await dbContext.Rooms.Where(r => r.Number == request.Number.Trim()).AnyAsync(cancellationToken);
            if (isNumberUnique)
                throw new RoomAlreadyExistsException(request.Number);
        }

        room.UpdateDetails(request.Number, request.Description, request.IsDescriptionSet, request.Capacity, request.Floor);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}