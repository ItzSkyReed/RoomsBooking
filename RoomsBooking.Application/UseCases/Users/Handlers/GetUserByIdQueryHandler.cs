using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Users.Dtos;
using RoomsBooking.Application.UseCases.Users.Mappers;
using RoomsBooking.Application.UseCases.Users.Queries;
using RoomsBooking.Domain.Exceptions.User;

namespace RoomsBooking.Application.UseCases.Users.Handlers;

public class GetUserByIdQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return userEntity is null
            ? throw new UserNotFoundException(request.Id)
            : userEntity.ToUserDto();
    }
}