using VetSync.Application.Common.Models;
using VetSync.Application.Interfaces;

namespace VetSync.Application.Services;

// Estructura base para ejecucion de TDD (fase roja)
public class NotificacionService : INotificacionService
{
    private readonly ICitaRepository _citaRepository;
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IUnitOfWork _unitOfWork;

    public NotificacionService(
        ICitaRepository citaRepository,
        INotificacionRepository notificacionRepository,
        IEmailSenderService emailSenderService,
        IUnitOfWork unitOfWork)
    {
        _citaRepository = citaRepository;
        _notificacionRepository = notificacionRepository;
        _emailSenderService = emailSenderService;
        _unitOfWork = unitOfWork;
    }

    public Task<ServiceResponse<int>> EnviarRecordatoriosCitasProximasAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
