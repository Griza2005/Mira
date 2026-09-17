using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class TerminoClaveRepository : ITerminoClaveRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TerminoClaveRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TerminoClave>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT termino, termino_ingles AS TerminoIngles, activo 
            FROM termino_clave 
            WHERE activo = TRUE 
            ORDER BY termino;";
        return await connection.QueryAsync<TerminoClave>(sql);
    }

    public async Task<TerminoClave?> GetByTerminoAsync(string termino)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT termino, termino_ingles AS TerminoIngles, activo 
            FROM termino_clave 
            WHERE termino = @Termino AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<TerminoClave>(sql, new { Termino = termino });
    }

    public async Task<TerminoClave> CreateAsync(TerminoClave entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO termino_clave (termino, termino_ingles, activo) 
            VALUES (@Termino, @TerminoIngles, @Activo) 
            RETURNING termino, termino_ingles AS TerminoIngles, activo;";
        return await connection.QuerySingleAsync<TerminoClave>(sql, entity);
    }

    public async Task<bool> UpdateAsync(string termino, TerminoClave entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE termino_clave 
            SET termino_ingles = @TerminoIngles 
            WHERE termino = @Termino AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { entity.TerminoIngles, Termino = termino });
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(string termino)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE termino_clave 
            SET activo = FALSE 
            WHERE termino = @Termino AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Termino = termino });
        return affected > 0;
    }
}

