using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Application.UseCases.Bookings.Dtos;
using RoomsBooking.Application.UseCases.Bookings.Mappers;
using RoomsBooking.Domain.Exceptions.Bookings;
using RoomsBooking.Domain.Exceptions.Room;
using RoomsBooking.Domain.Exceptions.User;

namespace RoomsBooking.Application.UseCases.Bookings.Handlers;

public class BookRoomCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<BookRoomCommand, BookingDto>
{
    public async Task<BookingDto> Handle(BookRoomCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Rooms.AsNoTracking().AnyAsync(r => r.Id == request.RoomId, cancellationToken))
            throw new RoomNotFoundException(request.RoomId);

        var userName = await dbContext.Users
                           .Where(u => u.Id == request.UserId)
                           .Select(u => u.Name)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new UserNotFoundException(request.UserId);

        var isOverlap = await dbContext.Bookings
            .Where(b => b.RoomId == request.RoomId)
            .AnyAsync(b => request.StartTime < b.EndTime && b.StartTime < request.EndTime, cancellationToken);

        if (isOverlap)
            throw new RoomAlreadyBooked(request.RoomId);

        var bookingEntity = request.ToBookingEntity();

        dbContext.Bookings.Add(bookingEntity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return bookingEntity.ToBookingDto(userName);
    }
}