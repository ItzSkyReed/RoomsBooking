using RoomsBooking.Application.UseCases.Bookings.Queries;

namespace RoomsBooking.API.Requests;

public record BookRoomRequest(Guid RoomId, DateTimeOffset StartTime, DateTimeOffset EndTime);

public record GetBookingsRequest(
    Guid? RoomId,
    Guid? UserId,
    DateTimeOffset? FromTime,
    DateTimeOffset? ToTime,
    BookingsSortBy? SortBy,
    bool SortDescending = false,
    int PageNumber = 1,
    int PageSize = 20
);