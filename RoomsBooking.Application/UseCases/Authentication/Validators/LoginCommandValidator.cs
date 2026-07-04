using RoomsBooking.Application.UseCases.Authentication.Commands;


using FluentValidation;
using RoomsBooking.Application.UseCases.Authentication.Commands;

namespace RoomsBooking.Application.UseCases.Authentication.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Неверный формат Email.")
            .OverridePropertyName("email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(8).WithMessage("Пароль должен быть не короче 8 символов.")
            .OverridePropertyName("password");
    }
}