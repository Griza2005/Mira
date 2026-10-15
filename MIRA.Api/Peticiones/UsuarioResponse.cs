using System.Collections.Generic;

namespace MIRA.Api.Peticiones
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}