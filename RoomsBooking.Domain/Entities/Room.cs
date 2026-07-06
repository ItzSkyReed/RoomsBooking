namespace RoomsBooking.Domain.Entities;

public class Room
{
    public const short MinCapacity = 0;
    public const int MaxNumberLength = 100;
    public const int MaxDescriptionLength = 2000;

    private Room()
    {
    } // Для EF Core

    public Room(string number, string? description, short capacity, short floor)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Номер комнаты не может быть пустым", nameof(number));

        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Вместимость комнаты должна быть больше нуля");

        Id = Guid.CreateVersion7();
        Number = number.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Capacity = capacity;
        Floor = floor;
    }

    public Guid Id { get; private set; }
    public string Number { get; private set; } = null!;
    public string? Description { get; private set; }
    public short Capacity { get; private set; }
    public short Floor { get; private set; }

    public IReadOnlyCollection<Booking> Bookings { get; private set; } = [];

    public void UpdateDetails(string? number, string? description, bool isDescriptionSet, short? capacity, short? floor)
    {
        if (number != null)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Номер комнаты не может быть пустым", nameof(number));
            Number = number.Trim();
        }

        if (isDescriptionSet)
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (capacity.HasValue)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Вместимость комнаты должна быть больше нуля");

            Capacity = capacity.Value;
        }

        if (floor.HasValue)
            Floor = floor.Value;
    }
}