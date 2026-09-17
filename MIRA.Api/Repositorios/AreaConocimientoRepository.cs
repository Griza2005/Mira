using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class AreaConocimientoRepository : IAreaConocimientoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AreaConocimientoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<AreaConocimiento>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, gran_area AS GranArea, area, disciplina, activo 
            FROM area_conocimiento 
            WHERE activo = TRUE 
            ORDER BY id;";
        return await connection.QueryAsync<AreaConocimiento>(sql);
    }

    public async Task<AreaConocimiento?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, gran_area AS GranArea, area, disciplina, activo 
            FROM area_conocimiento 
            WHERE id = @Id AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<AreaConocimiento>(sql, new { Id = id });
    }

    public async Task<AreaConocimiento> CreateAsync(AreaConocimiento entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO area_conocimiento (gran_area, area, disciplina, activo) 
            VALUES (@GranArea, @Area, @Disciplina, @Activo) 
            RETURNING id, gran_area AS GranArea, area, disciplina, activo;";
        return await connection.QuerySingleAsync<AreaConocimiento>(sql, entity);
    }

    public async Task<bool> UpdateAsync(AreaConocimiento entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE area_conocimiento 
            SET gran_area = @GranArea, area = @Area, disciplina = @Disciplina 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE area_conocimiento 
            SET activo = FALSE 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}

