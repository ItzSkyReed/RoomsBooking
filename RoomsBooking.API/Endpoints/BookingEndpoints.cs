using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomsBooking.API.Requests;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Queries;
using RoomsBooking.Domain.Exceptions.Authentication;

namespace RoomsBooking.API.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bookings")
            .WithTags("Bookings")
            .RequireAuthorization();

        group.MapPost("/", BookRoomAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<BookingDto>(StatusCodes.Status201Created)
            .WithSummary("Бронирование комнаты")
            .WithDescription("Бронирование комнаты на определенный период времени");

        group.MapGet("/{id:guid}", GetBookingAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<BookingDto>()
            .WithName("GetBooking")
            .WithSummary("Информация о конкретном бронирование");
    }

    private static async Task<IResult> BookRoomAsync(
        [FromBody] BookRoomRequest request,
        [FromServices] ISender mediator,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new BookRoomCommand(request.roomId, userId, request.startTime, request.endTime);
        var response = await mediator.Send(command, ct);

        return Results.CreatedAtRoute("GetBooking", new { id = response.Id }, response);
    }

    private static async Task<IResult> GetBookingAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new GetBookingQuery(id);
        var response = await mediator.Send(command, ct);

        return Results.Ok(response);
    }
}