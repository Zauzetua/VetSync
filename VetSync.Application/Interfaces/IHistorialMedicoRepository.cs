using VetSync.Domain.Entities;

namespace VetSync.Application.Interfaces;

public interface IHistorialMedicoRepository : IGenericRepository<HistorialMedico>
{
    Task<IReadOnlyList<HistorialMedico>> ObtenerPorMascotaIdAsync(int mascotaId, CancellationToken cancellationToken = default);
}
