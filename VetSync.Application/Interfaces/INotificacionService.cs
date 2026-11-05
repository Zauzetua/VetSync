using VetSync.Application.Common.Models;

namespace VetSync.Application.Interfaces;

public interface INotificacionService
{
    Task<ServiceResponse<int>> EnviarRecordatoriosCitasProximasAsync(CancellationToken cancellationToken = default);
}
