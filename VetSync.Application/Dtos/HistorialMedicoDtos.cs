namespace VetSync.Application.Dtos;

public class RegistrarHistorialMedicoDto
{
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public int? CitaId { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
}

public class HistorialMedicoDto
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public string NombreMascota { get; set; } = string.Empty;
    public int VeterinarioId { get; set; }
    public string NombreVeterinario { get; set; } = string.Empty;
    public int? CitaId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
}
