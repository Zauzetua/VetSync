namespace VetSync.Domain.Entities;

public class HistorialMedico
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public int? CitaId { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;

    public Mascota? Mascota { get; set; }
    public Veterinario? Veterinario { get; set; }
    public Cita? Cita { get; set; }
}
