using VetSync.Application.Common.Models;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;

namespace VetSync.Application.Services;

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

    public async Task<ServiceResponse<MascotaDto>> RegistrarMascotaAsync(RegistrarMascotaDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.PesoKg <= 0)
        {
            return ServiceResponse<MascotaDto>.Fallo(
                "El peso de la mascota debe ser mayor a cero kilogramos.",
                null,
                400);
        }

        var dueno = await _duenoRepository.ObtenerPorIdAsync(dto.DuenoId, cancellationToken);
        if (dueno == null)
        {
            return ServiceResponse<MascotaDto>.Fallo(
                $"No se encontro al dueno con identificador {dto.DuenoId}.",
                null,
                404);
        }

        var mascota = new Mascota
        {
            DuenoId = dto.DuenoId,
            Nombre = dto.Nombre,
            Especie = dto.Especie,
            Raza = dto.Raza,
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = dto.Sexo,
            PesoKg = dto.PesoKg,
            FechaRegistro = DateTime.UtcNow
        };

        await _mascotaRepository.AgregarAsync(mascota, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        var respuestaDto = new MascotaDto
        {
            Id = mascota.Id,
            DuenoId = mascota.DuenoId,
            NombreDueno = dueno.NombreCompleto,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie,
            Raza = mascota.Raza,
            FechaNacimiento = mascota.FechaNacimiento,
            Sexo = mascota.Sexo,
            PesoKg = mascota.PesoKg
        };

        return ServiceResponse<MascotaDto>.Exito(respuestaDto, "Mascota registrada exitosamente.", 201);
    }

    public async Task<ServiceResponse<MascotaDto>> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (mascota == null)
        {
            return ServiceResponse<MascotaDto>.Fallo($"No se encontro la mascota con identificador {id}.", null, 404);
        }

        var dto = new MascotaDto
        {
            Id = mascota.Id,
            DuenoId = mascota.DuenoId,
            NombreDueno = mascota.Dueno?.NombreCompleto ?? string.Empty,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie,
            Raza = mascota.Raza,
            FechaNacimiento = mascota.FechaNacimiento,
            Sexo = mascota.Sexo,
            PesoKg = mascota.PesoKg
        };

        return ServiceResponse<MascotaDto>.Exito(dto);
    }

    public async Task<ServiceResponse<IReadOnlyList<MascotaDto>>> ObtenerPorDuenoIdAsync(int duenoId, CancellationToken cancellationToken = default)
    {
        var mascotas = await _mascotaRepository.ObtenerPorDuenoIdAsync(duenoId, cancellationToken);
        var lista = mascotas.Select(m => new MascotaDto
        {
            Id = m.Id,
            DuenoId = m.DuenoId,
            NombreDueno = m.Dueno?.NombreCompleto ?? string.Empty,
            Nombre = m.Nombre,
            Especie = m.Especie,
            Raza = m.Raza,
            FechaNacimiento = m.FechaNacimiento,
            Sexo = m.Sexo,
            PesoKg = m.PesoKg
        }).ToList();

        return ServiceResponse<IReadOnlyList<MascotaDto>>.Exito(lista);
    }

    public async Task<ServiceResponse<DuenoDto>> RegistrarDuenoAsync(RegistrarDuenoDto dto, CancellationToken cancellationToken = default)
    {
        var duenoExistente = await _duenoRepository.ObtenerPorCorreoAsync(dto.CorreoElectronico, cancellationToken);
        if (duenoExistente != null)
        {
            return ServiceResponse<DuenoDto>.Fallo(
                $"Ya existe un dueno registrado con el correo electronico '{dto.CorreoElectronico}'.",
                null,
                409);
        }

        var nuevoDueno = new Dueno
        {
            NombreCompleto = dto.NombreCompleto,
            CorreoElectronico = dto.CorreoElectronico,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            FechaRegistro = DateTime.UtcNow
        };

        await _duenoRepository.AgregarAsync(nuevoDueno, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        var respuestaDto = new DuenoDto
        {
            Id = nuevoDueno.Id,
            NombreCompleto = nuevoDueno.NombreCompleto,
            CorreoElectronico = nuevoDueno.CorreoElectronico,
            Telefono = nuevoDueno.Telefono,
            Direccion = nuevoDueno.Direccion
        };

        return ServiceResponse<DuenoDto>.Exito(respuestaDto, "Dueno registrado exitosamente.", 201);
    }

    public async Task<ServiceResponse<HistorialMedicoDto>> RegistrarHistorialMedicoAsync(RegistrarHistorialMedicoDto dto, CancellationToken cancellationToken = default)
    {
        var mascota = await _mascotaRepository.ObtenerPorIdAsync(dto.MascotaId, cancellationToken);
        if (mascota == null)
        {
            return ServiceResponse<HistorialMedicoDto>.Fallo(
                $"No se encontro la mascota con identificador {dto.MascotaId}.",
                null,
                404);
        }

        var veterinario = await _veterinarioRepository.ObtenerPorIdAsync(dto.VeterinarioId, cancellationToken);
        if (veterinario == null)
        {
            return ServiceResponse<HistorialMedicoDto>.Fallo(
                $"No se encontro al veterinario con identificador {dto.VeterinarioId}.",
                null,
                404);
        }

        var historial = new HistorialMedico
        {
            MascotaId = dto.MascotaId,
            VeterinarioId = dto.VeterinarioId,
            CitaId = dto.CitaId,
            FechaRegistro = DateTime.UtcNow,
            Diagnostico = dto.Diagnostico,
            Tratamiento = dto.Tratamiento,
            Observaciones = dto.Observaciones
        };

        await _historialMedicoRepository.AgregarAsync(historial, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        var respuestaDto = new HistorialMedicoDto
        {
            Id = historial.Id,
            MascotaId = historial.MascotaId,
            NombreMascota = mascota.Nombre,
            VeterinarioId = historial.VeterinarioId,
            NombreVeterinario = veterinario.NombreCompleto,
            CitaId = historial.CitaId,
            FechaRegistro = historial.FechaRegistro,
            Diagnostico = historial.Diagnostico,
            Tratamiento = historial.Tratamiento,
            Observaciones = historial.Observaciones
        };

        return ServiceResponse<HistorialMedicoDto>.Exito(respuestaDto, "Historial medico registrado exitosamente.", 201);
    }

    public async Task<ServiceResponse<IReadOnlyList<HistorialMedicoDto>>> ObtenerHistorialPorMascotaAsync(int mascotaId, CancellationToken cancellationToken = default)
    {
        var historiales = await _historialMedicoRepository.ObtenerPorMascotaIdAsync(mascotaId, cancellationToken);
        var lista = historiales.Select(h => new HistorialMedicoDto
        {
            Id = h.Id,
            MascotaId = h.MascotaId,
            NombreMascota = h.Mascota?.Nombre ?? string.Empty,
            VeterinarioId = h.VeterinarioId,
            NombreVeterinario = h.Veterinario?.NombreCompleto ?? string.Empty,
            CitaId = h.CitaId,
            FechaRegistro = h.FechaRegistro,
            Diagnostico = h.Diagnostico,
            Tratamiento = h.Tratamiento,
            Observaciones = h.Observaciones
        }).ToList();

        return ServiceResponse<IReadOnlyList<HistorialMedicoDto>>.Exito(lista);
    }
}
