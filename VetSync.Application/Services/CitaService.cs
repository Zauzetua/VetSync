using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;

namespace VetSync.Application.Services;

// Estructura base para ejecucion de TDD (fase roja)
public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CitaService(
        ICitaRepository citaRepository,
        IMascotaRepository mascotaRepository,
        IVeterinarioRepository veterinarioRepository,
        IUnitOfWork unitOfWork)
    {
        _citaRepository = citaRepository;
        _mascotaRepository = mascotaRepository;
        _veterinarioRepository = veterinarioRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<ServiceResponse<CitaDto>> AgendarCitaAsync(CrearCitaDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<bool>> CancelarCitaAsync(int citaId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<CitaDto>> ObtenerPorIdAsync(int citaId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<IReadOnlyList<CitaDto>>> ObtenerCitasPorVeterinarioAsync(int veterinarioId, DateTime fecha, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
