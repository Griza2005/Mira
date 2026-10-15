namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

/// <summary>
/// Implementación del servicio de grupos con validación de existencia de sede y líder activo.
/// </summary>
public class GrupoInvestigacionService : IGrupoInvestigacionService
{
    private readonly IGrupoInvestigacionRepository _grupoRepository;
    private readonly IUniversidadRepository _universidadRepository;
    private readonly IDocenteRepository _docenteRepository;

    public GrupoInvestigacionService(
        IGrupoInvestigacionRepository grupoRepository,
        IUniversidadRepository universidadRepository,
        IDocenteRepository docenteRepository)
    {
        _grupoRepository = grupoRepository;
        _universidadRepository = universidadRepository;
        _docenteRepository = docenteRepository;
    }

    public async Task<IEnumerable<GrupoInvestigacion>> GetAllActivosAsync()
    {
        return await _grupoRepository.GetAllActivosAsync();
    }

    public async Task<GrupoInvestigacion?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;
        return await _grupoRepository.GetByIdAsync(id);
    }

    public async Task<GrupoInvestigacion> CreateAsync(GrupoInvestigacion entity)
    {
        // 1. Validaciones de campos obligatorios
        if (string.IsNullOrWhiteSpace(entity.Codigo))
            throw new ArgumentException("El campo 'codigo' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        // 2. Validación de Clave Foránea: Sede Universitaria
        var universidad = await _universidadRepository.GetByIdAsync(entity.UniversidadId);
        if (universidad == null || !universidad.Activo)
        {
            throw new ArgumentException($"La sede universitaria (ID {entity.UniversidadId}) no existe o se encuentra inactiva.");
        }

        // 3. Validación de Clave Foránea: Docente Líder
        var docente = await _docenteRepository.GetByIdAsync(entity.DocenteLiderCedula);
        if (docente == null || !docente.Activo)
        {
            throw new ArgumentException($"El docente asignado como líder (Cédula {entity.DocenteLiderCedula}) no existe o se encuentra inactivo.");
        }

        entity.Codigo = entity.Codigo.Trim().ToUpperInvariant();
        entity.Nombre = entity.Nombre.Trim();
        entity.Activo = true;

        return await _grupoRepository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, GrupoInvestigacion entity)
    {
        if (id <= 0) return false;

        var existing = await _grupoRepository.GetByIdAsync(id);
        if (existing == null) return false;

        if (string.IsNullOrWhiteSpace(entity.Codigo))
            throw new ArgumentException("El campo 'codigo' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El campo 'nombre' es obligatorio.");

        // Validación de FK: Sede
        var universidad = await _universidadRepository.GetByIdAsync(entity.UniversidadId);
        if (universidad == null || !universidad.Activo)
        {
            throw new ArgumentException($"La sede universitaria (ID {entity.UniversidadId}) no existe o se encuentra inactiva.");
        }

        // Validación de FK: Docente Líder
        var docente = await _docenteRepository.GetByIdAsync(entity.DocenteLiderCedula);
        if (docente == null || !docente.Activo)
        {
            throw new ArgumentException($"El docente asignado como líder (Cédula {entity.DocenteLiderCedula}) no existe o se encuentra inactivo.");
        }

        entity.Id = id;
        entity.Codigo = entity.Codigo.Trim().ToUpperInvariant();
        entity.Nombre = entity.Nombre.Trim();

        return await _grupoRepository.UpdateAsync(entity);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        if (id <= 0) return false;

        var existing = await _grupoRepository.GetByIdAsync(id);
        if (existing == null) return false;

        return await _grupoRepository.SoftDeleteAsync(id);
    }
}