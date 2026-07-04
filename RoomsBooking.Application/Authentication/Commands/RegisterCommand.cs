using MediatR;
using RoomsBooking.Application.Authentication.Dtos;

namespace RoomsBooking.Application.Authentication.Commands;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<AuthResponseDto>;