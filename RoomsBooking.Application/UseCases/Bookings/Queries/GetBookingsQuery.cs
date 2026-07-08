using MediatR;
using RoomsBooking.Application.Common.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Dtos;

namespace RoomsBooking.Application.UseCases.Bookings.Queries;

public enum BookingsSortBy
{
    StarTime = 1,
    EndTime = 2
}

public record GetBookingsQuery(
    Guid? RoomId,
    Guid? UserId,
    DateTimeOffset? FromTime,
    DateTimeOffset? ToTime,
    BookingsSortBy? SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize
) : IRequest<PagedResponse<BookingDto>>;