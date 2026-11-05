namespace VetSync.Domain.Entities;

public class Veterinario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CedulaProfesional { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
}
