namespace VetSync.Application.Interfaces;

public interface IEmailSenderService
{
    Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken cancellationToken = default);
}
