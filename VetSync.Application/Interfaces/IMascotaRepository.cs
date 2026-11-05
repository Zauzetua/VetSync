using VetSync.Domain.Entities;

namespace VetSync.Application.Interfaces;

public interface IMascotaRepository : IGenericRepository<Mascota>
{
    Task<IReadOnlyList<Mascota>> ObtenerPorDuenoIdAsync(int duenoId, CancellationToken cancellationToken = default);
}
