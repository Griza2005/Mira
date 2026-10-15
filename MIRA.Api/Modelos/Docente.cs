namespace MIRA.Api.Modelos;

/// <summary>
/// Representa el perfil académico y de investigación de un docente (HU 3).
/// </summary>
public class Docente
{
    // Cédula de ciudadanía institucional (Llave Primaria no autoincremental)
    public int Cedula { get; set; }

    // Información básica personal y de contacto
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    // Enlace público al perfil de CvLAC de Minciencias
    public string? EnlaceCvlac { get; set; }

    // Escalafón docente (ej. Auxiliar, Asistente, Asociado, Titular)
    public string? Escalafon { get; set; }

    // Clasificación otorgada por Minciencias (Senior, Asociado, Junior, Sin Categoría)
    public string? CategoriaMinciencias { get; set; }

    // Clave foránea opcional hacia la línea de investigación matriz
    public int? LineaInvestigacionPrincipal { get; set; }

    // Campo enriquecido que Dapper puebla mediante LEFT JOIN con linea_investigacion
    public string? LineaInvestigacionNombre { get; set; }

    // Bandera obligatoria para la regla constitucional de borrado lógico
    public bool Activo { get; set; } = true;
}