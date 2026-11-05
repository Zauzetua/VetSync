using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;

namespace VetSync.Application.Interfaces;

public interface ICitaService
{
    Task<ServiceResponse<CitaDto>> AgendarCitaAsync(CrearCitaDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResponse<bool>> CancelarCitaAsync(int citaId, CancellationToken cancellationToken = default);
    Task<ServiceResponse<CitaDto>> ObtenerPorIdAsync(int citaId, CancellationToken cancellationToken = default);
    Task<ServiceResponse<IReadOnlyList<CitaDto>>> ObtenerCitasPorVeterinarioAsync(int veterinarioId, DateTime fecha, CancellationToken cancellationToken = default);
}
