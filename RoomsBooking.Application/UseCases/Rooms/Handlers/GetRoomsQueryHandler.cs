using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Common.Dtos;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Dtos;
using RoomsBooking.Application.UseCases.Rooms.Mappers;
using RoomsBooking.Application.UseCases.Rooms.Queries;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

public class GetRoomsQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetRoomsQuery, PagedResponse<RoomDto>>
{
    public async Task<PagedResponse<RoomDto>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Rooms.AsQueryable();

        if (request.MinCapacity.HasValue)
            query = query.Where(r => r.Capacity >= request.MinCapacity.Value);

        if (request.Floor.HasValue)
            query = query.Where(r => r.Floor == request.Floor.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(r => r.Number.Contains(request.SearchTerm) ||
                                     (r.Description != null && r.Description.Contains(request.SearchTerm)));

        // Общее количество записей
        var totalCount = await query.CountAsync(cancellationToken);


        // Сортировка
        query = request.SortBy switch
        {
            RoomSortBy.Capacity => request.SortDescending
                ? query.OrderByDescending(r => r.Capacity).ThenBy(r => r.Id)
                : query.OrderBy(r => r.Capacity).ThenBy(r => r.Id),

            RoomSortBy.Floor => request.SortDescending
                ? query.OrderByDescending(r => r.Floor)
                : query.OrderBy(r => r.Floor),

            _ => request.SortDescending
                ? query.OrderByDescending(r => r.Number)
                : query.OrderBy(r => r.Number)
        };

        // Конкретная страницу данных
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x => x.ToRoomDto()).ToList();

        return new PagedResponse<RoomDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}