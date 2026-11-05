using FluentValidation;
using VetSync.Application.Dtos;

namespace VetSync.Application.Validators;

public class RegistrarHistorialMedicoDtoValidator : AbstractValidator<RegistrarHistorialMedicoDto>
{
    public RegistrarHistorialMedicoDtoValidator()
    {
        RuleFor(x => x.MascotaId)
            .GreaterThan(0)
            .WithMessage("El identificador de la mascota es requerido.");

        RuleFor(x => x.VeterinarioId)
            .GreaterThan(0)
            .WithMessage("El identificador del veterinario es requerido.");

        RuleFor(x => x.Diagnostico)
            .NotEmpty()
            .WithMessage("El diagnostico es requerido.")
            .MaximumLength(1000)
            .WithMessage("El diagnostico no debe superar los 1000 caracteres.");

        RuleFor(x => x.Tratamiento)
            .NotEmpty()
            .WithMessage("El tratamiento es requerido.")
            .MaximumLength(2000)
            .WithMessage("El tratamiento no debe superar los 2000 caracteres.");
    }
}
