using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.Auth.Commands;
using RoomsBooking.Application.Auth.Dtos;
using RoomsBooking.Application.Common.Authentication;

namespace RoomsBooking.API.Endpoints;

public static class AuthEndpoints
{
    private const bool CookieSecureMode = false; // ! False пока нет HTTPS

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication")
            .WithDisplayName("Аутентификация")
            .WithDescription("Эндпоинты для управления сессиями пользователей, регистрации и обновления токенов.");

        group.MapPost("/register", RegisterAsync)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces<AccessTokenDto>()
            .ProducesValidationProblem()
            .WithSummary("Регистрация нового пользователя")
            .WithDescription("Создает аккаунт и сразу возвращает Access Token и устанавливается Refresh Token в Cookie");

        group.MapPost("/login", LoginAsync)
            .Produces<AccessTokenDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Вход в аккаунт")
            .WithDescription("Производится вход в аккаунт, возвращается Access Token и устанавливается Refresh Token в Cookie");

        group.MapPost("/refresh", RefreshAsync)
            .Produces<AccessTokenDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Обновление access/refresh токенов")
            .WithDescription(
                "Производится удаление старого Refresh Token из БД сервера, возвращается новый Access Token, устанавливается новый Refresh Token в Cookie");

        group.MapPost("/logout", LogoutAsync)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Выход из аккаунта")
            .WithDescription("Производится удаление Refresh Token из Cookie и БД сервера")
            .RequireAuthorization();
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterRequest request,
        [FromServices] ISender mediator,
        HttpContext httpContext,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.Name);
        var authResponse = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, authResponse.RefreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(new AccessTokenDto(authResponse.AccessToken));
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] ISender mediator,
        HttpContext httpContext,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var authResponse = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, authResponse.RefreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(new AccessTokenDto(authResponse.AccessToken));
    }

    private static async Task<IResult> RefreshAsync(
        HttpContext httpContext,
        [FromServices] ISender mediator,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Results.Unauthorized();

        var command = new RefreshTokenCommand(refreshToken);

        var authResponse = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, authResponse.RefreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(new AccessTokenDto(authResponse.AccessToken));
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext httpContext,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        // Если кука с рефреш-токеном есть, удаляем токен из базы
        if (httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            var command = new LogoutCommand(refreshToken);
            await mediator.Send(command, ct);
        }

        httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = CookieSecureMode,
            SameSite = SameSiteMode.Strict
        });

        return Results.NoContent();
    }


    private static void SetRefreshTokenCookie(HttpContext context, string refreshToken, int expireDays)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = CookieSecureMode,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(expireDays)
        };

        context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}