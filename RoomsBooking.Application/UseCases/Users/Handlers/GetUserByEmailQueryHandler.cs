using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Users.Dtos;
using RoomsBooking.Application.UseCases.Users.Mappers;
using RoomsBooking.Application.UseCases.Users.Queries;
using RoomsBooking.Domain.Exceptions.User;

namespace RoomsBooking.Application.UseCases.Users.Handlers;

public class GetUserByEmailQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(r => r.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        return userEntity is null
            ? throw new UserNotFoundException(request.Email)
            : userEntity.ToUserDto();
    }
}