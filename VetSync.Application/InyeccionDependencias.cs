using Microsoft.Extensions.DependencyInjection;

namespace VetSync.Application;

public static class InyeccionDependencias
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Aquí se registrarán los servicios de aplicación (Casos de uso)
        return services;
    }
}
