namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de lógica de negocio para la gestión de docentes e investigadores (HU 3).
/// </summary>
public interface IDocenteService
{
    Task<IEnumerable<Docente>> GetAllActivosAsync();
    Task<Docente?> GetByIdAsync(int cedula);
    Task<Docente> CreateAsync(Docente entity);
    Task<bool> UpdateAsync(int cedula, Docente entity);
    Task<bool> SoftDeleteAsync(int cedula);
}