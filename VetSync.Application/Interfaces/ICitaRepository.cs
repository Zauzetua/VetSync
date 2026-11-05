using VetSync.Domain.Entities;

namespace VetSync.Application.Interfaces;

public interface ICitaRepository : IGenericRepository<Cita>
{
    Task<IReadOnlyList<Cita>> ObtenerPorVeterinarioYRangoAsync(int veterinarioId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default);
    Task<bool> ExisteSolapamientoAsync(int veterinarioId, DateTime inicio, DateTime fin, int? citaIdExcluir = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cita>> ObtenerCitasProximasSinNotificarAsync(DateTime limiteSuperior, CancellationToken cancellationToken = default);
}
