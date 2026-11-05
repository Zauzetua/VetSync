using VetSync.Domain.Enums;

namespace VetSync.Application.Dtos;

public class CitaDto
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public string NombreMascota { get; set; } = string.Empty;
    public string NombreDueno { get; set; } = string.Empty;
    public string CorreoDueno { get; set; } = string.Empty;
    public int VeterinarioId { get; set; }
    public string NombreVeterinario { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public EstadoCita Estado { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
