namespace RoomsBooking.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string Number { get; private set; }
    public string? Description { get; private set; }
    public short Capacity { get; private set; }
    public short Floor { get; private set; }

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
        Description = description;
        Capacity = capacity;
        Floor = floor;
    }
}