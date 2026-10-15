namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente de articulación entre grupos y líneas de investigación (HU 8).
/// </summary>
public class GrupoLinea
{
    public int GrupoId { get; set; }
    public int LineaId { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? GrupoNombre { get; set; }
    public string? LineaNombre { get; set; }
}