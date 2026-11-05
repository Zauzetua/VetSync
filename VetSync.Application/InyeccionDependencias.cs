using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VetSync.Application.Interfaces;
using VetSync.Application.Services;

namespace VetSync.Application;

public static class InyeccionDependencias
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Validadores FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Servicios de aplicacion (Casos de uso)
        services.AddScoped<ICitaService, CitaService>();
        services.AddScoped<IMascotaService, MascotaService>();
        services.AddScoped<INotificacionService, NotificacionService>();

        return services;
    }
}
