using MediatR;
using RoomsBooking.Application.UseCases.Authentication.Dtos;

namespace RoomsBooking.Application.UseCases.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(AccessTokenDto accessToken, string RefreshToken)>;