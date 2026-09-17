using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios;

public class LineaInvestigacionService : ILineaInvestigacionService
{
    private readonly ILineaInvestigacionRepository _repository;

    public LineaInvestigacionService(ILineaInvestigacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LineaInvestigacion>> GetAllActivosAsync()
    {
        return await _repository.GetAllActivosAsync();
    }

    public async Task<LineaInvestigacion?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _repository.GetByIdAsync(id);
    }

    public async Task<LineaInvestigacion> CreateAsync(LineaInvestigacion entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        entity.Nombre = entity.Nombre.Trim();
        entity.Descripcion = entity.Descripcion?.Trim();
        entity.Activo = true;

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, LineaInvestigacion entity)
    {
        if (id <= 0) return false;
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        entity.Id = id;
        entity.Nombre = entity.Nombre.Trim();
        entity.Descripcion = entity.Descripcion?.Trim();

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

