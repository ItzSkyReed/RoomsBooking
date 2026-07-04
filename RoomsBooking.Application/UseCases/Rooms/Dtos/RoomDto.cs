namespace RoomsBooking.Application.UseCases.Rooms.Dtos;

public record RoomDto(Guid Id, string Number, string? Description, short Capacity, short Floor);