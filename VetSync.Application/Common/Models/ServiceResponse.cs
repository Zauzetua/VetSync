namespace VetSync.Application.Common.Models;

// Modelo generico para encapsular respuestas de la capa de aplicacion
public class ServiceResponse<T>
{
    public bool Exitoso { get; set; }
    public T? Datos { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public IReadOnlyList<string> Errores { get; set; } = Array.Empty<string>();
    public int CodigoEstado { get; set; }

    public static ServiceResponse<T> Exito(T datos, string mensaje = "Operacion completada exitosamente", int codigoEstado = 200)
    {
        return new ServiceResponse<T>
        {
            Exitoso = true,
            Datos = datos,
            Mensaje = mensaje,
            CodigoEstado = codigoEstado
        };
    }

    public static ServiceResponse<T> Fallo(string mensaje, IReadOnlyList<string>? errores = null, int codigoEstado = 400)
    {
        return new ServiceResponse<T>
        {
            Exitoso = false,
            Datos = default,
            Mensaje = mensaje,
            Errores = errores ?? Array.Empty<string>(),
            CodigoEstado = codigoEstado
        };
    }
}

// Sobrecarga no generica para operaciones sin payload de retorno
public class ServiceResponse : ServiceResponse<bool>
{
    public static ServiceResponse ExitoSinDatos(string mensaje = "Operacion completada exitosamente", int codigoEstado = 200)
    {
        return new ServiceResponse
        {
            Exitoso = true,
            Datos = true,
            Mensaje = mensaje,
            CodigoEstado = codigoEstado
        };
    }

    public static new ServiceResponse Fallo(string mensaje, IReadOnlyList<string>? errores = null, int codigoEstado = 400)
    {
        return new ServiceResponse
        {
            Exitoso = false,
            Datos = false,
            Mensaje = mensaje,
            Errores = errores ?? Array.Empty<string>(),
            CodigoEstado = codigoEstado
        };
    }
}
