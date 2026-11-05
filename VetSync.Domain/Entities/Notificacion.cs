using VetSync.Domain.Enums;

namespace VetSync.Domain.Entities;

public class Notificacion
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public string DestinatarioCorreo { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public EstadoNotificacion Estado { get; set; } = EstadoNotificacion.Pendiente;
    public DateTime FechaProgramadaEnvio { get; set; }
    public DateTime? FechaEnvioReal { get; set; }
    public string? ErrorDetalle { get; set; }

    public Cita? Cita { get; set; }
}
