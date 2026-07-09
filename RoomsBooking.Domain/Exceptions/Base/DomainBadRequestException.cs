namespace RoomsBooking.Domain.Exceptions.Base;

public abstract class DomainBadRequestException(string message, string? field = null) : Exception(message)
{
    public string? Field { get; } = field;
}