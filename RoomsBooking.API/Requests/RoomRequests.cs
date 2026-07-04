using System.Text.Json.Serialization;

namespace RoomsBooking.API.Requests;

// Rooms
public record CreateRoomRequest(string Number, string? Description, short Capacity, short Floor);

public class PatchRoomRequest
{
    public short? Capacity { get; set; }
    public short? Floor { get; set; }
    public string? Number { get; set; }


    private string? _description;

    //  передал ли клиент поле description в Body
    [JsonIgnore] // Чтоб не показывался на схеме
    public bool IsDescriptionSet { get; private set; }

    public string? Description
    {
        get => _description;
        init // Вызовется только если поле есть в JSON
        {
            _description = value;
            IsDescriptionSet = true;
        }
    }
}
