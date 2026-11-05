using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class MascotaConfiguration : IEntityTypeConfiguration<Mascota>
{
    public void Configure(EntityTypeBuilder<Mascota> builder)
    {
        builder.ToTable("Mascotas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Especie)
            .IsRequired();

        builder.Property(m => m.Raza)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.FechaNacimiento)
            .HasColumnType("date");

        builder.Property(m => m.Sexo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.PesoKg)
            .HasPrecision(5, 2);

        builder.Property(m => m.FechaRegistro)
            .IsRequired();

        builder.HasMany(m => m.Citas)
            .WithOne(c => c.Mascota)
            .HasForeignKey(c => c.MascotaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.HistorialesMedicos)
            .WithOne(h => h.Mascota)
            .HasForeignKey(h => h.MascotaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
