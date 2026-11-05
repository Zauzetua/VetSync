using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSync.Domain.Entities;

namespace VetSync.Infrastructure.Data.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.DestinatarioCorreo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(n => n.Asunto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Mensaje)
            .IsRequired();

        builder.Property(n => n.Estado)
            .IsRequired();

        builder.Property(n => n.FechaProgramadaEnvio)
            .IsRequired();

        builder.Property(n => n.ErrorDetalle)
            .HasMaxLength(500);

        builder.HasIndex(n => new { n.Estado, n.FechaProgramadaEnvio });
    }
}
