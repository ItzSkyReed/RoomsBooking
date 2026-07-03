using MediatR;

namespace RoomsBooking.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;