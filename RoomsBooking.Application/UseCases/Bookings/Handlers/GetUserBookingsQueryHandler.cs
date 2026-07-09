using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Mappers;
using RoomsBooking.Application.UseCases.Bookings.Queries;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class GetUserBookingsQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetUserBookingsQuery, List<BookingDto>>
{
    public async Task<List<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookingDtos = await dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .ProjectToDto()
            .ToListAsync(cancellationToken);

        return bookingDtos;
    }
}