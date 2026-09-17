using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public interface ILineaInvestigacionRepository
{
    Task<IEnumerable<LineaInvestigacion>> GetAllActivosAsync();
    Task<LineaInvestigacion?> GetByIdAsync(int id);
    Task<LineaInvestigacion> CreateAsync(LineaInvestigacion entity);
    Task<bool> UpdateAsync(LineaInvestigacion entity);
    Task<bool> SoftDeleteAsync(int id);
}

