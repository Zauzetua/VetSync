using FluentAssertions;
using Moq;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Services;

public class CitaServiceConsultasTests
{
    private readonly Mock<ICitaRepository> _citaRepositoryMock;
    private readonly Mock<IMascotaRepository> _mascotaRepositoryMock;
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CitaService _citaService;

    public CitaServiceConsultasTests()
    {
        _citaRepositoryMock = new Mock<ICitaRepository>();
        _mascotaRepositoryMock = new Mock<IMascotaRepository>();
        _veterinarioRepositoryMock = new Mock<IVeterinarioRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _citaService = new CitaService(
            _citaRepositoryMock.Object,
            _mascotaRepositoryMock.Object,
            _veterinarioRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoCitaExiste_DebeRetornarCitaDto()
    {
        var cita = new Cita
        {
            Id = 10,
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = DateTime.UtcNow.AddDays(1),
            FechaHoraFin = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Estado = EstadoCita.Programada,
            Motivo = "Revision dental",
            Mascota = new Mascota
            {
                Nombre = "Rocky",
                Dueno = new Dueno { NombreCompleto = "Laura Gomez", CorreoElectronico = "laura@mail.com" }
            },
            Veterinario = new Veterinario { NombreCompleto = "Dr. Alvarez" }
        };

        _citaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        var resultado = await _citaService.ObtenerPorIdAsync(10);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().NotBeNull();
        resultado.Datos!.NombreMascota.Should().Be("Rocky");
        resultado.Datos.NombreDueno.Should().Be("Laura Gomez");
        resultado.Datos.NombreVeterinario.Should().Be("Dr. Alvarez");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoCitaNoExiste_DebeRetornarFallo404()
    {
        _citaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cita?)null);

        var resultado = await _citaService.ObtenerPorIdAsync(999);

        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerCitasPorVeterinarioAsync_DebeRetornarListaDeCitas()
    {
        var fecha = DateTime.UtcNow.Date;
        var lista = new List<Cita>
        {
            new Cita
            {
                Id = 1,
                MascotaId = 1,
                VeterinarioId = 5,
                FechaHoraInicio = fecha.AddHours(9),
                FechaHoraFin = fecha.AddHours(9).AddMinutes(30),
                Motivo = "Vacuna",
                Mascota = new Mascota { Nombre = "Boby" },
                Veterinario = new Veterinario { NombreCompleto = "Dra. Gomez" }
            }
        };

        _citaRepositoryMock.Setup(r => r.ObtenerPorVeterinarioYRangoAsync(5, fecha, fecha.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        var resultado = await _citaService.ObtenerCitasPorVeterinarioAsync(5, fecha);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().HaveCount(1);
        resultado.Datos![0].NombreMascota.Should().Be("Boby");
    }
}
