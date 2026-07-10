using FluentValidation;
using RoomsBooking.Application.UseCases.Bookings.Commands;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Bookings.Validators;

public class PatchBookingCommandValidator : AbstractValidator<PatchBookingCommand>
{
    public PatchBookingCommandValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTimeOffset.UtcNow).WithMessage("Время начала бронирования должно быть в будущем.")
            .OverridePropertyName("startTime")
            .When(x => x.StartTime.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime!.Value).WithMessage("Время окончания должно быть позже времени начала.")
            .OverridePropertyName("endTime")
            .When(x => x.EndTime.HasValue && x.StartTime.HasValue);

        // Проверяем длительность, только если переданы ОБА значения
        RuleFor(x => x)
            .Must(x => x.EndTime!.Value - x.StartTime!.Value >= Booking.MinBookingInterval)
            .WithMessage($"Интервал бронирования слишком короткий. Минимальное время: {Booking.MinBookingInterval.Minutes} минут.")
            .Must(x => x.EndTime!.Value - x.StartTime!.Value <= Booking.MaxBookingInterval)
            .WithMessage($"Интервал бронирования слишком долгий. Максимальное время: {Booking.MaxBookingInterval.Hours} часов.")
            .OverridePropertyName("duration")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);
    }
}