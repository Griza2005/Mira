namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

/// <summary>
/// Implementación del servicio de semilleros con validación de existencia del grupo base.
/// </summary>
public class SemilleroService : ISemilleroService
{
    private readonly ISemilleroRepository _semilleroRepository;
    private readonly IGrupoInvestigacionRepository _grupoRepository;

    public SemilleroService(
        ISemilleroRepository semilleroRepository,
        IGrupoInvestigacionRepository grupoRepository)
    {
        _semilleroRepository = semilleroRepository;
        _grupoRepository = grupoRepository;
    }

    public async Task<IEnumerable<Semillero>> GetAllActivosAsync()
    {
        return await _semilleroRepository.GetAllActivosAsync();
    }

    public async Task<Semillero?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _semilleroRepository.GetByIdAsync(id);
    }

    public async Task<Semillero> CreateAsync(Semillero entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        // Validación de Clave Foránea: Grupo Base
        var grupo = await _grupoRepository.GetByIdAsync(entity.GrupoInvestigacionId);
        if (grupo == null || !grupo.Activo)
        {
            throw new ArgumentException($"El grupo de investigación base (ID {entity.GrupoInvestigacionId}) no existe o se encuentra inactivo.");
        }

        entity.Nombre = entity.Nombre.Trim();
        entity.Activo = true;

        return await _semilleroRepository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, Semillero entity)
    {
        if (id <= 0) return false;

        var existing = await _semilleroRepository.GetByIdAsync(id);
        if (existing == null) return false;

        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        var grupo = await _grupoRepository.GetByIdAsync(entity.GrupoInvestigacionId);
        if (grupo == null || !grupo.Activo)
        {
            throw new ArgumentException($"El grupo de investigación base (ID {entity.GrupoInvestigacionId}) no existe o se encuentra inactivo.");
        }

        entity.Id = id;
        entity.Nombre = entity.Nombre.Trim();

        return await _semilleroRepository.UpdateAsync(entity);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        if (id <= 0) return false;

        var existing = await _semilleroRepository.GetByIdAsync(id);
        if (existing == null) return false;

        return await _semilleroRepository.SoftDeleteAsync(id);
    }
}