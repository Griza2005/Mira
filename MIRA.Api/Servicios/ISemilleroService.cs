namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de lógica de negocio para semilleros de investigación (HU 6).
/// </summary>
public interface ISemilleroService
{
    Task<IEnumerable<Semillero>> GetAllActivosAsync();
    Task<Semillero?> GetByIdAsync(int id);
    Task<Semillero> CreateAsync(Semillero entity);
    Task<bool> UpdateAsync(int id, Semillero entity);
    Task<bool> SoftDeleteAsync(int id);
}