using Riok.Mapperly.Abstractions;
using RoomsBooking.Application.UseCases.Users.Dtos;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Users.Mappers;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.PasswordHash))]
    public static partial UserDto ToUserDto(this User entity);
}