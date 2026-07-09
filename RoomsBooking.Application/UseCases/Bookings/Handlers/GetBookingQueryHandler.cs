using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Mappers;
using RoomsBooking.Application.UseCases.Bookings.Queries;
using RoomsBooking.Domain.Exceptions.Bookings;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class GetBookingQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetBookingQuery, BookingDto>
{
    public async Task<BookingDto> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var bookingDto = await dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectToDto()
            .FirstOrDefaultAsync(cancellationToken);

        return bookingDto ?? throw new BookingNotFoundException(request.Id);
    }
}