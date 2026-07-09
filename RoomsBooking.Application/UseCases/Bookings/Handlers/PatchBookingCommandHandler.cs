using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Domain.Exceptions.Bookings;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class PatchBookingCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<PatchBookingCommand, Unit>
{
    public async Task<Unit> Handle(PatchBookingCommand request, CancellationToken cancellationToken)
    {
        var bookingEntity = await dbContext.Bookings.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (bookingEntity == null)
            throw new BookingNotFoundException(request.Id);

        if (bookingEntity.UserId != request.UserId)
            throw new BookingForbiddenAccessException(request.Id);

        var isOverlap = await dbContext.Bookings.AsNoTracking()
            .Where(b => b.RoomId == request.RoomId)
            .AnyAsync(b => request.StartTime < b.EndTime && b.StartTime < request.EndTime, cancellationToken);

        if (isOverlap)
            throw new RoomAlreadyBooked();

        bookingEntity.UpdateDetails(request.RoomId, request.StartTime, request.EndTime);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}