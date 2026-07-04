namespace RoomsBooking.Application.UseCases.Users.Dtos;

public record UserDto(Guid Id, string Email,string Name, DateTimeOffset CreatedAt);
