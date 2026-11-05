using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;

namespace VetSync.Application.Services;

// Estructura base para ejecucion de TDD (fase roja)
public class MascotaService : IMascotaService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IDuenoRepository _duenoRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly IHistorialMedicoRepository _historialMedicoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MascotaService(
        IMascotaRepository mascotaRepository,
        IDuenoRepository duenoRepository,
        IVeterinarioRepository veterinarioRepository,
        IHistorialMedicoRepository historialMedicoRepository,
        IUnitOfWork unitOfWork)
    {
        _mascotaRepository = mascotaRepository;
        _duenoRepository = duenoRepository;
        _veterinarioRepository = veterinarioRepository;
        _historialMedicoRepository = historialMedicoRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<ServiceResponse<MascotaDto>> RegistrarMascotaAsync(RegistrarMascotaDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<MascotaDto>> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<IReadOnlyList<MascotaDto>>> ObtenerPorDuenoIdAsync(int duenoId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<DuenoDto>> RegistrarDuenoAsync(RegistrarDuenoDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<HistorialMedicoDto>> RegistrarHistorialMedicoAsync(RegistrarHistorialMedicoDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<IReadOnlyList<HistorialMedicoDto>>> ObtenerHistorialPorMascotaAsync(int mascotaId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
