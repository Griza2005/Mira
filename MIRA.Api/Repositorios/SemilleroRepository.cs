using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

/// <summary>
/// Implementación con Dapper del acceso a datos para Semillero con JOINs.
/// </summary>
public class SemilleroRepository : ISemilleroRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SemilleroRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Semillero>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        // Consulta enriquecida cruzando con grupo_investigacion
        const string sql = @"
            SELECT 
                s.id,
                s.nombre,
                s.descripcion,
                s.grupo_investigacion_id AS GrupoInvestigacionId,
                g.nombre AS GrupoInvestigacionNombre,
                s.activo
            FROM semillero s
            INNER JOIN grupo_investigacion g ON s.grupo_investigacion_id = g.id
            WHERE s.activo = TRUE
            ORDER BY s.nombre;";

        return await connection.QueryAsync<Semillero>(sql);
    }

    public async Task<Semillero?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                s.id,
                s.nombre,
                s.descripcion,
                s.grupo_investigacion_id AS GrupoInvestigacionId,
                g.nombre AS GrupoInvestigacionNombre,
                s.activo
            FROM semillero s
            INNER JOIN grupo_investigacion g ON s.grupo_investigacion_id = g.id
            WHERE s.id = @Id AND s.activo = TRUE;";

        return await connection.QueryFirstOrDefaultAsync<Semillero>(sql, new { Id = id });
    }

    public async Task<Semillero> CreateAsync(Semillero entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO semillero (
                nombre, descripcion, grupo_investigacion_id, activo
            ) VALUES (
                @Nombre, @Descripcion, @GrupoInvestigacionId, @Activo
            )
            RETURNING 
                id, nombre, descripcion, grupo_investigacion_id AS GrupoInvestigacionId, activo;";

        return await connection.QuerySingleAsync<Semillero>(sql, entity);
    }

    public async Task<bool> UpdateAsync(Semillero entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE semillero
            SET nombre = @Nombre,
                descripcion = @Descripcion,
                grupo_investigacion_id = @GrupoInvestigacionId
            WHERE id = @Id AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE semillero
            SET activo = FALSE
            WHERE id = @Id AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}