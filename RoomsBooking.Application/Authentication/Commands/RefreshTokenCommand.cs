using MediatR;
using RoomsBooking.Application.Authentication.Dtos;

namespace RoomsBooking.Application.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;