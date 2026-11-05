using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;

namespace VetSync.Application.Interfaces;

public interface IMascotaService
{
    Task<ServiceResponse<MascotaDto>> RegistrarMascotaAsync(RegistrarMascotaDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResponse<MascotaDto>> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResponse<IReadOnlyList<MascotaDto>>> ObtenerPorDuenoIdAsync(int duenoId, CancellationToken cancellationToken = default);
    Task<ServiceResponse<DuenoDto>> RegistrarDuenoAsync(RegistrarDuenoDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResponse<HistorialMedicoDto>> RegistrarHistorialMedicoAsync(RegistrarHistorialMedicoDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResponse<IReadOnlyList<HistorialMedicoDto>>> ObtenerHistorialPorMascotaAsync(int mascotaId, CancellationToken cancellationToken = default);
}
