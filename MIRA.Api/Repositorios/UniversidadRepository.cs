using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class UniversidadRepository : IUniversidadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UniversidadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Universidad>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, tipo, ciudad, activo 
            FROM universidad 
            WHERE activo = TRUE 
            ORDER BY id;";
        return await connection.QueryAsync<Universidad>(sql);
    }

    public async Task<Universidad?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, tipo, ciudad, activo 
            FROM universidad 
            WHERE id = @Id AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<Universidad>(sql, new { Id = id });
    }

    public async Task<Universidad> CreateAsync(Universidad entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO universidad (nombre, tipo, ciudad, activo) 
            VALUES (@Nombre, @Tipo, @Ciudad, @Activo) 
            RETURNING id, nombre, tipo, ciudad, activo;";
        return await connection.QuerySingleAsync<Universidad>(sql, entity);
    }

    public async Task<bool> UpdateAsync(Universidad entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE universidad 
            SET nombre = @Nombre, tipo = @Tipo, ciudad = @Ciudad 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE universidad 
            SET activo = FALSE 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}

