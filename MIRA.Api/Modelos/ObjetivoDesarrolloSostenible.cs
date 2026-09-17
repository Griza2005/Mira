namespace MIRA.Api.Modelos;

public class ObjetivoDesarrolloSostenible
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

