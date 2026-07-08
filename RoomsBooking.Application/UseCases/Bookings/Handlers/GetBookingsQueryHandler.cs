using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Common.Dtos;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Mappers;
using RoomsBooking.Application.UseCases.Bookings.Queries;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class GetBookingsQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetBookingsQuery, PagedResponse<BookingDto>>
{
    public async Task<PagedResponse<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Bookings.AsQueryable();

        if (request.RoomId.HasValue)
            query = query.Where(r => r.RoomId == request.RoomId.Value);

        if (request.UserId.HasValue)
            query = query.Where(r => r.UserId == request.UserId.Value);

        if (request.FromTime.HasValue)
            query = query.Where(r => r.StartTime >= request.FromTime);

        if (request.ToTime.HasValue)
            query = query.Where(r => r.EndTime <= request.ToTime.Value);

        // Общее количество записей
        var totalCount = await query.CountAsync(cancellationToken);

        // Сортировка
        query = request.SortBy switch
        {
            BookingsSortBy.EndTime => request.SortDescending
                ? query.OrderByDescending(r => r.EndTime).ThenBy(r => r.Id)
                : query.OrderBy(r => r.EndTime).ThenBy(r => r.Id),

            _ => request.SortDescending
                ? query.OrderByDescending(r => r.StartTime).ThenBy(r => r.Id)
                : query.OrderBy(r => r.StartTime).ThenBy(r => r.Id),
        };


        var dtos = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToDto()
            .ToListAsync(cancellationToken);

        return new PagedResponse<BookingDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}