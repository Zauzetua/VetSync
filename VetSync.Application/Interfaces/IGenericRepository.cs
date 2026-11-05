namespace VetSync.Application.Interfaces;

public interface IGenericRepository<TEntidad> where TEntidad : class
{
    Task<TEntidad?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(TEntidad entidad, CancellationToken cancellationToken = default);
    void Actualizar(TEntidad entidad);
    void Eliminar(TEntidad entidad);
}
