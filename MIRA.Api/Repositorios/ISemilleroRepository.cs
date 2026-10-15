namespace MIRA.Api.Repositorios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de acceso a datos para Semilleros de Investigación (HU 6).
/// </summary>
public interface ISemilleroRepository
{
    // Obtiene todos los semilleros activos con el nombre de su grupo base
    Task<IEnumerable<Semillero>> GetAllActivosAsync();

    // Obtiene un semillero puntual por su ID
    Task<Semillero?> GetByIdAsync(int id);

    // Inserta un nuevo semillero
    Task<Semillero> CreateAsync(Semillero entity);

    // Actualiza los datos del semillero
    Task<bool> UpdateAsync(Semillero entity);

    // Ejecuta el borrado lógico (activo = false)
    Task<bool> SoftDeleteAsync(int id);
}