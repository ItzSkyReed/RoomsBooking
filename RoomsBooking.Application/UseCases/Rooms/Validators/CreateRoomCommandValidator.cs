using FluentValidation;
using RoomsBooking.Application.UseCases.Rooms.Commands;

namespace RoomsBooking.Application.UseCases.Rooms.Validators;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    private const short MinCapacity = 0;
    private const int MaxNumberLength = 100;
    private const int MaxDescriptionLength = 2000;

    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Номер комнаты обязателен.")
            .MaximumLength(MaxNumberLength).WithMessage($"Максимальная длина названия комнаты - {MaxNumberLength} символов")
            .OverridePropertyName("number");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).WithMessage($"Максимальная длина описания комнаты - {MaxDescriptionLength} символов")
            .OverridePropertyName("description");

        RuleFor(x => x.Capacity)
            .GreaterThan(MinCapacity).WithMessage($"Вместимость комнаты должна быть больше {MinCapacity}")
            .OverridePropertyName("capacity");
    }
}