using RoomsBooking.Domain.Exceptions;
using RoomsBooking.Domain.Exceptions.Bookings;

namespace RoomsBooking.Domain.Entities;

public class Booking
{
    public static readonly TimeSpan MinBookingInterval = new(hours: 0, minutes: 10, seconds: 0);
    public static readonly TimeSpan MaxBookingInterval = new(hours: 12, minutes: 0, seconds: 0);

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

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }

    public User User { get; private set; } = null!;
    public Room Room { get; private set; } = null!;

    public void UpdateDetails(Guid? roomId, DateTimeOffset? startTime, DateTimeOffset? endTime)
    {
        if (startTime.HasValue)
            if (startTime <= DateTimeOffset.UtcNow)
                throw new BookingStartTimeInPastException();

            else if (endTime - startTime < MinBookingInterval)
                throw new BookingPeriodTooShortException(MinBookingInterval.Minutes);

            else
                StartTime = startTime.Value;

        if (roomId.HasValue)
            if (roomId == Guid.Empty)
                throw new EmptyFieldException(nameof(roomId));
            else
                RoomId = roomId.Value;

        if (endTime.HasValue)
            EndTime = endTime.Value;
    }
}