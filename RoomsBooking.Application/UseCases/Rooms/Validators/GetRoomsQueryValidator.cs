using FluentValidation;
using RoomsBooking.Application.UseCases.Rooms.Queries;

namespace RoomsBooking.Application.UseCases.Rooms.Validators;

public class GetRoomsQueryValidator : AbstractValidator<GetRoomsQuery>
{
    public GetRoomsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть больше или равен 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(x => x.PageSize * x.PageNumber)
            .InclusiveBetween(1, 200)
            .WithMessage("Количество комнат, полученное за раз не должно превышать 200.");

        RuleFor(x => x.MinCapacity)
            .GreaterThan(0)
            .When(x => x.MinCapacity.HasValue)
            .WithMessage("Минимальная вместимость должна быть больше нуля.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("Длина поискового запроса не должна превышать 100 символов.");
    }
}