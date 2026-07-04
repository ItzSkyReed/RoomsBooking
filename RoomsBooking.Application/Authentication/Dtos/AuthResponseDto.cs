namespace RoomsBooking.Application.Authentication.Dtos;

public record AuthResponseDto(string AccessToken, string RefreshToken);