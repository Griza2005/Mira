namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente para vincular docentes con grupos de investigación (HU 5).
/// </summary>
public class DocenteGrupo
{
    public int DocenteCedula { get; set; }
    public int GrupoId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? DocenteNombre { get; set; }
    public string? GrupoNombre { get; set; }
}