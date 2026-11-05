using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FechaHoraInicio)
            .IsRequired();

        builder.Property(c => c.FechaHoraFin)
            .IsRequired();

        builder.Property(c => c.Estado)
            .IsRequired();

        builder.Property(c => c.Motivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.FechaCreacion)
            .IsRequired();

        builder.HasIndex(c => new { c.VeterinarioId, c.FechaHoraInicio, c.FechaHoraFin });

        builder.HasOne(c => c.Veterinario)
            .WithMany(v => v.Citas)
            .HasForeignKey(c => c.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.HistorialMedico)
            .WithOne(h => h.Cita)
            .HasForeignKey<HistorialMedico>(h => h.CitaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Notificaciones)
            .WithOne(n => n.Cita)
            .HasForeignKey(n => n.CitaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
