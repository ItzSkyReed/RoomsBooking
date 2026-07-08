using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Authentication.Commands;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Domain.Entities;
using RoomsBooking.Domain.Exceptions.Authentication;
using RoomsBooking.Domain.Exceptions.User;
using RoomsBooking.Domain.Interfaces;

namespace RoomsBooking.Application.UseCases.Authentication.Handlers;

public class RefreshTokenCommandHandler(
    IAppDbContext dbContext,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions,
    ITokenHasher tokenHasher)
    : IRequestHandler<RefreshTokenCommand, (AccessTokenDto accessToken, string RefreshToken)>
{
    public async Task<(AccessTokenDto accessToken, string RefreshToken)> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingRefreshTokenHash = tokenHasher.Hash(request.RefreshToken);

        var oldTokenHash = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == incomingRefreshTokenHash, cancellationToken);

        if (oldTokenHash == null || !oldTokenHash.IsValid(request.RefreshToken, tokenHasher))
            throw new InvalidRefreshTokenException();


        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == oldTokenHash.UserId, cancellationToken);

        if (user == null)
            throw new UserNotFoundException();

        // Ротация
        dbContext.RefreshTokens.Remove(oldTokenHash);

        var newAccessToken = jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = jwtProvider.GenerateRefreshToken();

        var expiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays);
        var newRefreshTokenEntity = new RefreshToken(user.Id, newRefreshToken, expiresAt, tokenHasher);

        dbContext.RefreshTokens.Add(newRefreshTokenEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ValueTuple<AccessTokenDto, string>(new AccessTokenDto(newAccessToken), newRefreshToken);
    }
}