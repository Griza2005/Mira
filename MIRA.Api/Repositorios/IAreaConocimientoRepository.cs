using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public interface IAreaConocimientoRepository
{
    Task<IEnumerable<AreaConocimiento>> GetAllActivosAsync();
    Task<AreaConocimiento?> GetByIdAsync(int id);
    Task<AreaConocimiento> CreateAsync(AreaConocimiento entity);
    Task<bool> UpdateAsync(AreaConocimiento entity);
    Task<bool> SoftDeleteAsync(int id);
}

