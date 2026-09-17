using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface ILineaInvestigacionService
{
    Task<IEnumerable<LineaInvestigacion>> GetAllActivosAsync();
    Task<LineaInvestigacion?> GetByIdAsync(int id);
    Task<LineaInvestigacion> CreateAsync(LineaInvestigacion entity);
    Task<bool> UpdateAsync(int id, LineaInvestigacion entity);
    Task<bool> SoftDeleteAsync(int id);
}

