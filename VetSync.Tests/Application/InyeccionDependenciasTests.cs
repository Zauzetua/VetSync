using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VetSync.Application;
using VetSync.Application.Interfaces;
using Xunit;

namespace VetSync.Tests.Application;

public class InyeccionDependenciasTests
{
    [Fact]
    public void AddApplicationServices_DebeRegistrarServiciosCorrectamente()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        services.Should().Contain(s => s.ServiceType == typeof(ICitaService));
        services.Should().Contain(s => s.ServiceType == typeof(IMascotaService));
        services.Should().Contain(s => s.ServiceType == typeof(INotificacionService));
    }
}
