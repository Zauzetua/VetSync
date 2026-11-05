using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure.Data.Repositories;

public class CitaRepository : GenericRepository<Cita>, ICitaRepository
{
    public CitaRepository(VetSyncDbContext context) : base(context)
    {
    }

    public override async Task<Cita?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Mascota)
                .ThenInclude(m => m!.Dueno)
            .Include(c => c.Veterinario)
            .Include(c => c.HistorialMedico)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Cita>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Mascota)
                .ThenInclude(m => m!.Dueno)
            .Include(c => c.Veterinario)
            .OrderByDescending(c => c.FechaHoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cita>> ObtenerPorVeterinarioYRangoAsync(int veterinarioId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Mascota)
            .Where(c => c.VeterinarioId == veterinarioId &&
                        c.Estado != EstadoCita.Cancelada &&
                        c.FechaHoraInicio < fin &&
                        c.FechaHoraFin > inicio)
            .OrderBy(c => c.FechaHoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteSolapamientoAsync(int veterinarioId, DateTime inicio, DateTime fin, int? citaIdExcluir = null, CancellationToken cancellationToken = default)
    {
        var consulta = _dbSet
            .Where(c => c.VeterinarioId == veterinarioId &&
                        c.Estado != EstadoCita.Cancelada &&
                        c.FechaHoraInicio < fin &&
                        c.FechaHoraFin > inicio);

        if (citaIdExcluir.HasValue)
        {
            consulta = consulta.Where(c => c.Id != citaIdExcluir.Value);
        }

        return await consulta.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cita>> ObtenerCitasProximasSinNotificarAsync(DateTime limiteSuperior, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Mascota)
                .ThenInclude(m => m!.Dueno)
            .Include(c => c.Veterinario)
            .Where(c => c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada)
            .Where(c => c.FechaHoraInicio <= limiteSuperior && c.FechaHoraInicio >= DateTime.UtcNow)
            .Where(c => !c.Notificaciones.Any(n => n.Estado == EstadoNotificacion.Enviada))
            .ToListAsync(cancellationToken);
    }
}
