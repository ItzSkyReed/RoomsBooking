using FluentValidation;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Bookings.Validators;

public class BookRoomCommandValidator : AbstractValidator<BookRoomCommand>
{
    public BookRoomCommandValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTimeOffset.UtcNow).WithMessage("Время начала бронирования должно быть в будущем.")
            .OverridePropertyName("startTime");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("Время окончания должно быть позже времени начала.")
            .OverridePropertyName("endTime");

        RuleFor(x => x)
            .Must(x => x.EndTime - x.StartTime >= Booking.MinBookingInterval)
            .WithMessage($"Интервал бронирования слишком короткий. Минимальное время: {Booking.MinBookingInterval.Minutes} минут.")
            .Must(x => x.EndTime - x.StartTime <= Booking.MaxBookingInterval)
            .WithMessage($"Интервал бронирования слишком долгий. Максимальное время: {Booking.MaxBookingInterval.Hours} часов.")
            .OverridePropertyName("duration");
    }
}