using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoomsBooking.Application.UseCases.Authentication.Commands;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Application.Common.Authentication;

namespace RoomsBooking.API.Endpoints;

public static class AuthenticationEndpoints
{
    private const bool CookieSecureMode = false; // ! False пока нет HTTPS

    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapPost("/register", RegisterAsync)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces<AuthResponseDto>()
            .ProducesValidationProblem()
            .WithSummary("Регистрация нового пользователя")
            .WithDescription("Создает аккаунт, возвращает пользователя и access token, также устанавливает refresh token в Cookie");

        group.MapPost("/login", LoginAsync)
            .Produces<AuthResponseDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Вход в аккаунт")
            .WithDescription("Производит вход в аккаунт, возвращает пользователя и access token, также устанавливает refresh token в Cookie");

        group.MapPost("/refresh", RefreshAsync)
            .Produces<AccessTokenDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Обновление access/refresh токенов")
            .WithDescription(
                "Удаляет старый Refresh Token из БД сервера, возвращается новый access token, устанавливается новый refresh token в Cookie");

        group.MapPost("/logout", LogoutAsync)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Выход из аккаунта")
            .WithDescription("Производит удаление refresh token из Cookie и БД сервера")
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
        var (body, refreshToken) = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, refreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(body);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] ISender mediator,
        HttpContext httpContext,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var (body, refreshToken) = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, refreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(body);
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

        var (accessToken, newRefreshToken) = await mediator.Send(command, ct);

        SetRefreshTokenCookie(httpContext, newRefreshToken, jwtOptions.Value.RefreshTokenExpirationDays);

        return Results.Ok(accessToken);
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