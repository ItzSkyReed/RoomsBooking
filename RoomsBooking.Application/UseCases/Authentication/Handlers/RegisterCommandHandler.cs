using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.UseCases.Authentication.Commands;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Domain.Entities;
using RoomsBooking.Domain.Exceptions.User;
using UserMapper = RoomsBooking.Application.UseCases.Users.Mappers.UserMapper;

namespace RoomsBooking.Application.UseCases.Authentication.Handlers;

public class RegisterCommandHandler(
    IAppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RegisterCommand, (AuthResponseDto Body, string RefreshToken)>
{
    public async Task<(AuthResponseDto Body, string RefreshToken)> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (emailExists)
            throw new UserAlreadyExistsException(request.Email);

        var hashedPassword = passwordHasher.Hash(request.Password);

        var user = new User(request.Email, hashedPassword, request.Name);
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var expirationDays = jwtOptions.Value.RefreshTokenExpirationDays;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, expiresAt);

        dbContext.RefreshTokens.Add(refreshTokenEntity);
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ValueTuple<AuthResponseDto, string>(new AuthResponseDto(UserMapper.ToUserDto(user), accessToken), refreshToken);
    }
}