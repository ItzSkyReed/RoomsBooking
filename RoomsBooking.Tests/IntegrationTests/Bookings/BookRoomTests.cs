using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomsBooking.API.Requests;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Tests.IntegrationTests.Helpers;
using RoomsBooking.Tests.Setup;

namespace RoomsBooking.Tests.IntegrationTests.Bookings;

// Указываем xUnit, что используем фабрику
[Collection("SharedTestCollection")]
public class BookRoomTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    // Успешное создание
    [Fact]
    public async Task BookRoom_WithValidData_ShouldReturn201Created()
    {
        await Client.RegisterAndLoginAsNewUserAsync();
        var roomId = await Client.CreateTestRoomAsync();

        var startTime = DateTimeOffset.UtcNow.AddDays(1);
        var req = new BookRoomRequest(roomId, startTime, startTime.AddHours(2));

        var response = await Client.PostAsJsonAsync("/api/v1/bookings", req);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var bookingDto = await response.Content.ReadFromJsonAsync<BookingDto>();
        bookingDto.Should().NotBeNull();
        bookingDto.RoomId.Should().Be(roomId);
    }

    // Граничное условие
    [Fact]
    public async Task BookRoom_BackToBack_ShouldReturn201Created()
    {
        await Client.RegisterAndLoginAsNewUserAsync();
        var roomId = await Client.CreateTestRoomAsync();

        var timeA = DateTimeOffset.UtcNow.AddDays(1);
        var timeB = timeA.AddHours(1);
        var timeC = timeB.AddHours(1);

        // Бронь 1 с 12:00 до 13:00
        await Client.PostAsJsonAsync("/api/v1/bookings", new BookRoomRequest(roomId, timeA, timeB));

        // Бронь 2 с 13:00 до 14:00 (начинается ровно в момент окончания первой)
        var response = await Client.PostAsJsonAsync("/api/v1/bookings", new BookRoomRequest(roomId, timeB, timeC));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // Слишком короткая брань
    [Fact]
    public async Task BookRoom_DurationLessThan10Minutes_ShouldReturn400BadRequest()
    {
        await Client.RegisterAndLoginAsNewUserAsync();
        var roomId = Guid.NewGuid();

        var startTime = DateTimeOffset.UtcNow.AddDays(1);
        var endTime = startTime.AddMinutes(5); // Меньше 10 минут

        var req = new BookRoomRequest(roomId, startTime, endTime);
        var response = await Client.PostAsJsonAsync("/api/v1/bookings", req);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Несуществующая комната
    [Fact]
    public async Task BookRoom_WithNonExistentRoom_ShouldReturn404NotFound()
    {
        await Client.RegisterAndLoginAsNewUserAsync();
        var fakeRoomId = Guid.NewGuid();

        var startTime = DateTimeOffset.UtcNow.AddDays(1);
        var req = new BookRoomRequest(fakeRoomId, startTime, startTime.AddHours(1));

        var response = await Client.PostAsJsonAsync("/api/v1/bookings", req);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Без токена
    [Fact]
    public async Task BookRoom_WithoutToken_ShouldReturn401Unauthorized()
    {
        var req = new BookRoomRequest(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(1).AddHours(1));
        var response = await Client.PostAsJsonAsync("/api/v1/bookings", req);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Пересечение броней
    [Fact]
    public async Task BookRoom_WithOverlappingTime_ShouldReturn409Conflict()
    {
        await Client.RegisterAndLoginAsNewUserAsync();
        var roomId = await Client.CreateTestRoomAsync();

        var startTime1 = DateTimeOffset.UtcNow.AddDays(1);
        var endTime1 = startTime1.AddHours(2);

        // Создаем первую успешную бронь
        var firstBookingReq = new BookRoomRequest(roomId, startTime1, endTime1);
        var setupResponse = await Client.PostAsJsonAsync("/api/v1/bookings", firstBookingReq);
        setupResponse.EnsureSuccessStatusCode();

        // Формируем конфликтную бронь (сдвигаем старт на час)
        var overlappingReq = new BookRoomRequest(
            roomId,
            startTime1.AddHours(1),
            startTime1.AddHours(3));

        var response = await Client.PostAsJsonAsync("/api/v1/bookings", overlappingReq);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}