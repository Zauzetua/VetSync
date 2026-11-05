using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VetSync.Application.Interfaces;
using VetSync.Infrastructure.Data.Context;

namespace VetSync.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'. Asegúrate de configurarla en User Secrets o appsettings.");

        services.AddDbContext<VetSyncDbContext>(options =>
            options.UseSqlServer(cadenaConexion));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<VetSyncDbContext>());

        return services;
    }
}
