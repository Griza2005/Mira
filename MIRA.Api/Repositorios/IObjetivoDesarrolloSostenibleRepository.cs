using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public interface IObjetivoDesarrolloSostenibleRepository
{
    Task<IEnumerable<ObjetivoDesarrolloSostenible>> GetAllActivosAsync();
    Task<ObjetivoDesarrolloSostenible?> GetByIdAsync(int id);
    Task<ObjetivoDesarrolloSostenible> CreateAsync(ObjetivoDesarrolloSostenible entity);
    Task<bool> UpdateAsync(ObjetivoDesarrolloSostenible entity);
    Task<bool> SoftDeleteAsync(int id);
}

