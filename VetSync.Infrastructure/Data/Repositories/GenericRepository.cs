using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class GenericRepository<TEntidad> : IGenericRepository<TEntidad> where TEntidad : class
{
    protected readonly VetSyncDbContext _context;
    protected readonly DbSet<TEntidad> _dbSet;

    public GenericRepository(VetSyncDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntidad>();
    }

    public virtual async Task<TEntidad?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task AgregarAsync(TEntidad entidad, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entidad, cancellationToken);
    }

    public virtual void Actualizar(TEntidad entidad)
    {
        _dbSet.Update(entidad);
    }

    public virtual void Eliminar(TEntidad entidad)
    {
        _dbSet.Remove(entidad);
    }
}
