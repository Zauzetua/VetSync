using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace VetSync.Application;

public static class InyeccionDependencias
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registro automatico de validadores FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
