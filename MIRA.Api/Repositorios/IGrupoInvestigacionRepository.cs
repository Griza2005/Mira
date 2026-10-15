namespace MIRA.Api.Repositorios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de acceso a datos para Grupos de Investigación (HU 5).
/// </summary>
public interface IGrupoInvestigacionRepository
{
    // Obtiene todos los grupos activos enriquecidos con nombre de sede y líder
    Task<IEnumerable<GrupoInvestigacion>> GetAllActivosAsync();

    // Obtiene un grupo puntual por su ID
    Task<GrupoInvestigacion?> GetByIdAsync(int id);

    // Inserta un nuevo grupo de investigación
    Task<GrupoInvestigacion> CreateAsync(GrupoInvestigacion entity);

    // Actualiza los datos institucionales del grupo
    Task<bool> UpdateAsync(GrupoInvestigacion entity);

    // Ejecuta el borrado lógico (activo = false)
    Task<bool> SoftDeleteAsync(int id);
}