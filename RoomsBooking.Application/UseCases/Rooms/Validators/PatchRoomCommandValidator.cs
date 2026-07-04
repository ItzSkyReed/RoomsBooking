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
            .OverridePropertyName("number");

        RuleFor(x => x.Description)
            .MaximumLength(Room.MaxDescriptionLength).WithMessage($"Максимальная длина описания комнаты - {Room.MaxDescriptionLength} символов")
            .OverridePropertyName("description");

        RuleFor(x => x.Capacity)
            .GreaterThan(Room.MinCapacity).WithMessage($"Вместимость комнаты должна быть больше {Room.MinCapacity}")
            .OverridePropertyName("capacity");
    }
}