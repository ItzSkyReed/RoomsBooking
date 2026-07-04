using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Authentication.Dtos;
using RoomsBooking.Application.Authentication.Commands;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Domain.Entities;
using RoomsBooking.Domain.Exceptions.User;
namespace RoomsBooking.Application.Authentication.Handlers;

public class LoginCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null)
            throw new UserNotFoundException(request.Email);

        var isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new WrongUserPasswordException(request.Email);

        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var expirationDays = jwtOptions.Value.RefreshTokenExpirationDays;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, expiresAt);

        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(accessToken, refreshToken);
    }
}