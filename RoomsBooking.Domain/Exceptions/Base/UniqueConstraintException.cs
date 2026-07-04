namespace RoomsBooking.Domain.Exceptions.Base;

public class UniqueConstraintException(string constraintName, Exception innerException)
    : Exception($"Нарушение уникальности данных (индекс: {constraintName})", innerException)
{
    public string ConstraintName { get; } = constraintName;
}