namespace RoomsBooking.API.Requests;

public record BookRoomRequest(Guid roomId, DateTimeOffset startTime, DateTimeOffset endTime);