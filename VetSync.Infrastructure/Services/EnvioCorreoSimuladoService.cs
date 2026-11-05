using Microsoft.Extensions.Logging;
using VetSync.Application.Interfaces;

namespace VetSync.Infrastructure.Services;

public class EnvioCorreoSimuladoService : IEmailSenderService
{
    private readonly ILogger<EnvioCorreoSimuladoService> _logger;

    public EnvioCorreoSimuladoService(ILogger<EnvioCorreoSimuladoService> logger)
    {
        _logger = logger;
    }

    public Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[NOTIFICACION SIMULADA] Para: {Destinatario} | Asunto: {Asunto} | Contenido: {Cuerpo}",
            destinatario,
            asunto,
            cuerpoHtml);

        return Task.CompletedTask;
    }
}
