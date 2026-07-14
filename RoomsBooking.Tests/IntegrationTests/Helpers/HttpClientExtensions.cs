using System.Net.Http.Headers;
using System.Net.Http.Json;
using RoomsBooking.Application.UseCases.Authentication.Dtos;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.Tests.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        // Регистрирует юзера, логинит его и сразу вешает токен на HttpClient
        public async Task<Guid> RegisterAndLoginAsNewUserAsync()
        {
            var request = TestDataFactory.GenerateRegisterRequest();

            var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);
            response.EnsureSuccessStatusCode();

            var authData = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            // Устанавливаем токен
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authData!.AccessToken);

            return authData.User.Id;
        }

        // Создает комнату и возвращает её ID
        public async Task<Guid> CreateTestRoomAsync(string? number = null)
        {
            var request = TestDataFactory.GenerateRoomRequest(number);

            var response = await client.PostAsJsonAsync("/api/v1/rooms", request);
            response.EnsureSuccessStatusCode();

            var roomData = await response.Content.ReadFromJsonAsync<RoomDto>();
            return roomData!.Id;
        }
    }
}