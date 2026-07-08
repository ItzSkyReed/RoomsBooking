using MediatR;
using RoomsBooking.Application.UseCases.Bookings.Dtos;

namespace RoomsBooking.Application.UseCases.Bookings.Queries;

public record GetBookingQuery(Guid Id) : IRequest<BookingDto>;