using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class AreaAplicacionRepository : IAreaAplicacionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AreaAplicacionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<AreaAplicacion>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, activo 
            FROM area_aplicacion 
            WHERE activo = TRUE 
            ORDER BY id;";
        return await connection.QueryAsync<AreaAplicacion>(sql);
    }

    public async Task<AreaAplicacion?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, activo 
            FROM area_aplicacion 
            WHERE id = @Id AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<AreaAplicacion>(sql, new { Id = id });
    }

    public async Task<AreaAplicacion> CreateAsync(AreaAplicacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO area_aplicacion (nombre, activo) 
            VALUES (@Nombre, @Activo) 
            RETURNING id, nombre, activo;";
        return await connection.QuerySingleAsync<AreaAplicacion>(sql, entity);
    }

    public async Task<bool> UpdateAsync(AreaAplicacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE area_aplicacion 
            SET nombre = @Nombre 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE area_aplicacion 
            SET activo = FALSE 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}

