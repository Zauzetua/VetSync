using VetSync.Domain.Enums;

namespace VetSync.Application.Dtos;

public class MascotaDto
{
    public int Id { get; set; }
    public int DuenoId { get; set; }
    public string NombreDueno { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public EspecieMascota Especie { get; set; }
    public string Raza { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
}
