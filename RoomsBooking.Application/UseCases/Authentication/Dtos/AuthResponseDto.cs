using RoomsBooking.Application.UseCases.Users.Dtos;

namespace RoomsBooking.Application.UseCases.Authentication.Dtos;

public record AuthResponseDto(UserDto User, string AccessToken);