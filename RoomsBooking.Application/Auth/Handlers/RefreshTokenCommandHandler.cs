
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Auth.Commands;
using RoomsBooking.Application.Auth.Dtos;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Domain.Entities;
using RoomsBooking.Domain.Exceptions;
using RoomsBooking.Domain.Exceptions.User;

namespace RoomsBooking.Application.Auth.Handlers;

public class RefreshTokenCommandHandler(
    IAppDbContext dbContext,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldTokenEntity = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (oldTokenEntity == null || oldTokenEntity.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new InvalidRefreshTokenException();


        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == oldTokenEntity.UserId, cancellationToken);

        if (user == null)
            throw new UserNotFoundException();


        // Ротация
        dbContext.RefreshTokens.Remove(oldTokenEntity);

        var newAccessToken = jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = jwtProvider.GenerateRefreshToken();

        var expiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays);
        var newRefreshTokenEntity = new RefreshToken(user.Id, newRefreshToken, expiresAt);

        dbContext.RefreshTokens.Add(newRefreshTokenEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(newAccessToken, newRefreshToken);
    }
}