using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class VeterinarioRepository : GenericRepository<Veterinario>, IVeterinarioRepository
{
    public VeterinarioRepository(VetSyncDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Veterinario>> ObtenerActivosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.Activo)
            .OrderBy(v => v.NombreCompleto)
            .ToListAsync(cancellationToken);
    }
}
