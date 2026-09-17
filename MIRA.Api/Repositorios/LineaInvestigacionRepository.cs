using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

public class LineaInvestigacionRepository : ILineaInvestigacionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public LineaInvestigacionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<LineaInvestigacion>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, descripcion, activo 
            FROM linea_investigacion 
            WHERE activo = TRUE 
            ORDER BY id;";
        return await connection.QueryAsync<LineaInvestigacion>(sql);
    }

    public async Task<LineaInvestigacion?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, nombre, descripcion, activo 
            FROM linea_investigacion 
            WHERE id = @Id AND activo = TRUE;";
        return await connection.QueryFirstOrDefaultAsync<LineaInvestigacion>(sql, new { Id = id });
    }

    public async Task<LineaInvestigacion> CreateAsync(LineaInvestigacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO linea_investigacion (nombre, descripcion, activo) 
            VALUES (@Nombre, @Descripcion, @Activo) 
            RETURNING id, nombre, descripcion, activo;";
        return await connection.QuerySingleAsync<LineaInvestigacion>(sql, entity);
    }

    public async Task<bool> UpdateAsync(LineaInvestigacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE linea_investigacion 
            SET nombre = @Nombre, descripcion = @Descripcion 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE linea_investigacion 
            SET activo = FALSE 
            WHERE id = @Id AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}

