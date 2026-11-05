using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class HistorialMedicoConfiguration : IEntityTypeConfiguration<HistorialMedico>
{
    public void Configure(EntityTypeBuilder<HistorialMedico> builder)
    {
        builder.ToTable("HistorialesMedicos");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.FechaRegistro)
            .IsRequired();

        builder.Property(h => h.Diagnostico)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(h => h.Tratamiento)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(h => h.Observaciones)
            .HasMaxLength(2000);

        builder.HasOne(h => h.Veterinario)
            .WithMany(v => v.HistorialesMedicos)
            .HasForeignKey(h => h.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
