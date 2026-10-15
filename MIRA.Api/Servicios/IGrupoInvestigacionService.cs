namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de lógica de negocio para grupos de investigación (HU 5).
/// </summary>
public interface IGrupoInvestigacionService
{
    Task<IEnumerable<GrupoInvestigacion>> GetAllActivosAsync();
    Task<GrupoInvestigacion?> GetByIdAsync(int id);
    Task<GrupoInvestigacion> CreateAsync(GrupoInvestigacion entity);
    Task<bool> UpdateAsync(int id, GrupoInvestigacion entity);
    Task<bool> SoftDeleteAsync(int id);
}