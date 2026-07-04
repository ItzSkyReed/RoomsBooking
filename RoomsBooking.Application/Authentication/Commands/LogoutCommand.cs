using MediatR;

namespace RoomsBooking.Application.Authentication.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;