using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public interface IUniversidadRepository
{
    Task<IEnumerable<Universidad>> GetAllActivosAsync();
    Task<Universidad?> GetByIdAsync(int id);
    Task<Universidad> CreateAsync(Universidad entity);
    Task<bool> UpdateAsync(Universidad entity);
    Task<bool> SoftDeleteAsync(int id);
}

