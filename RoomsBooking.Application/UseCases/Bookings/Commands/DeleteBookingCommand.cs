using MediatR;

namespace RoomsBooking.Application.UseCases.Bookings.Commands;

public record DeleteBookingCommand(Guid BookingId, Guid UserId) : IRequest<Unit>;