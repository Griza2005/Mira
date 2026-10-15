namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

/// <summary>
/// Implementación del servicio de docentes con validación de clave foránea activa.
/// </summary>
public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly ILineaInvestigacionRepository _lineaRepository;

    public DocenteService(
        IDocenteRepository docenteRepository, 
        ILineaInvestigacionRepository lineaRepository)
    {
        _docenteRepository = docenteRepository;
        _lineaRepository = lineaRepository;
    }

    public async Task<IEnumerable<Docente>> GetAllActivosAsync()
    {
        return await _docenteRepository.GetAllActivosAsync();
    }

    public async Task<Docente?> GetByIdAsync(int cedula)
    {
        if (cedula <= 0) return null;
        return await _docenteRepository.GetByIdAsync(cedula);
    }

    public async Task<Docente> CreateAsync(Docente entity)
    {
        // 1. Validaciones de campos obligatorios
        if (entity.Cedula <= 0)
            throw new ArgumentException("El número de cédula debe ser un entero positivo válido.");

        if (string.IsNullOrWhiteSpace(entity.Nombres))
            throw new ArgumentException("El campo 'nombres' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Apellidos))
            throw new ArgumentException("El campo 'apellidos' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Correo))
            throw new ArgumentException("El campo 'correo' es obligatorio.");

        // 2. Validación de Integridad Referencial (Clave Foránea)
        if (entity.LineaInvestigacionPrincipal.HasValue)
        {
            var linea = await _lineaRepository.GetByIdAsync(entity.LineaInvestigacionPrincipal.Value);
            if (linea == null || !linea.Activo)
            {
                throw new ArgumentException($"La línea de investigación con ID {entity.LineaInvestigacionPrincipal.Value} no existe o se encuentra inactiva.");
            }
        }

        // 3. Normalización y estado activo
        entity.Nombres = entity.Nombres.Trim();
        entity.Apellidos = entity.Apellidos.Trim();
        entity.Correo = entity.Correo.Trim().ToLowerInvariant();
        entity.Activo = true;

        return await _docenteRepository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int cedula, Docente entity)
    {
        if (cedula <= 0) return false;

        var existing = await _docenteRepository.GetByIdAsync(cedula);
        if (existing == null) return false;

        if (string.IsNullOrWhiteSpace(entity.Nombres))
            throw new ArgumentException("El campo 'nombres' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Apellidos))
            throw new ArgumentException("El campo 'apellidos' es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.Correo))
            throw new ArgumentException("El campo 'correo' es obligatorio.");

        // Validación de FK si se actualiza la línea matriz
        if (entity.LineaInvestigacionPrincipal.HasValue)
        {
            var linea = await _lineaRepository.GetByIdAsync(entity.LineaInvestigacionPrincipal.Value);
            if (linea == null || !linea.Activo)
            {
                throw new ArgumentException($"La línea de investigación con ID {entity.LineaInvestigacionPrincipal.Value} no existe o se encuentra inactiva.");
            }
        }

        entity.Cedula = cedula;
        entity.Nombres = entity.Nombres.Trim();
        entity.Apellidos = entity.Apellidos.Trim();
        entity.Correo = entity.Correo.Trim().ToLowerInvariant();

        return await _docenteRepository.UpdateAsync(entity);
    }

    public async Task<bool> SoftDeleteAsync(int cedula)
    {
        if (cedula <= 0) return false;

        var existing = await _docenteRepository.GetByIdAsync(cedula);
        if (existing == null) return false;

        return await _docenteRepository.SoftDeleteAsync(cedula);
    }
}