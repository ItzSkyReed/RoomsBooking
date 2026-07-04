using FluentValidation;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Rooms.Validators;

public class DeleteRoomByNumberCommandValidator : AbstractValidator<DeleteRoomByNumberCommand>
{

    public DeleteRoomByNumberCommandValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Номер комнаты обязателен.")
            .MaximumLength(Room.MaxNumberLength).WithMessage($"Максимальная длина названия комнаты - {Room.MaxNumberLength} символов")
            .OverridePropertyName("number");
    }
}