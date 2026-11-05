namespace VetSync.Domain.Exceptions;

public class EntidadNoEncontradaException : DominioException
{
    public string NombreEntidad { get; }
    public object Clave { get; }

    public EntidadNoEncontradaException(string nombreEntidad, object clave)
        : base($"No se encontró la entidad '{nombreEntidad}' con identificador '{clave}'.")
    {
        NombreEntidad = nombreEntidad;
        Clave = clave;
    }
}
