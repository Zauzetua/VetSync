using VetSync.Domain.Entities;

namespace VetSync.Application.Interfaces;

public interface IVeterinarioRepository : IGenericRepository<Veterinario>
{
    Task<IReadOnlyList<Veterinario>> ObtenerActivosAsync(CancellationToken cancellationToken = default);
}
