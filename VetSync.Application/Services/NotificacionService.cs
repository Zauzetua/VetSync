using VetSync.Application.Common.Models;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;

namespace VetSync.Application.Services;

public class NotificacionService : INotificacionService
{
    private readonly ICitaRepository _citaRepository;
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IUnitOfWork _unitOfWork;

    public NotificacionService(
        ICitaRepository citaRepository,
        INotificacionRepository notificacionRepository,
        IEmailSenderService emailSenderService,
        IUnitOfWork unitOfWork)
    {
        _citaRepository = citaRepository;
        _notificacionRepository = notificacionRepository;
        _emailSenderService = emailSenderService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<int>> EnviarRecordatoriosCitasProximasAsync(CancellationToken cancellationToken = default)
    {
        // Limite superior de 24 horas para recordatorios de citas proximas
        var limiteSuperior = DateTime.UtcNow.AddHours(24);
        var citasProximas = await _citaRepository.ObtenerCitasProximasSinNotificarAsync(limiteSuperior, cancellationToken);

        if (citasProximas.Count == 0)
        {
            return ServiceResponse<int>.Exito(0, "No se encontraron citas pendientes de notificacion.");
        }

        var enviadasExitosas = 0;

        foreach (var cita in citasProximas)
        {
            var nombreMascota = cita.Mascota?.Nombre ?? "su mascota";
            var correoDestino = cita.Mascota?.Dueno?.CorreoElectronico ?? string.Empty;
            var nombreDueno = cita.Mascota?.Dueno?.NombreCompleto ?? "Cliente";
            var fechaFormateada = cita.FechaHoraInicio.ToString("yyyy-MM-dd HH:mm");

            var asunto = $"Recordatorio de cita veterinaria para {nombreMascota}";
            var mensaje = $"Hola {nombreDueno}, le recordamos que {nombreMascota} tiene una cita programada para el {fechaFormateada}.";

            var notificacion = new Notificacion
            {
                CitaId = cita.Id,
                DestinatarioCorreo = correoDestino,
                Asunto = asunto,
                Mensaje = mensaje,
                FechaProgramadaEnvio = DateTime.UtcNow
            };

            try
            {
                await _emailSenderService.EnviarCorreoAsync(correoDestino, asunto, mensaje, cancellationToken);
                notificacion.Estado = EstadoNotificacion.Enviada;
                notificacion.FechaEnvioReal = DateTime.UtcNow;
                enviadasExitosas++;
            }
            catch (Exception ex)
            {
                notificacion.Estado = EstadoNotificacion.Fallida;
                notificacion.ErrorDetalle = ex.Message;
            }

            await _notificacionRepository.AgregarAsync(notificacion, cancellationToken);
        }

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return ServiceResponse<int>.Exito(
            enviadasExitosas,
            $"Procesamiento de notificaciones completado. Notificaciones exitosas: {enviadasExitosas} de {citasProximas.Count}.");
    }
}
