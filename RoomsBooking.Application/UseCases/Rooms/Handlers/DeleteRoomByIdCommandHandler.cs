using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class DeleteRoomByIdCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteRoomByIdCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomByIdCommand request, CancellationToken cancellationToken)
    {
        var hasActiveBookings = await dbContext.Bookings
            .AnyAsync(b => b.RoomId == request.Id && b.EndTime > DateTimeOffset.UtcNow, cancellationToken);

        if (hasActiveBookings)
            throw new RoomHasActiveBookingsException(request.Id);

        await dbContext.Rooms.Where(r => r.Id == request.Id).ExecuteDeleteAsync(cancellationToken);

        return Unit.Value;
    }
}