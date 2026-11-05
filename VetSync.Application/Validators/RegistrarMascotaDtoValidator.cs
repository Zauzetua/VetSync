using FluentValidation;
using VetSync.Application.Dtos;

namespace VetSync.Application.Validators;

public class RegistrarMascotaDtoValidator : AbstractValidator<RegistrarMascotaDto>
{
    public RegistrarMascotaDtoValidator()
    {
        RuleFor(x => x.DuenoId)
            .GreaterThan(0)
            .WithMessage("El identificador del dueno es requerido y debe ser mayor a cero.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre de la mascota es requerido.")
            .MaximumLength(50)
            .WithMessage("El nombre no debe superar los 50 caracteres.");

        RuleFor(x => x.Especie)
            .IsInEnum()
            .WithMessage("La especie indicada no es valida.");

        RuleFor(x => x.Raza)
            .NotEmpty()
            .WithMessage("La raza es requerida.")
            .MaximumLength(50)
            .WithMessage("La raza no debe superar los 50 caracteres.");

        RuleFor(x => x.FechaNacimiento)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("La fecha de nacimiento no puede ser una fecha futura.");

        RuleFor(x => x.Sexo)
            .NotEmpty()
            .WithMessage("El sexo de la mascota es requerido.")
            .MaximumLength(20)
            .WithMessage("El sexo no debe superar los 20 caracteres.");

        RuleFor(x => x.PesoKg)
            .GreaterThan(0)
            .WithMessage("El peso debe ser mayor a cero kilogramos.");
    }
}
