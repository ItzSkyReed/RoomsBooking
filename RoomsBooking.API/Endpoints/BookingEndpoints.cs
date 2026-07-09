using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomsBooking.API.Requests;
using RoomsBooking.Application.Common.Dtos;
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
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/", BookRoomAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<BookingDto>(StatusCodes.Status201Created)
            .WithSummary("Бронирование комнаты")
            .WithDescription("Бронирование комнаты на определенный период времени");

        group.MapGet("/{id:guid}", GetBookingAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<BookingDto>()
            .WithName("GetBooking")
            .WithSummary("Информация о конкретном бронирование");

        group.MapGet("/", GetBookingsAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<PagedResponse<BookingDto>>()
            .WithSummary("Поиск и фильтрация бронирований");

        group.MapDelete("/{id:guid}", DeleteBookingAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .Produces<PagedResponse<BookingDto>>()
            .WithSummary("Удаление брони");

        group.MapDelete("/me", GetMyBookingsAsync)
            .ProducesValidationProblem()
            .Produces<List<BookingDto>>()
            .WithSummary("Все брони пользователя");
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

        var command = new BookRoomCommand(request.RoomId, userId, request.StartTime, request.EndTime);
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

    private static async Task<IResult> GetBookingsAsync(
        [AsParameters] GetBookingsRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new GetBookingsQuery(request.RoomId, request.UserId, request.FromTime, request.ToTime,
            request.SortBy, request.SortDescending, request.PageNumber, request.PageSize);

        var response = await mediator.Send(command, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteBookingAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new DeleteBookingCommand(id, userId);
        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> GetMyBookingsAsync(
        [FromServices] ISender mediator,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new GetUserBookingsQuery(userId);
        var response = await mediator.Send(command, ct);

        return Results.Ok(response);
    }
}