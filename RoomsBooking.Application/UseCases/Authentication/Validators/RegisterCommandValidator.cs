using RoomsBooking.Application.UseCases.Authentication.Commands;


using FluentValidation;
using RoomsBooking.Application.UseCases.Authentication.Commands;

namespace RoomsBooking.Application.UseCases.Authentication.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Неверный формат Email.")
            .OverridePropertyName("email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(8).WithMessage("Пароль должен быть не короче 8 символов.")
            .OverridePropertyName("password");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно.")
            .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов.")
            .OverridePropertyName("name");
    }
}