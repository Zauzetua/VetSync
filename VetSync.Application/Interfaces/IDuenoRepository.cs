using VetSync.Domain.Entities;

namespace VetSync.Application.Interfaces;

public interface IDuenoRepository : IGenericRepository<Dueno>
{
    Task<Dueno?> ObtenerPorCorreoAsync(string correoElectronico, CancellationToken cancellationToken = default);
}
