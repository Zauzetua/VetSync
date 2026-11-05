using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class DuenoConfiguration : IEntityTypeConfiguration<Dueno>
{
    public void Configure(EntityTypeBuilder<Dueno> builder)
    {
        builder.ToTable("Duenos");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NombreCompleto)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.CorreoElectronico)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(d => d.CorreoElectronico)
            .IsUnique();

        builder.Property(d => d.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Direccion)
            .HasMaxLength(250);

        builder.Property(d => d.FechaRegistro)
            .IsRequired();

        builder.HasMany(d => d.Mascotas)
            .WithOne(m => m.Dueno)
            .HasForeignKey(m => m.DuenoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
