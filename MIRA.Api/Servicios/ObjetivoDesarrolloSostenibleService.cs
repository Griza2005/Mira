using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios;

public class ObjetivoDesarrolloSostenibleService : IObjetivoDesarrolloSostenibleService
{
    private readonly IObjetivoDesarrolloSostenibleRepository _repository;

    public ObjetivoDesarrolloSostenibleService(IObjetivoDesarrolloSostenibleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ObjetivoDesarrolloSostenible>> GetAllActivosAsync()
    {
        return await _repository.GetAllActivosAsync();
    }

    public async Task<ObjetivoDesarrolloSostenible?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ObjetivoDesarrolloSostenible> CreateAsync(ObjetivoDesarrolloSostenible entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Categoria))
            throw new ArgumentException("El campo 'categoria' es obligatorio.");

        entity.Nombre = entity.Nombre.Trim();
        entity.Categoria = entity.Categoria.Trim();
        entity.Activo = true;

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, ObjetivoDesarrolloSostenible entity)
    {
        if (id <= 0) return false;
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Categoria))
            throw new ArgumentException("El campo 'categoria' es obligatorio.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        entity.Id = id;
        entity.Nombre = entity.Nombre.Trim();
        entity.Categoria = entity.Categoria.Trim();

        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        if (id <= 0) return false;
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        return await _repository.SoftDeleteAsync(id);
    }
}

