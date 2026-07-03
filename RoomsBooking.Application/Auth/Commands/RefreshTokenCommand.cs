using MediatR;
using RoomsBooking.Application.Auth.Dtos;

namespace RoomsBooking.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;