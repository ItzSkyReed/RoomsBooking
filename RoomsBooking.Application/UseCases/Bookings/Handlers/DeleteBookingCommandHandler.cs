using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Domain.Exceptions.Bookings;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class DeleteBookingCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteBookingCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings.SingleOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
            return Unit.Value;

        if (booking.UserId != request.UserId)
            throw new BookingForbiddenAccessException(request.BookingId);

        dbContext.Bookings.Remove(booking);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}