using MediatR;
using RoomsBooking.Application.Common.Dtos;
using RoomsBooking.Application.UseCases.Rooms.Dtos;

namespace RoomsBooking.Application.UseCases.Rooms.Queries;

public enum RoomSortBy
{
    Number,
    Capacity,
    Floor
}

public record GetRoomsQuery(
    int? MinCapacity,
    short? Floor,
    string? SearchTerm,
    RoomSortBy? SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize
) : IRequest<PagedResponse<RoomDto>>;