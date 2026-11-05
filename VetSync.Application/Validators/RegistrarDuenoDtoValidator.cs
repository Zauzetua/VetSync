using FluentValidation;
using VetSync.Application.Dtos;

namespace VetSync.Application.Validators;

public class RegistrarDuenoDtoValidator : AbstractValidator<RegistrarDuenoDto>
{
    public RegistrarDuenoDtoValidator()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty()
            .WithMessage("El nombre completo es requerido.")
            .MaximumLength(100)
            .WithMessage("El nombre no debe superar los 100 caracteres.");

        RuleFor(x => x.CorreoElectronico)
            .NotEmpty()
            .WithMessage("El correo electronico es requerido.")
            .EmailAddress()
            .WithMessage("El formato del correo electronico no es valido.")
            .MaximumLength(150)
            .WithMessage("El correo no debe superar los 150 caracteres.");

        RuleFor(x => x.Telefono)
            .NotEmpty()
            .WithMessage("El telefono es requerido.")
            .MaximumLength(20)
            .WithMessage("El telefono no debe superar los 20 caracteres.");
    }
}
