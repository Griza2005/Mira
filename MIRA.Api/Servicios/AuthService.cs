using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MIRA.Api.Peticiones;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> AutenticarAsync(LoginRequest peticion)
        {
            // 1. Buscar usuario en base de datos
            var usuario = await _usuarioRepository.ObtenerPorUsernameAsync(peticion.Username);
            if (usuario == null) return null;

            // 2. Verificar contraseña contra el Hash de BCrypt
            bool passwordValida = BCrypt.Net.BCrypt.Verify(peticion.Password, usuario.Password);
            if (!passwordValida) return null;

            // 3. Crear Claims para el Token (incluyendo los roles del usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            foreach (var rol in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            // 4. Firmar el Token JWT
            var keyString = _configuration["JwtSettings:Key"] ?? "ClaveSecretaSuperSeguraParaElModuloMIRA2026!*AutenticacionJWT_USBMed";
            var issuer = _configuration["JwtSettings:Issuer"] ?? "MIRA_Api";
            var audience = _configuration["JwtSettings:Audience"] ?? "MIRA_Client";
            var durationMinutes = double.TryParse(_configuration["JwtSettings:DurationInMinutes"], out var d) ? d : 480;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                Usuario = new UsuarioAuthDto
                {
                    Username = usuario.Username,
                    NombreCompleto = usuario.NombreCompleto,
                    Roles = usuario.Roles
                }
            };
        }
    }
}