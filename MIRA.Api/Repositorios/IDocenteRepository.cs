namespace MIRA.Api.Repositorios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de acceso a datos para la entidad Docente (HU 3).
/// </summary>
public interface IDocenteRepository
{
    // Obtiene todos los docentes activos con el nombre de su línea de investigación
    Task<IEnumerable<Docente>> GetAllActivosAsync();

    // Obtiene un docente puntual por su cédula (Llave Primaria)
    Task<Docente?> GetByIdAsync(int cedula);

    // Inserta un nuevo docente en la base de datos
    Task<Docente> CreateAsync(Docente entity);

    // Actualiza la información académica y de vinculación del docente
    Task<bool> UpdateAsync(Docente entity);

    // Aplica borrado lógico (activo = false)
    Task<bool> SoftDeleteAsync(int cedula);
}