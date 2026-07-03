using MediatR;
using RoomsBooking.Application.Auth.Dtos;

namespace RoomsBooking.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;