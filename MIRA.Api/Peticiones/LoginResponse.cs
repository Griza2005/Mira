using System.Collections.Generic;

namespace MIRA.Api.Peticiones
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioAuthDto Usuario { get; set; } = null!;
    }

    public class UsuarioAuthDto
    {
        public string Username { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}