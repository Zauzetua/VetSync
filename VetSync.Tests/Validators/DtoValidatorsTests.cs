using FluentAssertions;
using VetSync.Application.Dtos;
using VetSync.Application.Validators;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Validators;

public class DtoValidatorsTests
{
    [Fact]
    public void CrearCitaDtoValidator_ValidacionesCorrectas()
    {
        var validator = new CrearCitaDtoValidator();

        var valido = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = DateTime.UtcNow.AddDays(1),
            DuracionMinutos = 30,
            Motivo = "Consulta general"
        };
        validator.Validate(valido).IsValid.Should().BeTrue();

        var invalido = new CrearCitaDto
        {
            MascotaId = 0,
            VeterinarioId = -1,
            FechaHoraInicio = DateTime.UtcNow.AddDays(-1),
            DuracionMinutos = 5,
            Motivo = ""
        };
        var resultado = validator.Validate(invalido);
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void RegistrarMascotaDtoValidator_ValidacionesCorrectas()
    {
        var validator = new RegistrarMascotaDtoValidator();

        var valido = new RegistrarMascotaDto
        {
            DuenoId = 1,
            Nombre = "Rocky",
            Especie = EspecieMascota.Perro,
            Raza = "Pug",
            FechaNacimiento = DateTime.UtcNow.AddYears(-2),
            Sexo = "Macho",
            PesoKg = 8.5m
        };
        validator.Validate(valido).IsValid.Should().BeTrue();

        var invalido = new RegistrarMascotaDto
        {
            DuenoId = 0,
            Nombre = "",
            Raza = "",
            FechaNacimiento = DateTime.UtcNow.AddDays(2),
            Sexo = "",
            PesoKg = 0
        };
        validator.Validate(invalido).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarDuenoDtoValidator_ValidacionesCorrectas()
    {
        var validator = new RegistrarDuenoDtoValidator();

        var valido = new RegistrarDuenoDto
        {
            NombreCompleto = "Alberto Gomez",
            CorreoElectronico = "alberto@gmail.com",
            Telefono = "1234567890"
        };
        validator.Validate(valido).IsValid.Should().BeTrue();

        var invalido = new RegistrarDuenoDto
        {
            NombreCompleto = "",
            CorreoElectronico = "correo-invalido",
            Telefono = ""
        };
        validator.Validate(invalido).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarHistorialMedicoDtoValidator_ValidacionesCorrectas()
    {
        var validator = new RegistrarHistorialMedicoDtoValidator();

        var valido = new RegistrarHistorialMedicoDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            Diagnostico = "Dermatitis",
            Tratamiento = "Pomada topica"
        };
        validator.Validate(valido).IsValid.Should().BeTrue();

        var invalido = new RegistrarHistorialMedicoDto
        {
            MascotaId = 0,
            VeterinarioId = 0,
            Diagnostico = "",
            Tratamiento = ""
        };
        validator.Validate(invalido).IsValid.Should().BeFalse();
    }
}
