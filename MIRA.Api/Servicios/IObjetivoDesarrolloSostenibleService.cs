using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface IObjetivoDesarrolloSostenibleService
{
    Task<IEnumerable<ObjetivoDesarrolloSostenible>> GetAllActivosAsync();
    Task<ObjetivoDesarrolloSostenible?> GetByIdAsync(int id);
    Task<ObjetivoDesarrolloSostenible> CreateAsync(ObjetivoDesarrolloSostenible entity);
    Task<bool> UpdateAsync(int id, ObjetivoDesarrolloSostenible entity);
    Task<bool> SoftDeleteAsync(int id);
}

