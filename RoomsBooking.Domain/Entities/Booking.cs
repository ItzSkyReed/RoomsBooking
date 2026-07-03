namespace RoomsBooking.Domain.Entities;

public class Booking
{
    private static readonly TimeSpan MinBookingInterval = new(hours: 0, minutes: 10, seconds: 0);

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }

    private Booking()
    {
    } // Для EF Core

    public Booking(Guid roomId, Guid userId, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (startTime <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Время начала бронирования должно быть в будущем", nameof(startTime));

        if (endTime - startTime < MinBookingInterval)
            throw new ArgumentException($"Интервал бронирования слишком короткий. Минимальное время: {MinBookingInterval.Minutes} минут.",
                nameof(endTime));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId не может быть пустым Guid", nameof(userId));

        if (roomId == Guid.Empty)
            throw new ArgumentException("RoomId не может быть пустым Guid", nameof(roomId));

        Id = Guid.CreateVersion7();
        RoomId = roomId;
        UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
    }
}