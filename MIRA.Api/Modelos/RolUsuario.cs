namespace MIRA.Api.Modelos
{
    public class RolUsuario
    {
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; } = true;

        public string? UsuarioUsername { get; set; }
        public string? RolNombre { get; set; }
    }
}