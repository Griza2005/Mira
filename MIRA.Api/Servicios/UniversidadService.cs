using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios;

public class UniversidadService : IUniversidadService
{
    private readonly IUniversidadRepository _repository;

    public UniversidadService(IUniversidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Universidad>> GetAllActivosAsync()
    {
        return await _repository.GetAllActivosAsync();
    }

    public async Task<Universidad?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Universidad> CreateAsync(Universidad entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Tipo))
            throw new ArgumentException("El campo 'tipo' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Ciudad))
            throw new ArgumentException("El campo 'ciudad' es obligatorio.");

        entity.Nombre = entity.Nombre.Trim();
        entity.Tipo = entity.Tipo.Trim();
        entity.Ciudad = entity.Ciudad.Trim();
        entity.Activo = true;

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, Universidad entity)
    {
        if (id <= 0) return false;
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Tipo))
            throw new ArgumentException("El campo 'tipo' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Ciudad))
            throw new ArgumentException("El campo 'ciudad' es obligatorio.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        entity.Id = id;
        entity.Nombre = entity.Nombre.Trim();
        entity.Tipo = entity.Tipo.Trim();
        entity.Ciudad = entity.Ciudad.Trim();

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

