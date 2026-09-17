using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios;

public class AreaConocimientoService : IAreaConocimientoService
{
    private readonly IAreaConocimientoRepository _repository;

    public AreaConocimientoService(IAreaConocimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AreaConocimiento>> GetAllActivosAsync()
    {
        return await _repository.GetAllActivosAsync();
    }

    public async Task<AreaConocimiento?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _repository.GetByIdAsync(id);
    }

    public async Task<AreaConocimiento> CreateAsync(AreaConocimiento entity)
    {
        if (string.IsNullOrWhiteSpace(entity.GranArea))
            throw new ArgumentException("El campo 'gran_area' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Area))
            throw new ArgumentException("El campo 'area' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Disciplina))
            throw new ArgumentException("El campo 'disciplina' es obligatorio.");

        entity.GranArea = entity.GranArea.Trim();
        entity.Area = entity.Area.Trim();
        entity.Disciplina = entity.Disciplina.Trim();
        entity.Activo = true;

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, AreaConocimiento entity)
    {
        if (id <= 0) return false;
        if (string.IsNullOrWhiteSpace(entity.GranArea))
            throw new ArgumentException("El campo 'gran_area' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Area))
            throw new ArgumentException("El campo 'area' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.Disciplina))
            throw new ArgumentException("El campo 'disciplina' es obligatorio.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        entity.Id = id;
        entity.GranArea = entity.GranArea.Trim();
        entity.Area = entity.Area.Trim();
        entity.Disciplina = entity.Disciplina.Trim();

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

