namespace VetSync.Domain.Exceptions;

public class SolapamientoCitaException : DominioException
{
    public int VeterinarioId { get; }
    public DateTime FechaHoraInicio { get; }
    public DateTime FechaHoraFin { get; }

    public SolapamientoCitaException(int veterinarioId, DateTime fechaHoraInicio, DateTime fechaHoraFin)
        : base($"El veterinario con ID {veterinarioId} ya cuenta con una cita asignada en el intervalo de {fechaHoraInicio:yyyy-MM-dd HH:mm} a {fechaHoraFin:yyyy-MM-dd HH:mm}.")
    {
        VeterinarioId = veterinarioId;
        FechaHoraInicio = fechaHoraInicio;
        FechaHoraFin = fechaHoraFin;
    }
}
