using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface IUniversidadService
{
    Task<IEnumerable<Universidad>> GetAllActivosAsync();
    Task<Universidad?> GetByIdAsync(int id);
    Task<Universidad> CreateAsync(Universidad entity);
    Task<bool> UpdateAsync(int id, Universidad entity);
    Task<bool> SoftDeleteAsync(int id);
}

