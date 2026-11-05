using FluentAssertions;
using Moq;
using VetSync.Application.Dtos;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;
using VetSync.Domain.Entities;
using VetSync.Domain.Enums;
using Xunit;

namespace VetSync.Tests.Services;

public class MascotaServiceConsultasTests
{
    private readonly Mock<IMascotaRepository> _mascotaRepositoryMock;
    private readonly Mock<IDuenoRepository> _duenoRepositoryMock;
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly Mock<IHistorialMedicoRepository> _historialMedicoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MascotaService _mascotaService;

    public MascotaServiceConsultasTests()
    {
        _mascotaRepositoryMock = new Mock<IMascotaRepository>();
        _duenoRepositoryMock = new Mock<IDuenoRepository>();
        _veterinarioRepositoryMock = new Mock<IVeterinarioRepository>();
        _historialMedicoRepositoryMock = new Mock<IHistorialMedicoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _mascotaService = new MascotaService(
            _mascotaRepositoryMock.Object,
            _duenoRepositoryMock.Object,
            _veterinarioRepositoryMock.Object,
            _historialMedicoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoExiste_DebeRetornarMascota()
    {
        var mascota = new Mascota
        {
            Id = 3,
            Nombre = "Toby",
            Especie = EspecieMascota.Perro,
            Raza = "Beagle",
            Dueno = new Dueno { NombreCompleto = "Pedro Sanchez" }
        };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mascota);

        var resultado = await _mascotaService.ObtenerPorIdAsync(3);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos!.Nombre.Should().Be("Toby");
        resultado.Datos.NombreDueno.Should().Be("Pedro Sanchez");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoNoExiste_DebeRetornarFallo()
    {
        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mascota?)null);

        var resultado = await _mascotaService.ObtenerPorIdAsync(99);

        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorDuenoIdAsync_DebeRetornarMascotas()
    {
        var lista = new List<Mascota>
        {
            new Mascota { Id = 1, Nombre = "Pelusa", Dueno = new Dueno { NombreCompleto = "Maria" } }
        };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorDuenoIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        var resultado = await _mascotaService.ObtenerPorDuenoIdAsync(1);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().HaveCount(1);
    }

    [Fact]
    public async Task RegistrarDuenoAsync_CuandoDatosSonValidos_DebeRegistrarExitosamente()
    {
        var dto = new RegistrarDuenoDto
        {
            NombreCompleto = "Martin Diaz",
            CorreoElectronico = "martin@test.com",
            Telefono = "5551234567"
        };

        _duenoRepositoryMock.Setup(r => r.ObtenerPorCorreoAsync(dto.CorreoElectronico, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dueno?)null);

        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _mascotaService.RegistrarDuenoAsync(dto);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos!.NombreCompleto.Should().Be("Martin Diaz");
        _duenoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<Dueno>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarDuenoAsync_CuandoCorreoEstaDuplicado_DebeRetornarFallo409()
    {
        var dto = new RegistrarDuenoDto
        {
            NombreCompleto = "Martin Diaz",
            CorreoElectronico = "martin@test.com"
        };

        _duenoRepositoryMock.Setup(r => r.ObtenerPorCorreoAsync(dto.CorreoElectronico, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dueno { Id = 10, CorreoElectronico = dto.CorreoElectronico });

        var resultado = await _mascotaService.RegistrarDuenoAsync(dto);

        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(409);
    }

    [Fact]
    public async Task RegistrarHistorialMedicoAsync_CuandoVeterinarioNoExiste_DebeRetornarFallo404()
    {
        var dto = new RegistrarHistorialMedicoDto
        {
            MascotaId = 1,
            VeterinarioId = 888,
            Diagnostico = "Dolor muscular",
            Tratamiento = "Antiinflamatorio"
        };

        _mascotaRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.MascotaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mascota { Id = 1, Nombre = "Coco" });

        _veterinarioRepositoryMock.Setup(r => r.ObtenerPorIdAsync(dto.VeterinarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veterinario?)null);

        var resultado = await _mascotaService.RegistrarHistorialMedicoAsync(dto);

        resultado.Exitoso.Should().BeFalse();
        resultado.CodigoEstado.Should().Be(404);
        resultado.Mensaje.ToLower().Should().Contain("veterinario");
    }

    [Fact]
    public async Task ObtenerHistorialPorMascotaAsync_DebeRetornarListaDeHistoriales()
    {
        var lista = new List<HistorialMedico>
        {
            new HistorialMedico
            {
                Id = 1,
                MascotaId = 1,
                Diagnostico = "Chequeo sano",
                Tratamiento = "Ninguno",
                Veterinario = new Veterinario { NombreCompleto = "Dra. Gomez" },
                Mascota = new Mascota { Nombre = "Coco" }
            }
        };

        _historialMedicoRepositoryMock.Setup(r => r.ObtenerPorMascotaIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        var resultado = await _mascotaService.ObtenerHistorialPorMascotaAsync(1);

        resultado.Exitoso.Should().BeTrue();
        resultado.Datos.Should().HaveCount(1);
    }
}
