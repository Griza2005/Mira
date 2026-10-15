namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente de articulación entre semilleros y líneas de investigación (HU 8).
/// </summary>
public class SemilleroLinea
{
    public int SemilleroId { get; set; }
    public int LineaId { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? SemilleroNombre { get; set; }
    public string? LineaNombre { get; set; }
}