namespace MIRA.Api.Modelos;

/// <summary>
/// Tabla puente para tutorías y docentes vinculados a semilleros (HU 6).
/// </summary>
public class DocenteSemillero
{
    public int DocenteCedula { get; set; }
    public int SemilleroId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public string PeriodoAcademico { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    // Campos enriquecidos para lecturas con Dapper
    public string? DocenteNombre { get; set; }
    public string? SemilleroNombre { get; set; }
}