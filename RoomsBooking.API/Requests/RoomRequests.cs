using System.Text.Json.Serialization;
using RoomsBooking.Application.UseCases.Rooms.Queries;

namespace RoomsBooking.API.Requests;

// Rooms
public record CreateRoomRequest(string Number, string? Description, short Capacity, short Floor);

public class PatchRoomRequest
{
    private string? _description;
    public short? Capacity { get; set; }
    public short? Floor { get; set; }
    public string? Number { get; set; }

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

public record GetRoomsRequest(
    int? MinCapacity,
    short? Floor,
    string? SearchTerm,
    RoomSortBy? SortBy,
    bool SortDescending = false,
    int PageNumber = 1,
    int PageSize = 20
);