using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.API.Middleware;

public partial class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
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
                problemDetails.Status = badHttpRequestException.StatusCode; // Обычно 400 Bad Request
                problemDetails.Detail = "Запрос имеет неверный формат (синтаксическая ошибка в JSON или некорректные заголовки).";
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

            // Ошибки прав
            case ForbiddenException forbiddenException:
                problemDetails.Title = "Недостаточно прав для совершения действия";
                problemDetails.Status = StatusCodes.Status403Forbidden;
                problemDetails.Detail = forbiddenException.Message;
                break;

            case ConflictException conflictException:
                problemDetails.Title = "Конфликт бизнес-логики";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = conflictException.Message;

                if (conflictException.Field != null)
                    problemDetails.Extensions["fields"] = new[] { conflictException.Field };
                break;

            case DomainBadRequestException badRequestException:
                problemDetails.Title = "Некорректный запрос";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = badRequestException.Message;

                if (badRequestException.Field != null)
                    problemDetails.Extensions["errors"] = new[] { badRequestException.Field };
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