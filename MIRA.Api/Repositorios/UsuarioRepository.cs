using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MIRA.Api.Modelos;
using Npgsql;

namespace MIRA.Api.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgreSql") 
                ?? configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSql") 
                ?? "";
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
        {
            using var connection = CreateConnection();
            var sqlUsuario = @"
                SELECT id, username, password, email, nombre_completo AS NombreCompleto, 
                       activo, fecha_creacion AS FechaCreacion, fecha_actualizacion AS FechaActualizacion
                FROM usuario
                WHERE username = @Username AND activo = TRUE;";

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(sqlUsuario, new { Username = username });

            if (usuario != null)
            {
                var sqlRoles = @"
                    SELECT r.nombre 
                    FROM rol r
                    INNER JOIN rol_usuario ru ON r.id = ru.rol_id
                    WHERE ru.usuario_id = @UsuarioId AND ru.activo = TRUE AND r.activo = TRUE;";

                var roles = await connection.QueryAsync<string>(sqlRoles, new { UsuarioId = usuario.Id });
                usuario.Roles = roles.ToList();
            }

            return usuario;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sqlUsuario = @"
                SELECT id, username, password, email, nombre_completo AS NombreCompleto, 
                       activo, fecha_creacion AS FechaCreacion, fecha_actualizacion AS FechaActualizacion
                FROM usuario
                WHERE id = @Id AND activo = TRUE;";

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(sqlUsuario, new { Id = id });

            if (usuario != null)
            {
                var sqlRoles = @"
                    SELECT r.nombre 
                    FROM rol r
                    INNER JOIN rol_usuario ru ON r.id = ru.rol_id
                    WHERE ru.usuario_id = @UsuarioId AND ru.activo = TRUE AND r.activo = TRUE;";

                var roles = await connection.QueryAsync<string>(sqlRoles, new { UsuarioId = usuario.Id });
                usuario.Roles = roles.ToList();
            }

            return usuario;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            using var connection = CreateConnection();
            var sqlUsuarios = @"
                SELECT id, username, password, email, nombre_completo AS NombreCompleto, 
                       activo, fecha_creacion AS FechaCreacion, fecha_actualizacion AS FechaActualizacion
                FROM usuario
                WHERE activo = TRUE
                ORDER BY id ASC;";

            var usuarios = (await connection.QueryAsync<Usuario>(sqlUsuarios)).ToList();

            foreach (var u in usuarios)
            {
                var sqlRoles = @"
                    SELECT r.nombre 
                    FROM rol r
                    INNER JOIN rol_usuario ru ON r.id = ru.rol_id
                    WHERE ru.usuario_id = @UsuarioId AND ru.activo = TRUE AND r.activo = TRUE;";

                var roles = await connection.QueryAsync<string>(sqlRoles, new { UsuarioId = u.Id });
                u.Roles = roles.ToList();
            }

            return usuarios;
        }

        public async Task<bool> ExisteUsernameOEmailAsync(string username, string email)
        {
            using var connection = CreateConnection();
            var sql = "SELECT COUNT(1) FROM usuario WHERE (username = @Username OR email = @Email) AND activo = TRUE;";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username, Email = email });
            return count > 0;
        }

        public async Task<int> CrearAsync(Usuario usuario, List<int> rolesIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sqlUsuario = @"
                    INSERT INTO usuario (username, password, email, nombre_completo, activo, fecha_creacion, fecha_actualizacion)
                    VALUES (@Username, @Password, @Email, @NombreCompleto, TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
                    RETURNING id;";

                var nuevoId = await connection.ExecuteScalarAsync<int>(sqlUsuario, new
                {
                    usuario.Username,
                    usuario.Password,
                    usuario.Email,
                    usuario.NombreCompleto
                }, transaction);

                if (rolesIds != null && rolesIds.Any())
                {
                    var sqlRolUsuario = @"
                        INSERT INTO rol_usuario (usuario_id, rol_id, activo)
                        VALUES (@UsuarioId, @RolId, TRUE);";

                    foreach (var rolId in rolesIds)
                    {
                        await connection.ExecuteAsync(sqlRolUsuario, new { UsuarioId = nuevoId, RolId = rolId }, transaction);
                    }
                }

                transaction.Commit();
                return nuevoId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> EliminarLogicoAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sqlUsuario = "UPDATE usuario SET activo = FALSE, fecha_actualizacion = CURRENT_TIMESTAMP WHERE id = @Id;";
                var filas = await connection.ExecuteAsync(sqlUsuario, new { Id = id }, transaction);

                var sqlRoles = "UPDATE rol_usuario SET activo = FALSE WHERE usuario_id = @Id;";
                await connection.ExecuteAsync(sqlRoles, new { Id = id }, transaction);

                transaction.Commit();
                return filas > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}