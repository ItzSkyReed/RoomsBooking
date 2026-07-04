using MediatR;

namespace RoomsBooking.Application.UseCases.Authentication.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;