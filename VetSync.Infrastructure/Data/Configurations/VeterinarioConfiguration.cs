using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class VeterinarioConfiguration : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.ToTable("Veterinarios");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.NombreCompleto)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.CedulaProfesional)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.CedulaProfesional)
            .IsUnique();

        builder.Property(v => v.CorreoElectronico)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(v => v.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.Especialidad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
