using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

/// <summary>
/// Implementación con Dapper del acceso a datos para Grupo de Investigación con JOINs.
/// </summary>
public class GrupoInvestigacionRepository : IGrupoInvestigacionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GrupoInvestigacionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<GrupoInvestigacion>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        // Consulta enriquecida cruzando sede y docente líder en una sola sentencia SQL
        const string sql = @"
            SELECT 
                g.id,
                g.codigo,
                g.nombre,
                g.enlace_gruplac AS EnlaceGruplac,
                g.categoria_minciencias AS CategoriaMinciencias,
                g.universidad_id AS UniversidadId,
                g.docente_lider_cedula AS DocenteLiderCedula,
                u.nombre AS UniversidadNombre,
                CONCAT(d.nombres, ' ', d.apellidos) AS DocenteLiderNombre,
                g.activo
            FROM grupo_investigacion g
            INNER JOIN universidad u ON g.universidad_id = u.id
            INNER JOIN docente d ON g.docente_lider_cedula = d.cedula
            WHERE g.activo = TRUE
            ORDER BY g.nombre;";

        return await connection.QueryAsync<GrupoInvestigacion>(sql);
    }

    public async Task<GrupoInvestigacion?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                g.id,
                g.codigo,
                g.nombre,
                g.enlace_gruplac AS EnlaceGruplac,
                g.categoria_minciencias AS CategoriaMinciencias,
                g.universidad_id AS UniversidadId,
                g.docente_lider_cedula AS DocenteLiderCedula,
                u.nombre AS UniversidadNombre,
                CONCAT(d.nombres, ' ', d.apellidos) AS DocenteLiderNombre,
                g.activo
            FROM grupo_investigacion g
            INNER JOIN universidad u ON g.universidad_id = u.id
            INNER JOIN docente d ON g.docente_lider_cedula = d.cedula
            WHERE g.id = @Id AND g.activo = TRUE;";

        return await connection.QueryFirstOrDefaultAsync<GrupoInvestigacion>(sql, new { Id = id });
    }

    public async Task<GrupoInvestigacion> CreateAsync(GrupoInvestigacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO grupo_investigacion (
                codigo, nombre, enlace_gruplac, categoria_minciencias, 
                universidad_id, docente_lider_cedula, activo
            ) VALUES (
                @Codigo, @Nombre, @EnlaceGruplac, @CategoriaMinciencias, 
                @UniversidadId, @DocenteLiderCedula, @Activo
            )
            RETURNING 
                id, codigo, nombre, enlace_gruplac AS EnlaceGruplac, 
                categoria_minciencias AS CategoriaMinciencias, 
                universidad_id AS UniversidadId, 
                docente_lider_cedula AS DocenteLiderCedula, activo;";

        return await connection.QuerySingleAsync<GrupoInvestigacion>(sql, entity);
    }

    public async Task<bool> UpdateAsync(GrupoInvestigacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE grupo_investigacion
            SET codigo = @Codigo,
                nombre = @Nombre,
                enlace_gruplac = @EnlaceGruplac,
                categoria_minciencias = @CategoriaMinciencias,
                universidad_id = @UniversidadId,
                docente_lider_cedula = @DocenteLiderCedula
            WHERE id = @Id AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE grupo_investigacion
            SET activo = FALSE
            WHERE id = @Id AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}