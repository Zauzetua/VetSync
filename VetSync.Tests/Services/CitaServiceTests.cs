using FluentAssertions;
using Moq;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Services;

public class CitaServiceTests
{
    private readonly Mock<ICitaRepository> _citaRepositoryMock;
    private readonly Mock<IMascotaRepository> _mascotaRepositoryMock;
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CitaService _citaService;

    public CitaServiceTests()
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
    public async Task AgendarCitaAsync_CuandoDatosSonValidos_DebeCrearCitaExitosamente()
    {
        // Arrange
        var fechaInicio = DateTime.UtcNow.AddDays(1).Date.AddHours(10);
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = fechaInicio,
            DuracionMinutos = 30,
            Motivo = "Revision general"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Max", DuenoId = 1 };
        var veterinario = new Veterinario { Id = 2, NombreCompleto = "Dr. Perez", Activo = true };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veterinario);

        _citaRepositoryMock.Setup(r => r.ExisteSolapamientoAsync(dto.VeterinarioId, dto.FechaHoraInicio, dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().NotBeNull();
        resultado.Datos!.MascotaId.Should().Be(dto.MascotaId);
        resultado.Datos.VeterinarioId.Should().Be(dto.VeterinarioId);
        resultado.Datos.FechaHoraInicio.Should().Be(dto.FechaHoraInicio);
        resultado.Datos.FechaHoraFin.Should().Be(dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos));
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoVeterinarioEstaOcupado_DebeRetornarFalloPorSolapamiento()
    {
        // Arrange
        var fechaInicio = DateTime.UtcNow.AddDays(1).Date.AddHours(10);
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = fechaInicio,
            DuracionMinutos = 30,
            Motivo = "Vacunacion"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Rocky" };
        var veterinario = new Veterinario { Id = 2, NombreCompleto = "Dr. Perez", Activo = true };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veterinario);

        // Simulamos solapamiento de horario existente
        _citaRepositoryMock.Setup(r => r.ExisteSolapamientoAsync(dto.VeterinarioId, dto.FechaHoraInicio, dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(409); // Conflicto
        resultado.Mensaje.ToLower().Should().Contain("solapamiento");
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoFechaEsEnElPasado_DebeRetornarFallo()
    {
        // Arrange
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = DateTime.UtcNow.AddHours(-2), // Fecha en el pasado
            DuracionMinutos = 30,
            Motivo = "Consulta tardia"
        };

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(400);
        resultado.Mensaje.ToLower().Should().Contain("pasado");
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoMascotaNoExiste_DebeRetornarFallo()
    {
        // Arrange
        var dto = new CrearCitaDto
        {
            MascotaId = 999, // Mascota no registrada
            VeterinarioId = 2,
            FechaHoraInicio = DateTime.UtcNow.AddDays(1),
            DuracionMinutos = 30,
            Motivo = "Chequeo"
        };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mascota?)null);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        resultado.Mensaje.ToLower().Should().Contain("mascota");
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoVeterinarioNoExisteOEstaInactivo_DebeRetornarFallo()
    {
        // Arrange
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 888, // Veterinario inexistente
            FechaHoraInicio = DateTime.UtcNow.AddDays(1),
            DuracionMinutos = 30,
            Motivo = "Urgencia menor"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Luna" };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veterinario?)null);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        resultado.Mensaje.ToLower().Should().Contain("veterinario");
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoCitasSonConsecutivas_DebePermitirAgendarSinConflicto()
    {
        // Caso de borde: Cita anterior termina a las 10:30 y nueva inicia exactamente a las 10:30
        var fechaInicio = DateTime.UtcNow.AddDays(2).Date.AddHours(10).AddMinutes(30);
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = fechaInicio,
            DuracionMinutos = 30,
            Motivo = "Continuidad de consulta"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Toby" };
        var veterinario = new Veterinario { Id = 2, NombreCompleto = "Dr. Gomez", Activo = true };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veterinario);

        // Horarios contiguos no deben solaparse
        _citaRepositoryMock.Setup(r => r.ExisteSolapamientoAsync(dto.VeterinarioId, dto.FechaHoraInicio, dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().NotBeNull();
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AgendarCitaAsync_CuandoHayTraslapeMinimoDeUnMinuto_DebeRechazarPorSolapamiento()
    {
        // Caso de borde: Choque de 1 minuto
        var fechaInicio = DateTime.UtcNow.AddDays(2).Date.AddHours(10).AddMinutes(29);
        var dto = new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 2,
            FechaHoraInicio = fechaInicio,
            DuracionMinutos = 30,
            Motivo = "Consulta solapada por un minuto"
        };

        var mascota = new Mascota { Id = 1, Nombre = "Toby" };
        var veterinario = new Veterinario { Id = 2, NombreCompleto = "Dr. Gomez", Activo = true };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veterinario);

        _citaRepositoryMock.Setup(r => r.ExisteSolapamientoAsync(dto.VeterinarioId, dto.FechaHoraInicio, dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _citaService.AgendarCitaAsync(dto);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(409);
        _citaRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Cita>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelarCitaAsync_CuandoCitaExisteYEstaProgramada_DebeCancelarExitosamente()
    {
        // Arrange
        var citaId = 10;
        var cita = new Cita
        {
            Id = citaId,
            Estado = EstadoCita.Programada,
            FechaHoraInicio = DateTime.UtcNow.AddDays(1)
        };

        _citaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _citaService.CancelarCitaAsync(citaId);

        // Assert
        resultado.Exitoso.Should().BeTrue();
        cita.Estado.Should().Be(EstadoCita.Cancelada);
        _citaRepositoryMock.Verify(r => r.Actualizar(cita), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelarCitaAsync_CuandoCitaNoExiste_DebeRetornarFallo()
    {
        // Arrange
        var citaId = 999;
        _citaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cita?)null);

        // Act
        var resultado = await _citaService.CancelarCitaAsync(citaId);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelarCitaAsync_CuandoCitaYaFueCanceladaOCompletada_DebeRetornarFallo()
    {
        // Arrange
        var citaId = 15;
        var cita = new Cita
        {
            Id = citaId,
            Estado = EstadoCita.Cancelada, // Ya cancelada
            FechaHoraInicio = DateTime.UtcNow.AddDays(1)
        };

        _citaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(citaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cita);

        // Act
        var resultado = await _citaService.CancelarCitaAsync(citaId);

        // Assert
        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(400);
        resultado.Mensaje.ToLower().Should().Contain("cancelada");
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
