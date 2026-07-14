using Bogus;
using RoomsBooking.API.Requests;

namespace RoomsBooking.Tests.IntegrationTests.Helpers;

public static class TestDataFactory
{
    public static RegisterRequest GenerateRegisterRequest(string? email = null)
    {
        var faker = new Faker();
        return new RegisterRequest(
            Email: email ?? faker.Internet.Email(),
            Password: "Password123!",
            Name: faker.Name.FullName()
        );
    }

    public static CreateRoomRequest GenerateRoomRequest(string? number = null)
    {
        var faker = new Faker();
        return new CreateRoomRequest(
            Number: number ?? faker.Random.AlphaNumeric(6).ToUpper(),
            Description: faker.Lorem.Sentence(),
            Capacity: faker.Random.Short(10, 50),
            Floor: faker.Random.Short(1, 10)
        );
    }
}