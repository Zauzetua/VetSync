using VetSync.Domain.Enums;

namespace VetSync.Domain.Entities;

public class Mascota
{
    public int Id { get; set; }
    public int DuenoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public EspecieMascota Especie { get; set; }
    public string Raza { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public Dueno? Dueno { get; set; }
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
}
