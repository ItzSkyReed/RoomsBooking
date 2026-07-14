using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomsBooking.API.Requests;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Tests.IntegrationTests.Helpers;
using RoomsBooking.Tests.Setup;

namespace RoomsBooking.Tests.IntegrationTests.Authentication;

[Collection("SharedTestCollection")]
public class LoginTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200OkAndSetCookie()
    {
        // регистрируем пользователя, чтобы он появился в БД
        var registerReq = TestDataFactory.GenerateRegisterRequest();
        await Client.PostAsJsonAsync("/api/v1/auth/register", registerReq);

        var loginReq = new LoginRequest(registerReq.Email, registerReq.Password);

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginReq);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authData = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        authData.Should().NotBeNull();
        authData.AccessToken.Should().NotBeNullOrEmpty();

        authData.User.Should().NotBeNull();
        authData.User.Email.Should().Be(registerReq.Email);

        // Проверяем, что сервер вернул куки с новым refresh-токеном
        var setCookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();
        setCookieHeaders.Should().Contain(c => c.StartsWith("refreshToken=", StringComparison.OrdinalIgnoreCase));
        setCookieHeaders.Should().Contain(c => c.Contains("HttpOnly", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn401Unauthorized()
    {
        var registerReq = TestDataFactory.GenerateRegisterRequest();
        await Client.PostAsJsonAsync("/api/v1/auth/register", registerReq);

        // Пытаемся войти с правильным email, но неправильным паролем
        var loginReq = new LoginRequest(registerReq.Email, "WrongPassword123!");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginReq);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ShouldReturn404NotFound()
    {
        // Используем email, которого нет в базе
        var loginReq = new LoginRequest("notfound@example.com", "SomePassword123!");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginReq);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ShouldReturn400BadRequest()
    {
        var loginReq = new LoginRequest("invalid-email-format", "SomePassword123!");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginReq);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturn400BadRequest()
    {
        var loginReq = new LoginRequest("test@example.com", "");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginReq);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}