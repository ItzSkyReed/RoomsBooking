using MediatR;
using RoomsBooking.Application.UseCases.Users.Dtos;

namespace RoomsBooking.Application.UseCases.Users.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto>;