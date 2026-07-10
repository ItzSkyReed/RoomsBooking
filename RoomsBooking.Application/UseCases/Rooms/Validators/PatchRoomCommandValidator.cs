using FluentValidation;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Rooms.Validators;

public class PatchRoomCommandValidator : AbstractValidator<PatchRoomCommand>
{
    public PatchRoomCommandValidator()
    {
        RuleFor(x => x.Number)
            .MaximumLength(Room.MaxNumberLength).WithMessage($"Максимальная длина названия комнаты - {Room.MaxNumberLength} символов")
            .NotEmpty().WithMessage("Номер комнаты не может быть пустым.")
            .OverridePropertyName("number")
            .When(x => x.Number != null);

        RuleFor(x => x.Description)
            .MaximumLength(Room.MaxDescriptionLength).WithMessage($"Максимальная длина описания комнаты - {Room.MaxDescriptionLength} символов")
            .OverridePropertyName("description")
            .When(x => x.IsDescriptionSet);

        RuleFor(x => x.Capacity)
            .GreaterThan(Room.MinCapacity).WithMessage($"Вместимость комнаты должна быть больше {Room.MinCapacity}")
            .OverridePropertyName("capacity")
            .When(x => x.Capacity.HasValue);
    }
}