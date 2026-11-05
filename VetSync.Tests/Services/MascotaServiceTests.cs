using FluentAssertions;
using Moq;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Services;

public class MascotaServiceTests
{
    private readonly Mock<IMascotaRepository> _mascotaRepositoryMock;
    private readonly Mock<IDuenoRepository> _duenoRepositoryMock;
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly Mock<IHistorialMedicoRepository> _historialMedicoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MascotaService _mascotaService;

    public MascotaServiceTests()
    {
        _mascotaRepositoryMock = new Mock<IMascotaRepository>();
        _duenoRepositoryMock = new Mock<IDuenoRepository>();
        _veterinarioRepositoryMock = new Mock<IVeterinarioRepository>();
        _historialMedicoRepositoryMock = new Mock<IHistorialMedicoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _mascotaService = new MascotaService(
            _mascotaRepositoryMock.Object,
            _duenoRepositoryMock.Object,
            _veterinarioRepositoryMock.Object,
            _historialMedicoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegistrarMascotaAsync_CuandoDuenoExiste_DebeRegistrarExitosamente()
    {
        // Arrange
        var dto = new RegistrarMascotaDto
        {
            DuenoId = 1,
            Nombre = "Firulais",
            Especie = EspecieMascota.Perro,
            Raza = "Golden Retriever",
            FechaNacimiento = DateTime.UtcNow.AddYears(-2),
            Sexo = "Macho",
            PesoKg = 25.5m
        };

        var dueno = new Dueno { Id = 1, NombreCompleto = "Carlos Mendez" };

        _duenoRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.DuenoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueno);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _mascotaService.RegistrarMascotaAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().NotBeNull();
        resultado.Datos!.Nombre.Should().Be("Firulais");
        resultado.Datos.DuenoId.Should().Be(1);
        _mascotaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Mascota>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarMascotaAsync_CuandoDuenoNoExiste_DebeRetornarFallo()
    {
        // Arrange
        var dto = new RegistrarMascotaDto
        {
            DuenoId = 999,
            Nombre = "Milo",
            Especie = EspecieMascota.Gato,
            Raza = "Siames",
            FechaNacimiento = DateTime.UtcNow.AddYears(-1),
            Sexo = "Macho",
            PesoKg = 4.2m
        };

        _duenoRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.DuenoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dueno?)null);

        // Act
        var resultado = await _mascotaService.RegistrarMascotaAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        resultado.Mensaje.ToLower().Should().Contain("dueno");
        _mascotaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Mascota>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarMascotaAsync_CuandoPesoEsInvalido_DebeRetornarFallo()
    {
        // Arrange
        var dto = new RegistrarMascotaDto
        {
            DuenoId = 1,
            Nombre = "Boby",
            Especie = EspecieMascota.Perro,
            Raza = "Criollo",
            FechaNacimiento = DateTime.UtcNow.AddYears(-1),
            Sexo = "Macho",
            PesoKg = -5.0m // Peso invalido
        };

        var dueno = new Dueno { Id = 1, NombreCompleto = "Laura Gomez" };

        _duenoRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.DuenoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dueno);

        // Act
        var resultado = await _mascotaService.RegistrarMascotaAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(400);
        _mascotaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Mascota>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarHistorialMedicoAsync_CuandoMascotaYVeterinarioExisten_DebeRegistrarExitosamente()
    {
        // Arrange
        var dto = new RegistrarHistorialMedicoDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            Diagnostico = "Infeccion leve de oido",
            Tratamiento = "Gotas oticas por 7 dias",
            Observaciones = "Control en una semana"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Coco" };
        var veterinario = new Veterinario { Id = 2, NombreCompleto = "Dr. Ruiz", Activo = true };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veterinario);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _mascotaService.RegistrarHistorialMedicoAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().NotBeNull();
        resultado.Datos!.Diagnostico.Should().Be("Infeccion leve de oido");
        _historialMedicoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<HistorialMedico>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarHistorialMedicoAsync_CuandoMascotaNoExiste_DebeRetornarFallo()
    {
        // Arrange
        var dto = new RegistrarHistorialMedicoDto
        {
            MascotaId = 999,
            VeterinarioId = 2,
            Diagnostico = "Gripe",
            Tratamiento = "Reposo"
        };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mascota?)null);

        // Act
        var resultado = await _mascotaService.RegistrarHistorialMedicoAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        _historialMedicoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<HistorialMedico>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
