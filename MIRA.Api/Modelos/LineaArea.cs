namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente entre líneas de investigación y áreas de conocimiento (HU 9).
/// </summary>
public class LineaArea
{
    public int LineaId { get; set; }
    public int AreaConocimientoId { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? LineaNombre { get; set; }
    public string? AreaConocimientoDisciplina { get; set; }
}