namespace MIRA.Api.Modelos;

/// <summary>
/// Representa un Grupo de Investigación institucional adscrito a una sede (HU 5).
/// </summary>
public class GrupoInvestigacion
{
    // Identificador autoincremental en PostgreSQL (SERIAL)
    public int Id { get; set; }

    // Código institucional único del grupo (ej: COL0012345)
    public string Codigo { get; set; } = string.Empty;

    // Nombre oficial del grupo de investigación
    public string Nombre { get; set; } = string.Empty;

    // Enlace público al perfil GrupLAC de Minciencias
    public string? EnlaceGruplac { get; set; }

    // Categoría otorgada por Minciencias (A1, A, B, C, Reconocido)
    public string? CategoriaMinciencias { get; set; }

    // Clave foránea obligatoria hacia la sede universitaria (universidad)
    public int UniversidadId { get; set; }

    // Clave foránea obligatoria hacia el docente que ejerce como líder del grupo
    public int DocenteLiderCedula { get; set; }

    // Campos enriquecidos que Dapper puebla mediante JOINs con universidad y docente
    public string? UniversidadNombre { get; set; }
    public string? DocenteLiderNombre { get; set; }

    // Bandera obligatoria para borrado lógico
    public bool Activo { get; set; } = true;
}