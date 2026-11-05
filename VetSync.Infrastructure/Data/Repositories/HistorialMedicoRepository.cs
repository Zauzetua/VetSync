using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class HistorialMedicoRepository : GenericRepository<HistorialMedico>, IHistorialMedicoRepository
{
    public HistorialMedicoRepository(VetSyncDbContext context) : base(context)
    {
    }

    public override async Task<HistorialMedico?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Mascota)
            .Include(h => h.Veterinario)
            .Include(h => h.Cita)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<HistorialMedico>> ObtenerPorMascotaIdAsync(int mascotaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Veterinario)
            .Where(h => h.MascotaId == mascotaId)
            .OrderByDescending(h => h.FechaRegistro)
            .ToListAsync(cancellationToken);
    }
}
