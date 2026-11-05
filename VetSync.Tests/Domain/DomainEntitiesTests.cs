using FluentAssertions;
using VetSync.Application.Dtos;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using VetSync.Domain.Exceptions;
using Xunit;

namespace VetSync.Tests.Domain;

public class DomainEntitiesTests
{
    [Fact]
    public void Cita_SeSolapaCon_CuandoHayConflicto_DebeRetornarTrue()
    {
        var inicio = new DateTime(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);
        var fin = inicio.AddMinutes(30);

        var cita = new Cita
        {
            FechaHoraInicio = inicio,
            FechaHoraFin = fin,
            Estado = EstadoCita.Programada
        };

        // Conflicto total o parcial
        cita.SeSolapaCon(inicio.AddMinutes(10), fin.AddMinutes(10)).Should().BeTrue();
    }

    [Fact]
    public void Cita_SeSolapaCon_CuandoSonContiguas_DebeRetornarFalse()
    {
        var inicio = new DateTime(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);
        var fin = inicio.AddMinutes(30);

        var cita = new Cita
        {
            FechaHoraInicio = inicio,
            FechaHoraFin = fin,
            Estado = EstadoCita.Programada
        };

        // Empieza justo cuando la otra termina
        cita.SeSolapaCon(fin, fin.AddMinutes(30)).Should().BeFalse();
        // Termina justo cuando la otra empieza
        cita.SeSolapaCon(inicio.AddMinutes(-30), inicio).Should().BeFalse();
    }

    [Fact]
    public void Cita_SeSolapaCon_CuandoEstaCancelada_DebeRetornarFalse()
    {
        var inicio = new DateTime(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);
        var fin = inicio.AddMinutes(30);

        var cita = new Cita
        {
            FechaHoraInicio = inicio,
            FechaHoraFin = fin,
            Estado = EstadoCita.Cancelada
        };

        cita.SeSolapaCon(inicio, fin).Should().BeFalse();
    }

    [Fact]
    public void ExcepcionesDeDominio_DebenContenerPropiedadesYMensajesEsperados()
    {
        var ex1 = new DominioException("Error de negocio");
        ex1.Message.Should().Be("Error de negocio");

        var inner = new Exception("Fallo interno");
        var ex1Inner = new DominioException("Error con causa", inner);
        ex1Inner.InnerException.Should().Be(inner);

        var inicio = DateTime.UtcNow;
        var fin = inicio.AddMinutes(30);
        var exSolapamiento = new SolapamientoCitaException(5, inicio, fin);
        exSolapamiento.VeterinarioId.Should().Be(5);
        exSolapamiento.FechaHoraInicio.Should().Be(inicio);
        exSolapamiento.FechaHoraFin.Should().Be(fin);
        exSolapamiento.Message.Should().Contain("5");

        var exNoEncontrada = new EntidadNoEncontradaException("Mascota", 99);
        exNoEncontrada.NombreEntidad.Should().Be("Mascota");
        exNoEncontrada.Clave.Should().Be(99);
        exNoEncontrada.Message.Should().Contain("Mascota");
    }

    [Fact]
    public void Dtos_InstanciacionYPropiedades()
    {
        var duenoDto = new DuenoDto
        {
            Id = 1,
            NombreCompleto = "Juan",
            CorreoElectronico = "j@t.com",
            Telefono = "123",
            Direccion = "Calle 1"
        };
        duenoDto.Id.Should().Be(1);

        var notifDto = new NotificacionDto
        {
            Id = 2,
            CitaId = 3,
            DestinatarioCorreo = "a@b.com",
            Asunto = "Asunto",
            Mensaje = "Msg",
            Estado = EstadoNotificacion.Enviada,
            FechaProgramadaEnvio = DateTime.UtcNow,
            FechaEnvioReal = DateTime.UtcNow
        };
        notifDto.Id.Should().Be(2);

        var regDueno = new RegistrarDuenoDto
        {
            NombreCompleto = "Ana",
            CorreoElectronico = "ana@mail.com",
            Telefono = "456",
            Direccion = "Av 2"
        };
        regDueno.NombreCompleto.Should().Be("Ana");
    }
}
