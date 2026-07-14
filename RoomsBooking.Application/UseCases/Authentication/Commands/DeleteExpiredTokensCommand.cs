using MediatR;

namespace RoomsBooking.Application.UseCases.Authentication.Commands;

public record DeleteExpiredTokensCommand : IRequest<Unit>;