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
        var cleanNumber = request.Number.Trim();

        var hasActiveBookings = await dbContext.Bookings
            .AnyAsync(b => b.Room.Number == cleanNumber && b.EndTime > DateTimeOffset.UtcNow, cancellationToken);

        if (hasActiveBookings)
            throw new RoomHasActiveBookingsException(cleanNumber);

        await dbContext.Rooms.Where(r => r.Number == cleanNumber).ExecuteDeleteAsync(cancellationToken);

        return Unit.Value;
    }
}