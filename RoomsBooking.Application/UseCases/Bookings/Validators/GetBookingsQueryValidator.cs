using FluentValidation;
using RoomsBooking.Application.UseCases.Bookings.Queries;

namespace RoomsBooking.Application.UseCases.Rooms.Validators;

public class GetBookingsQueryValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть больше или равен 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(x => x.PageSize * x.PageNumber)
            .InclusiveBetween(1, 200)
            .WithMessage("Количество бронирований, полученное за раз не должно превышать 200.");

        RuleFor(x => x.ToTime)
            .GreaterThan(x => x.FromTime!.Value)
            .WithMessage("Фильтр времени окончания должен быть позже времени начала.")
            .When(x => x.FromTime.HasValue && x.ToTime.HasValue)
            .OverridePropertyName("toTime");
    }
}