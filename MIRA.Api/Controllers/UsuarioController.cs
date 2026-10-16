using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Peticiones;
using MIRA.Api.Servicios;

namespace MIRA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     [Authorize(Roles = "Administrador")] // Desactivado temporalmente para registrar el superadmin
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(new
            {
                tabla = "usuario",
                total = usuarios.Count(),
                datos = usuarios
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { estado = 404, mensaje = "Usuario no encontrado." });
            }
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioCrear peticion)
        {
            try
            {
                var usuarioCreado = await _usuarioService.CrearAsync(peticion);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = usuarioCreado.Id }, usuarioCreado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { estado = 400, mensaje = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound(new { estado = 404, mensaje = "El usuario no existe o ya está inactivo." });
            }
            return Ok(new { estado = 200, mensaje = "Usuario eliminado exitosamente (borrado lógico)." });
        }
    }
}