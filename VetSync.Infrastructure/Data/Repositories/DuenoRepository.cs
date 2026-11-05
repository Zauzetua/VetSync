using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class DuenoRepository : GenericRepository<Dueno>, IDuenoRepository
{
    public DuenoRepository(VetSyncDbContext context) : base(context)
    {
    }

    public override async Task<Dueno?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Mascotas)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Dueno?> ObtenerPorCorreoAsync(string correoElectronico, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Mascotas)
            .FirstOrDefaultAsync(d => d.CorreoElectronico.ToLower() == correoElectronico.ToLower(), cancellationToken);
    }

    public override async Task<IReadOnlyList<Dueno>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Mascotas)
            .OrderBy(d => d.NombreCompleto)
            .ToListAsync(cancellationToken);
    }
}
