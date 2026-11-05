using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VetSync.Application.Interfaces;
using VetSync.Infrastructure.Data.Context;
using VetSync.Infrastructure.Data.Repositories;
using VetSync.Infrastructure.Services;

namespace VetSync.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'DefaultConnection'. Asegurate de configurarla en User Secrets o appsettings.");

        services.AddDbContext<VetSyncDbContext>(options =>
            options.UseSqlServer(cadenaConexion));

        // Unidad de trabajo
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<VetSyncDbContext>());

        // Repositorios genericos y especificos
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IMascotaRepository, MascotaRepository>();
        services.AddScoped<IDuenoRepository, DuenoRepository>();
        services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();
        services.AddScoped<IHistorialMedicoRepository, HistorialMedicoRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();

        // Adaptadores y servicios externos (simulados)
        services.AddScoped<IEmailSenderService, EnvioCorreoSimuladoService>();

        return services;
    }
}
