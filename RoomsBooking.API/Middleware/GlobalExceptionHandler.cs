using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RoomsBooking.Domain.Exceptions.Base;
using RoomsBooking.Infrastructure.Persistence;

namespace RoomsBooking.API.Middleware;

public partial class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    UniqueConstraintResolver constraintResolver) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            // Ошибки самого HTTP-запроса (невалидный JSON, неверные кавычки)
            case BadHttpRequestException badHttpRequestException:
                problemDetails.Title = "Некорректный HTTP-запрос";
                problemDetails.Status = badHttpRequestException.StatusCode;
                problemDetails.Detail = badHttpRequestException.InnerException?.Message ?? badHttpRequestException.Message;
                break;

            // Ошибки валидации от FluentValidation
            case ValidationException validationException:
                problemDetails.Title = "Ошибка валидации запроса";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = "Один или несколько параметров не прошли проверку.";
                problemDetails.Extensions["errors"] = validationException.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage });
                break;

            // Неверный пароль, обращение к защищенным эндпоинтам без Access Token
            case UnauthorizedException unauthorizedException:
                problemDetails.Title = "В доступе отказано";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Detail = unauthorizedException.Message;
                break;

            // Ошибки отсутствия данных (UserNotFound, RoomNotFound и т.д.)
            case NotFoundException notFoundException:
                problemDetails.Title = "Сущность не найдена";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Detail = notFoundException.Message;
                break;

            // Ошибки конфликтов (Пользователь существует, время занято)
            case ConflictException conflictException:
                problemDetails.Title = "Конфликт бизнес-логики";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = conflictException.Message; // Текст из конкретной ошибки
                break;

            case DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pgEx }:

                problemDetails.Title = "Конфликт данных";
                problemDetails.Status = StatusCodes.Status409Conflict;

                if (constraintResolver.TryGetFields(pgEx.ConstraintName!, out var fields))
                {
                    problemDetails.Detail = $"Значение для поля(полей) {string.Join(", ", fields)} уже существует.";
                    problemDetails.Extensions["fields"] = fields;
                }
                else
                    problemDetails.Detail = "Запись с такими параметрами уже существует.";

                break;

            case DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation } pgEx }:
                problemDetails.Title = "Конфликт бронирования";
                problemDetails.Status = StatusCodes.Status409Conflict;

                if (pgEx.ConstraintName == "EXCLUDE_overlapping_bookings")
                    problemDetails.Detail = "Выбранное время для этой комнаты пересекается с уже существующим бронированием.";
                else
                    problemDetails.Detail = "Произошло недопустимое пересечение данных.";
                break;

            // Все остальные непредвиденные ошибки
            default:
                LogUnexpectedHandlerException(exception.Message, exception); // Логируем именно непредвиденные
                problemDetails.Title = "Внутренняя ошибка сервера";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "Произошла непредвиденная ошибка на сервере.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // Отправляем JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    [LoggerMessage(LogLevel.Error, "Произошла непредвиденная ошибка: {Message}")]
    partial void LogUnexpectedHandlerException(string message, Exception exception);
}