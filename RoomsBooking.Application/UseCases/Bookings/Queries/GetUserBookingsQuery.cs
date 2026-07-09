using MediatR;
using RoomsBooking.Application.UseCases.Bookings.Dtos;

namespace RoomsBooking.Application.UseCases.Bookings.Queries;

public record GetUserBookingsQuery(Guid UserId) : IRequest<List<BookingDto>>;