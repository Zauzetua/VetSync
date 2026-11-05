using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;

namespace VetSync.Application.Services;

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

    public async Task<ServiceResponse<CitaDto>> AgendarCitaAsync(CrearCitaDto dto, CancellationToken cancellationToken = default)
    {
        // Validacion de fecha en el pasado
        if (dto.FechaHoraInicio <= DateTime.UtcNow)
        {
            return ServiceResponse<CitaDto>.Fallo(
                "No se puede programar una cita en una fecha u hora en el pasado.",
                null,
                400);
        }

        // Validacion de existencia de mascota
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(dto.MascotaId, cancellationToken);
        if (mascota == null)
        {
            return ServiceResponse<CitaDto>.Fallo(
                $"No se encontro la mascota con identificador {dto.MascotaId}.",
                null,
                404);
        }

        // Validacion de existencia y estado del veterinario
        var veterinario = await _veterinarioRepository.ObtenerPorIdAsync(dto.VeterinarioId, cancellationToken);
        if (veterinario == null || !veterinario.Activo)
        {
            return ServiceResponse<CitaDto>.Fallo(
                $"El veterinario con identificador {dto.VeterinarioId} no existe o no se encuentra activo.",
                null,
                404);
        }

        var fechaHoraFin = dto.FechaHoraInicio.AddMinutes(dto.DuracionMinutos);

        // Validacion de choque o solapamiento de horario para el veterinario
        var existeSolapamiento = await _citaRepository.ExisteSolapamientoAsync(
            dto.VeterinarioId,
            dto.FechaHoraInicio,
            fechaHoraFin,
            null,
            cancellationToken);

        if (existeSolapamiento)
        {
            return ServiceResponse<CitaDto>.Fallo(
                $"El veterinario ya cuenta con una cita en el rango de horario solicitado (solapamiento detectado).",
                null,
                409);
        }

        var nuevaCita = new Cita
        {
            MascotaId = dto.MascotaId,
            VeterinarioId = dto.VeterinarioId,
            FechaHoraInicio = dto.FechaHoraInicio,
            FechaHoraFin = fechaHoraFin,
            Estado = EstadoCita.Programada,
            Motivo = dto.Motivo,
            FechaCreacion = DateTime.UtcNow
        };

        await _citaRepository.AgregarAsync(nuevaCita, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        var respuestaDto = new CitaDto
        {
            Id = nuevaCita.Id,
            MascotaId = nuevaCita.MascotaId,
            NombreMascota = mascota.Nombre,
            VeterinarioId = nuevaCita.VeterinarioId,
            NombreVeterinario = veterinario.NombreCompleto,
            FechaHoraInicio = nuevaCita.FechaHoraInicio,
            FechaHoraFin = nuevaCita.FechaHoraFin,
            Estado = nuevaCita.Estado,
            Motivo = nuevaCita.Motivo
        };

        return ServiceResponse<CitaDto>.Exito(respuestaDto, "Cita agendada exitosamente.", 201);
    }

    public async Task<ServiceResponse<bool>> CancelarCitaAsync(int citaId, CancellationToken cancellationToken = default)
    {
        var cita = await _citaRepository.ObtenerPorIdAsync(citaId, cancellationToken);
        if (cita == null)
        {
            return ServiceResponse.Fallo($"No se encontro la cita con identificador {citaId}.", null, 404);
        }

        if (cita.Estado == EstadoCita.Cancelada || cita.Estado == EstadoCita.Completada)
        {
            return ServiceResponse.Fallo(
                $"La cita no puede cancelarse porque ya se encuentra en estado {cita.Estado}.",
                null,
                400);
        }

        cita.Estado = EstadoCita.Cancelada;
        _citaRepository.Actualizar(cita);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return ServiceResponse.ExitoSinDatos("Cita cancelada exitosamente.");
    }

    public async Task<ServiceResponse<CitaDto>> ObtenerPorIdAsync(int citaId, CancellationToken cancellationToken = default)
    {
        var cita = await _citaRepository.ObtenerPorIdAsync(citaId, cancellationToken);
        if (cita == null)
        {
            return ServiceResponse<CitaDto>.Fallo($"No se encontro la cita con identificador {citaId}.", null, 404);
        }

        var dto = new CitaDto
        {
            Id = cita.Id,
            MascotaId = cita.MascotaId,
            NombreMascota = cita.Mascota?.Nombre ?? string.Empty,
            NombreDueno = cita.Mascota?.Dueno?.NombreCompleto ?? string.Empty,
            CorreoDueno = cita.Mascota?.Dueno?.CorreoElectronico ?? string.Empty,
            VeterinarioId = cita.VeterinarioId,
            NombreVeterinario = cita.Veterinario?.NombreCompleto ?? string.Empty,
            FechaHoraInicio = cita.FechaHoraInicio,
            FechaHoraFin = cita.FechaHoraFin,
            Estado = cita.Estado,
            Motivo = cita.Motivo
        };

        return ServiceResponse<CitaDto>.Exito(dto);
    }

    public async Task<ServiceResponse<IReadOnlyList<CitaDto>>> ObtenerCitasPorVeterinarioAsync(int veterinarioId, DateTime fecha, CancellationToken cancellationToken = default)
    {
        var inicio = fecha.Date;
        var fin = inicio.AddDays(1);

        var citas = await _citaRepository.ObtenerPorVeterinarioYRangoAsync(veterinarioId, inicio, fin, cancellationToken);

        var listaDto = citas.Select(c => new CitaDto
        {
            Id = c.Id,
            MascotaId = c.MascotaId,
            NombreMascota = c.Mascota?.Nombre ?? string.Empty,
            VeterinarioId = c.VeterinarioId,
            NombreVeterinario = c.Veterinario?.NombreCompleto ?? string.Empty,
            FechaHoraInicio = c.FechaHoraInicio,
            FechaHoraFin = c.FechaHoraFin,
            Estado = c.Estado,
            Motivo = c.Motivo
        }).ToList();

        return ServiceResponse<IReadOnlyList<CitaDto>>.Exito(listaDto);
    }
}
