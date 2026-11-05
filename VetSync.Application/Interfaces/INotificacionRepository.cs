using VetSync.Domain.Entities;
using VetSync.Domain.Enums;

namespace VetSync.Application.Interfaces;

public interface INotificacionRepository : IGenericRepository<Notificacion>
{
    Task<IReadOnlyList<Notificacion>> ObtenerPorEstadoAsync(EstadoNotificacion estado, CancellationToken cancellationToken = default);
}
