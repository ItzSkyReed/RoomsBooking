namespace RoomsBooking.Tests.Setup;

[CollectionDefinition("SharedTestCollection")]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    // класс служит только для xUnit, чтобы связать
    // строку "SharedTestCollection" с классом CustomWebApplicationFactory.
}