using MediatR;
using RoomsBooking.Application.Authentication.Dtos;

namespace RoomsBooking.Application.Authentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;