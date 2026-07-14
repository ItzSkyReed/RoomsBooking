using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomsBooking.API.Requests;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Tests.IntegrationTests.Helpers;
using RoomsBooking.Tests.Setup;

namespace RoomsBooking.Tests.IntegrationTests.Authentication;

[Collection("SharedTestCollection")]
public class RegisterTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_WithValidData_ShouldReturn200OkAndSetCookie()
    {
        var request = TestDataFactory.GenerateRegisterRequest();

        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authData = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        authData.Should().NotBeNull();
        authData.AccessToken.Should().NotBeNullOrEmpty();

        authData.User.Should().NotBeNull();
        authData.User.Email.Should().Be(request.Email);
        authData.User.Name.Should().Be(request.Name);
        authData.User.Id.Should().NotBeEmpty();

        // Проверяем, что установилась кука с refresh-токеном и она HttpOnly
        var setCookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();
        setCookieHeaders.Should().Contain(c => c.StartsWith("refreshToken="));
        setCookieHeaders.Should().Contain(c => c.Contains("HttpOnly"));
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturn409Conflict()
    {
        var request = TestDataFactory.GenerateRegisterRequest();

        await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturn400BadRequest()
    {
        var request = new RegisterRequest(
            Email: "invalid-email-format",
            Password: "ValidPassword123!",
            Name: "Test User"
        );

        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithShortPassword_ShouldReturn400BadRequest()
    {
        var request = new RegisterRequest(
            Email: "test@example.com",
            Password: "short", // Меньше 8 символов
            Name: "Test User"
        );

        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithEmptyName_ShouldReturn400BadRequest()
    {
        var request = new RegisterRequest(
            Email: "test@example.com",
            Password: "ValidPassword123!",
            Name: "" // Пустое имя
        );

        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}