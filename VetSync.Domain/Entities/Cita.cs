using VetSync.Domain.Enums;

namespace VetSync.Domain.Entities;

public class Cita
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public EstadoCita Estado { get; set; } = EstadoCita.Programada;
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Mascota? Mascota { get; set; }
    public Veterinario? Veterinario { get; set; }
    public HistorialMedico? HistorialMedico { get; set; }
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    public bool SeSolapaCon(DateTime inicio, DateTime fin)
    {
        return Estado != EstadoCita.Cancelada && FechaHoraInicio < fin && FechaHoraFin > inicio;
    }
}
