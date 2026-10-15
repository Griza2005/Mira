namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente entre líneas de investigación y áreas de aplicación industrial (HU 9).
/// </summary>
public class LineaAreaAplicacion
{
    public int LineaId { get; set; }
    public int AreaAplicacionId { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? LineaNombre { get; set; }
    public string? AreaAplicacionNombre { get; set; }
}