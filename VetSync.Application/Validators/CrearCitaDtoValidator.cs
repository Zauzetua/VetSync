using FluentValidation;
using VetSync.Application.Dtos;

namespace VetSync.Application.Validators;

public class CrearCitaDtoValidator : AbstractValidator<CrearCitaDto>
{
    public CrearCitaDtoValidator()
    {
        RuleFor(x => x.MascotaId)
            .GreaterThan(0)
            .WithMessage("El identificador de la mascota es requerido y debe ser mayor a cero.");

        RuleFor(x => x.VeterinarioId)
            .GreaterThan(0)
            .WithMessage("El identificador del veterinario es requerido y debe ser mayor a cero.");

        RuleFor(x => x.FechaHoraInicio)
            .NotEmpty()
            .WithMessage("La fecha y hora de inicio es requerida.")
            .Must(fecha => fecha > DateTime.UtcNow)
            .WithMessage("La cita debe programarse en una fecha y hora futura.");

        RuleFor(x => x.DuracionMinutos)
            .InclusiveBetween(15, 240)
            .WithMessage("La duracion de la cita debe estar entre 15 y 240 minutos.");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("El motivo de la cita es requerido.")
            .MaximumLength(500)
            .WithMessage("El motivo no debe exceder los 500 caracteres.");
    }
}
