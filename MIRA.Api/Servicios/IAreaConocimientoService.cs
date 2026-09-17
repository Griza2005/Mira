using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface IAreaConocimientoService
{
    Task<IEnumerable<AreaConocimiento>> GetAllActivosAsync();
    Task<AreaConocimiento?> GetByIdAsync(int id);
    Task<AreaConocimiento> CreateAsync(AreaConocimiento entity);
    Task<bool> UpdateAsync(int id, AreaConocimiento entity);
    Task<bool> SoftDeleteAsync(int id);
}

