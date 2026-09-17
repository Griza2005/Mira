using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface IAreaAplicacionService
{
    Task<IEnumerable<AreaAplicacion>> GetAllActivosAsync();
    Task<AreaAplicacion?> GetByIdAsync(int id);
    Task<AreaAplicacion> CreateAsync(AreaAplicacion entity);
    Task<bool> UpdateAsync(int id, AreaAplicacion entity);
    Task<bool> SoftDeleteAsync(int id);
}

