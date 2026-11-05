using VetSync.Domain.Enums;

namespace VetSync.Application.Dtos;

public class NotificacionDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public string DestinatarioCorreo { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public EstadoNotificacion Estado { get; set; }
    public DateTime FechaProgramadaEnvio { get; set; }
    public DateTime? FechaEnvioReal { get; set; }
}
