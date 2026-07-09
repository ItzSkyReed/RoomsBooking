using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomsBooking.Application.UseCases.Users.Dtos;
using RoomsBooking.Application.UseCases.Users.Queries;
using RoomsBooking.Domain.Exceptions.Authentication;

namespace RoomsBooking.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", GetMeAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о своем профиле");

        group.MapGet("/{id:guid}", GetUserByIdAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о пользователе по UUID");

        group.MapGet("/{email}", GetUserByEmailAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о пользователе по Email");
    }

    private static async Task<IResult> GetMeAsync(
        [FromServices] ISender mediator,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var query = new GetUserByIdQuery(userId);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetUserByIdAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetUserByIdQuery(id);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetUserByEmailAsync(
        [FromRoute] string email,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetUserByEmailQuery(email);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }
}