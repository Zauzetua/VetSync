using Microsoft.EntityFrameworkCore;
using VetSync.Application.Interfaces;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Context;

public class VetSyncDbContext : DbContext, IUnitOfWork
{
    public VetSyncDbContext(DbContextOptions<VetSyncDbContext> options) : base(options)
    {
    }

    public DbSet<Dueno> Duenos => Set<Dueno>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();
    public DbSet<Veterinario> Veterinarios => Set<Veterinario>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistorialMedico> HistorialesMedicos => Set<HistorialMedico>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VetSyncDbContext).Assembly);
    }

    public async Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }
}
