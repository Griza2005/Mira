using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MIRA.Api.Modelos;
using MIRA.Api.Peticiones;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        private static UsuarioResponse MapearAResponse(Usuario u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            NombreCompleto = u.NombreCompleto,
            Activo = u.Activo,
            Roles = u.Roles
        };

        public async Task<IEnumerable<UsuarioResponse>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObtenerTodosAsync();
            return usuarios.Select(MapearAResponse);
        }

        public async Task<UsuarioResponse?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            return usuario == null ? null : MapearAResponse(usuario);
        }

        public async Task<UsuarioResponse> CrearAsync(UsuarioCrear peticion)
        {
            if (await _usuarioRepository.ExisteUsernameOEmailAsync(peticion.Username, peticion.Email))
            {
                throw new ArgumentException("El nombre de usuario o correo electrónico ya está registrado.");
            }

            var nuevoUsuario = new Usuario
            {
                Username = peticion.Username,
                // Regla de Oro: Encriptar la contraseña con BCrypt antes de guardar
                Password = BCrypt.Net.BCrypt.HashPassword(peticion.Password),
                Email = peticion.Email,
                NombreCompleto = peticion.NombreCompleto
            };

            var nuevoId = await _usuarioRepository.CrearAsync(nuevoUsuario, peticion.RolesIds);
            var usuarioCreado = await _usuarioRepository.ObtenerPorIdAsync(nuevoId);

            return MapearAResponse(usuarioCreado!);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var existente = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (existente == null) return false;

            return await _usuarioRepository.EliminarLogicoAsync(id);
        }
    }
}