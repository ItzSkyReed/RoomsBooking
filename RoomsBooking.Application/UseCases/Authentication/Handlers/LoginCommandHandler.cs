using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Authentication.Commands;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Domain.Entities;
using RoomsBooking.Domain.Exceptions.User;
using UserMapper = RoomsBooking.Application.UseCases.Users.Mappers.UserMapper;

namespace RoomsBooking.Application.UseCases.Authentication.Handlers;

public class LoginCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginCommand, (AuthResponseDto Body, string RefreshToken)>
{
    public async Task<(AuthResponseDto Body, string RefreshToken)> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

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

        return new ValueTuple<AuthResponseDto, string>(new AuthResponseDto(UserMapper.ToUserDto(user), accessToken), refreshToken);
    }
}