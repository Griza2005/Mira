namespace MIRA.Api.Modelos;

/// <summary>
/// Representa un Semillero de Investigación adscrito a un grupo base (HU 6).
/// </summary>
public class Semillero
{
    // Identificador autoincremental en PostgreSQL (SERIAL)
    public int Id { get; set; }

    // Nombre oficial del semillero
    public string Nombre { get; set; } = string.Empty;

    // Propósito o descripción de las actividades formativas del semillero
    public string? Descripcion { get; set; }

    // Clave foránea obligatoria hacia el grupo de investigación base
    public int GrupoInvestigacionId { get; set; }

    // Campo enriquecido que Dapper puebla mediante JOIN con grupo_investigacion
    public string? GrupoInvestigacionNombre { get; set; }

    // Bandera obligatoria para borrado lógico
    public bool Activo { get; set; } = true;
}