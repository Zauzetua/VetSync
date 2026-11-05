using System.Net;
using System.Text.Json;
using VetSync.Domain.Exceptions;

namespace VetSync.Middlewares;

public class ManejadorExcepcionesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ManejadorExcepcionesMiddleware(
        RequestDelegate next,
        ILogger<ManejadorExcepcionesMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ManejarExcepcionAsync(context, ex);
        }
    }

    private Task ManejarExcepcionAsync(HttpContext context, Exception exception)
    {
        var codigoEstado = HttpStatusCode.InternalServerError;
        var titulo = "Ha ocurrido un error inesperado en el servidor.";
        var detalle = _environment.IsDevelopment() ? exception.Message : null;

        switch (exception)
        {
            case SolapamientoCitaException solapamientoEx:
                codigoEstado = HttpStatusCode.Conflict;
                titulo = "Conflicto en horario de cita";
                detalle = solapamientoEx.Message;
                _logger.LogWarning(solapamientoEx, "Conflicto de solapamiento detectado para el veterinario {VeterinarioId}", solapamientoEx.VeterinarioId);
                break;

            case EntidadNoEncontradaException noEncontradaEx:
                codigoEstado = HttpStatusCode.NotFound;
                titulo = "Recurso no encontrado";
                detalle = noEncontradaEx.Message;
                _logger.LogWarning(noEncontradaEx, "Entidad {Entidad} no encontrada con clave {Clave}", noEncontradaEx.NombreEntidad, noEncontradaEx.Clave);
                break;

            case DominioException dominioEx:
                codigoEstado = HttpStatusCode.BadRequest;
                titulo = "Regla de negocio no cumplida";
                detalle = dominioEx.Message;
                _logger.LogWarning(dominioEx, "Violación de regla de dominio: {Mensaje}", dominioEx.Message);
                break;

            default:
                _logger.LogError(exception, "Error no controlado durante la solicitud {Path}", context.Request.Path);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)codigoEstado;

        var respuesta = new
        {
            Estatus = (int)codigoEstado,
            Titulo = titulo,
            Detalle = detalle,
            Ruta = context.Request.Path.Value,
            InstanteUtc = DateTime.UtcNow
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
    }
}
