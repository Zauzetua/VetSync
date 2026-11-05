namespace VetSync.Application.Dtos;

public class CrearCitaDto
{
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; } = 30;
    public string Motivo { get; set; } = string.Empty;
}
