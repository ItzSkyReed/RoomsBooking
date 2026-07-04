using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.API.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rooms")
            .WithTags("Room")
            .RequireAuthorization();

        group.MapPost("/", CreateRoomAsync)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces<RoomDto>()
            .WithSummary("Добавление новой переговорной комнаты")
            .WithDescription("Создает переговорную комнату, возвращает её путь в Location заголовке и сущность");

        group.MapGet("/{id:guid}", GetRoomByIdAsync)
            .WithName("GetRoomById")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<RoomDto>()
            .WithSummary("Получение информации о переговорной комнате  по Id");

        group.MapDelete("/{id:guid}", DeleteRoomByIdAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Удаление переговорной комнаты по Id");

        group.MapDelete("/{number}", DeleteRoomByNumberAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Удаление переговорной комнаты по номеру");
    }

    private static async Task<IResult> CreateRoomAsync(
        [FromBody] CreateRoomRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new CreateRoomCommand(request.Number, request.Description, request.Capacity, request.Floor);
        var response = await mediator.Send(command, ct);

        // Перенаправляем Location хедер на GetRoomByIdAsync и передаем UUID для поиска
        return Results.CreatedAtRoute("GetRoomById", new { id = response.Id }, response);
    }

    private static async Task<IResult> GetRoomByIdAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new GetRoomByIdCommand(id);
        var response = await mediator.Send(command, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteRoomByIdAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new DeleteRoomByIdCommand(id);
        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteRoomByNumberAsync(
        [FromRoute] string number,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new DeleteRoomByNumberCommand(number);
        await mediator.Send(command, ct);
        return Results.NoContent();
    }
}