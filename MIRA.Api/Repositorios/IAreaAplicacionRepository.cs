using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public interface IAreaAplicacionRepository
{
    Task<IEnumerable<AreaAplicacion>> GetAllActivosAsync();
    Task<AreaAplicacion?> GetByIdAsync(int id);
    Task<AreaAplicacion> CreateAsync(AreaAplicacion entity);
    Task<bool> UpdateAsync(AreaAplicacion entity);
    Task<bool> SoftDeleteAsync(int id);
}

