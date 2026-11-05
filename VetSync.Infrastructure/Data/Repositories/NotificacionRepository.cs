using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class NotificacionRepository : GenericRepository<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(VetSyncDbContext context) : base(context)
    {
    }

    public override async Task<Notificacion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(n => n.Cita)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Notificacion>> ObtenerPorEstadoAsync(EstadoNotificacion estado, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(n => n.Cita)
            .Where(n => n.Estado == estado)
            .OrderBy(n => n.FechaProgramadaEnvio)
            .ToListAsync(cancellationToken);
    }
}
