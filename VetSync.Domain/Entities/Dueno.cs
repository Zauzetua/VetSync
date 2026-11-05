namespace VetSync.Domain.Entities;

public class Dueno
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
