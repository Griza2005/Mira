namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente entre líneas de investigación y ODS (HU 9).
/// </summary>
public class LineaOds
{
    public int LineaId { get; set; }
    public int OdsId { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? LineaNombre { get; set; }
    public string? OdsNombre { get; set; }
}