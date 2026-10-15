using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Peticiones;
using MIRA.Api.Servicios;

namespace MIRA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest peticion)
        {
            var resultado = await _authService.AutenticarAsync(peticion);

            if (resultado == null)
            {
                return Unauthorized(new
                {
                    estado = 401,
                    mensaje = "Credenciales inválidas",
                    detalle = "El nombre de usuario o la contraseña no coinciden."
                });
            }

            return Ok(new
            {
                estado = 200,
                mensaje = "Autenticación exitosa",
                token = resultado.Token,
                usuario = resultado.Usuario
            });
        }
    }
}