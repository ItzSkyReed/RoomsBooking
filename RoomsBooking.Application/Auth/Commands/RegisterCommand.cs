using MediatR;
using RoomsBooking.Application.Auth.Dtos;

namespace RoomsBooking.Application.Auth.Commands;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<AuthResponseDto>;