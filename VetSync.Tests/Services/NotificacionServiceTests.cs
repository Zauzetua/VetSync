using FluentAssertions;
using Moq;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Services;

public class NotificacionServiceTests
{
    private readonly Mock<ICitaRepository> _citaRepositoryMock;
    private readonly Mock<INotificacionRepository> _notificacionRepositoryMock;
    private readonly Mock<IEmailSenderService> _emailSenderServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly NotificacionService _notificacionService;

    public NotificacionServiceTests()
    {
        _citaRepositoryMock = new Mock<ICitaRepository>();
        _notificacionRepositoryMock = new Mock<INotificacionRepository>();
        _emailSenderServiceMock = new Mock<IEmailSenderService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _notificacionService = new NotificacionService(
            _citaRepositoryMock.Object,
            _notificacionRepositoryMock.Object,
            _emailSenderServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task EnviarRecordatoriosCitasProximasAsync_CuandoHayCitasProximasSinNotificar_DebeEnviarCorreosYRegistrarNotificaciones()
    {
        // Arrange
        var dueno = new Dueno { Id = 1, NombreCompleto = "Ana Morales", CorreoElectronico = "ana@correo.com" };
        var mascota = new Mascota { Id = 1, Nombre = "Mia", Dueno = dueno };
        var veterinario = new Veterinario { Id = 1, NombreCompleto = "Dr. Sanchez" };

        var citasProximas = new List<Cita>
        {
            new Cita
            {
                Id = 101,
                MascotaId = 1,
                Mascota = mascota,
                VeterinarioId = 1,
                Veterinario = veterinario,
                FechaHoraInicio = DateTime.UtcNow.AddHours(12),
                Estado = EstadoCita.Programada
            }
        };

        _citaRepositoryMock.Setup(r => r.ObtenerCitasProximasSinNotificarAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(citasProximas);

        _emailSenderServiceMock.Setup(e => e.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _notificacionService.EnviarRecordatoriosCitasProximasAsync();

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().Be(1); // 1 notificacion enviada
        _emailSenderServiceMock.Verify(e => e.EnviarCorreoAsync(
            "ana@correo.com",
            It.Is<string>(s => s.Contains("Recordatorio")),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _notificacionRepositoryMock.Verify(n => n.AgregarAsync(
            It.Is<Notificacion>(notif => notif.CitaId == 101 && notif.Estado == EstadoNotificacion.Enviada),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnviarRecordatoriosCitasProximasAsync_CuandoNoHayCitasProximas_NoDebeEnviarCorreos()
    {
        // Arrange
        _citaRepositoryMock.Setup(r => r.ObtenerCitasProximasSinNotificarAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cita>());

        // Act
        var resultado = await _notificacionService.EnviarRecordatoriosCitasProximasAsync();

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().Be(0);
        _emailSenderServiceMock.Verify(e => e.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _notificacionRepositoryMock.Verify(n => n.AgregarAsync(It.IsAny<Notificacion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EnviarRecordatoriosCitasProximasAsync_CuandoFallaEnvioCorreo_DebeRegistrarNotificacionComoFallidaSinInterrumpirProceso()
    {
        // Arrange
        var dueno = new Dueno { Id = 1, NombreCompleto = "Pedro Gomez", CorreoElectronico = "pedro@correo.com" };
        var mascota = new Mascota { Id = 1, Nombre = "Thor", Dueno = dueno };
        var veterinario = new Veterinario { Id = 1, NombreCompleto = "Dr. Sanchez" };

        var citasProximas = new List<Cita>
        {
            new Cita
            {
                Id = 102,
                MascotaId = 1,
                Mascota = mascota,
                VeterinarioId = 1,
                Veterinario = veterinario,
                FechaHoraInicio = DateTime.UtcNow.AddHours(6),
                Estado = EstadoCita.Programada
            }
        };

        _citaRepositoryMock.Setup(r => r.ObtenerCitasProximasSinNotificarAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(citasProximas);

        // Simulamos fallo en servidor de correos
        _emailSenderServiceMock.Setup(e => e.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Servidor SMTP no disponible"));

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _notificacionService.EnviarRecordatoriosCitasProximasAsync();

        // Assert
        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().Be(0); // 0 exitosas
        _notificacionRepositoryMock.Verify(n => n.AgregarAsync(
            It.Is<Notificacion>(notif => notif.CitaId == 102 && notif.Estado == EstadoNotificacion.Fallida),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
