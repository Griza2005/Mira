using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class ObjetivoDesarrolloSostenibleRepository : IObjetivoDesarrolloSostenibleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ObjetivoDesarrolloSostenibleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ObjetivoDesarrolloSostenible>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, categoria, activo 
            FROM objetivo_desarrollo_sostenible 
            WHERE activo = TRUE 
            ORDER BY id;";
        return await connection.QueryAsync<ObjetivoDesarrolloSostenible>(sql);
    }

    public async Task<ObjetivoDesarrolloSostenible?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, categoria, activo 
            FROM objetivo_desarrollo_sostenible 
            WHERE id = @Id AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<ObjetivoDesarrolloSostenible>(sql, new { Id = id });
    }

    public async Task<ObjetivoDesarrolloSostenible> CreateAsync(ObjetivoDesarrolloSostenible entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO objetivo_desarrollo_sostenible (nombre, categoria, activo) 
            VALUES (@Nombre, @Categoria, @Activo) 
            RETURNING id, nombre, categoria, activo;";
        return await connection.QuerySingleAsync<ObjetivoDesarrolloSostenible>(sql, entity);
    }

    public async Task<bool> UpdateAsync(ObjetivoDesarrolloSostenible entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE objetivo_desarrollo_sostenible 
            SET nombre = @Nombre, categoria = @Categoria 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE objetivo_desarrollo_sostenible 
            SET activo = FALSE 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}

