using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

/// <summary>
/// Implementación con Dapper del acceso a datos para Docente con LEFT JOIN.
/// </summary>
public class DocenteRepository : IDocenteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DocenteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Docente>> GetAllActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        // Consulta enriquecida con LEFT JOIN para traer el nombre de la línea en una sola consulta
        const string sql = @"
            SELECT 
                d.cedula,
                d.nombres,
                d.apellidos,
                d.correo,
                d.enlace_cvlac AS EnlaceCvlac,
                d.escalafon,
                d.categoria_minciencias AS CategoriaMinciencias,
                d.linea_investigacion_principal AS LineaInvestigacionPrincipal,
                l.nombre AS LineaInvestigacionNombre,
                d.activo
            FROM docente d
            LEFT JOIN linea_investigacion l ON d.linea_investigacion_principal = l.id
            WHERE d.activo = TRUE
            ORDER BY d.apellidos, d.nombres;";

        return await connection.QueryAsync<Docente>(sql);
    }

    public async Task<Docente?> GetByIdAsync(int cedula)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                d.cedula,
                d.nombres,
                d.apellidos,
                d.correo,
                d.enlace_cvlac AS EnlaceCvlac,
                d.escalafon,
                d.categoria_minciencias AS CategoriaMinciencias,
                d.linea_investigacion_principal AS LineaInvestigacionPrincipal,
                l.nombre AS LineaInvestigacionNombre,
                d.activo
            FROM docente d
            LEFT JOIN linea_investigacion l ON d.linea_investigacion_principal = l.id
            WHERE d.cedula = @Cedula AND d.activo = TRUE;";

        return await connection.QueryFirstOrDefaultAsync<Docente>(sql, new { Cedula = cedula });
    }

    public async Task<Docente> CreateAsync(Docente entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO docente (
                cedula, nombres, apellidos, correo, enlace_cvlac, 
                escalafon, categoria_minciencias, linea_investigacion_principal, activo
            ) VALUES (
                @Cedula, @Nombres, @Apellidos, @Correo, @EnlaceCvlac, 
                @Escalafon, @CategoriaMinciencias, @LineaInvestigacionPrincipal, @Activo
            )
            RETURNING 
                cedula, nombres, apellidos, correo, enlace_cvlac AS EnlaceCvlac, 
                escalafon, categoria_minciencias AS CategoriaMinciencias, 
                linea_investigacion_principal AS LineaInvestigacionPrincipal, activo;";

        return await connection.QuerySingleAsync<Docente>(sql, entity);
    }

    public async Task<bool> UpdateAsync(Docente entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE docente
            SET nombres = @Nombres,
                apellidos = @Apellidos,
                correo = @Correo,
                enlace_cvlac = @EnlaceCvlac,
                escalafon = @Escalafon,
                categoria_minciencias = @CategoriaMinciencias,
                linea_investigacion_principal = @LineaInvestigacionPrincipal
            WHERE cedula = @Cedula AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int cedula)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE docente
            SET activo = FALSE
            WHERE cedula = @Cedula AND activo = TRUE;";

        var affected = await connection.ExecuteAsync(sql, new { Cedula = cedula });
        return affected > 0;
    }
}