using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIRA.Api.Peticiones
{
    public class UsuarioCrear
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
        public string Email { get; set; } = string.Empty;

        public string? NombreCompleto { get; set; }

        [Required(ErrorMessage = "Debe asignar al menos un rol.")]
        public List<int> RolesIds { get; set; } = new();
    }
}