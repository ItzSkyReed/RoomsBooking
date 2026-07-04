using MediatR;
using RoomsBooking.Application.UseCases.Authentication.Dtos;

namespace RoomsBooking.Application.UseCases.Authentication.Commands;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<(AuthResponseDto Body, string RefreshToken)>;