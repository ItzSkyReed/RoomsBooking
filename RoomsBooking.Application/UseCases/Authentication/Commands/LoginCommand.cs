using MediatR;
using RoomsBooking.Application.UseCases.Authentication.Dtos;

namespace RoomsBooking.Application.UseCases.Authentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<(AuthResponseDto Body, string RefreshToken)>;