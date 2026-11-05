using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class MascotaRepository : GenericRepository<Mascota>, IMascotaRepository
{
    public MascotaRepository(VetSyncDbContext context) : base(context)
    {
    }

    public override async Task<Mascota?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Dueno)
            .Include(m => m.HistorialesMedicos)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Mascota>> ObtenerPorDuenoIdAsync(int duenoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.DuenoId == duenoId)
            .OrderBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IReadOnlyList<Mascota>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Dueno)
            .OrderBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }
}
