using System;
using System.Collections.Generic;

namespace MIRA.Api.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Hash BCrypt
        public string Email { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Roles cargados para Dapper
        public List<string> Roles { get; set; } = new();
    }
}