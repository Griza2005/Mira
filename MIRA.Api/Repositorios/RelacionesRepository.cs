using Dapper;
using MIRA.Api.Configuracion;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios;

/// <summary>
/// Implementación con Dapper para tablas puente con JOINs y reactivación ON CONFLICT.
/// </summary>
public class RelacionesRepository : IRelacionesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RelacionesRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // --- Docente Grupo (HU 5) ---
    public async Task<IEnumerable<DocenteGrupo>> GetAllDocenteGrupoActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT dg.docente_cedula AS DocenteCedula, dg.grupo_id AS GrupoId, 
                   dg.rol, dg.fecha_inicio AS FechaInicio, dg.fecha_fin AS FechaFin, dg.activo,
                   CONCAT(d.nombres, ' ', d.apellidos) AS DocenteNombre, g.nombre AS GrupoNombre
            FROM docente_grupo dg
            INNER JOIN docente d ON dg.docente_cedula = d.cedula
            INNER JOIN grupo_investigacion g ON dg.grupo_id = g.id
            WHERE dg.activo = TRUE;";
        return await connection.QueryAsync<DocenteGrupo>(sql);
    }

    public async Task<bool> CreateDocenteGrupoAsync(DocenteGrupo entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO docente_grupo (docente_cedula, grupo_id, rol, fecha_inicio, fecha_fin, activo)
            VALUES (@DocenteCedula, @GrupoId, @Rol, @FechaInicio, @FechaFin, TRUE)
            ON CONFLICT (docente_cedula, grupo_id, fecha_inicio) 
            DO UPDATE SET rol = @Rol, fecha_fin = @FechaFin, activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteDocenteGrupoAsync(int docenteCedula, int grupoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE docente_grupo 
            SET activo = FALSE 
            WHERE docente_cedula = @DocenteCedula AND grupo_id = @GrupoId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { DocenteCedula = docenteCedula, GrupoId = grupoId });
        return affected > 0;
    }

    // --- Docente Semillero (HU 6) ---
    public async Task<IEnumerable<DocenteSemillero>> GetAllDocenteSemilleroActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ds.docente_cedula AS DocenteCedula, ds.semillero_id AS SemilleroId, 
                   ds.rol, ds.periodo_academico AS PeriodoAcademico, ds.activo,
                   CONCAT(d.nombres, ' ', d.apellidos) AS DocenteNombre, s.nombre AS SemilleroNombre
            FROM docente_semillero ds
            INNER JOIN docente d ON ds.docente_cedula = d.cedula
            INNER JOIN semillero s ON ds.semillero_id = s.id
            WHERE ds.activo = TRUE;";
        return await connection.QueryAsync<DocenteSemillero>(sql);
    }

    public async Task<bool> CreateDocenteSemilleroAsync(DocenteSemillero entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO docente_semillero (docente_cedula, semillero_id, rol, periodo_academico, activo)
            VALUES (@DocenteCedula, @SemilleroId, @Rol, @PeriodoAcademico, TRUE)
            ON CONFLICT (docente_cedula, semillero_id, periodo_academico) 
            DO UPDATE SET rol = @Rol, activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteDocenteSemilleroAsync(int docenteCedula, int semilleroId, string periodoAcademico)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE docente_semillero 
            SET activo = FALSE 
            WHERE docente_cedula = @DocenteCedula AND semillero_id = @SemilleroId 
              AND periodo_academico = @PeriodoAcademico AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { DocenteCedula = docenteCedula, SemilleroId = semilleroId, PeriodoAcademico = periodoAcademico });
        return affected > 0;
    }

    // --- Grupo Línea (HU 8) ---
    public async Task<IEnumerable<GrupoLinea>> GetAllGrupoLineaActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT gl.grupo_id AS GrupoId, gl.linea_id AS LineaId, gl.activo,
                   g.nombre AS GrupoNombre, l.nombre AS LineaNombre
            FROM grupo_linea gl
            INNER JOIN grupo_investigacion g ON gl.grupo_id = g.id
            INNER JOIN linea_investigacion l ON gl.linea_id = l.id
            WHERE gl.activo = TRUE;";
        return await connection.QueryAsync<GrupoLinea>(sql);
    }

    public async Task<bool> CreateGrupoLineaAsync(GrupoLinea entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO grupo_linea (grupo_id, linea_id, activo)
            VALUES (@GrupoId, @LineaId, TRUE)
            ON CONFLICT (grupo_id, linea_id) DO UPDATE SET activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteGrupoLineaAsync(int grupoId, int lineaId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE grupo_linea 
            SET activo = FALSE 
            WHERE grupo_id = @GrupoId AND linea_id = @LineaId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { GrupoId = grupoId, LineaId = lineaId });
        return affected > 0;
    }

    // --- Semillero Línea (HU 8) ---
    public async Task<IEnumerable<SemilleroLinea>> GetAllSemilleroLineaActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT sl.semillero_id AS SemilleroId, sl.linea_id AS LineaId, sl.activo,
                   s.nombre AS SemilleroNombre, l.nombre AS LineaNombre
            FROM semillero_linea sl
            INNER JOIN semillero s ON sl.semillero_id = s.id
            INNER JOIN linea_investigacion l ON sl.linea_id = l.id
            WHERE sl.activo = TRUE;";
        return await connection.QueryAsync<SemilleroLinea>(sql);
    }

    public async Task<bool> CreateSemilleroLineaAsync(SemilleroLinea entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO semillero_linea (semillero_id, linea_id, activo)
            VALUES (@SemilleroId, @LineaId, TRUE)
            ON CONFLICT (semillero_id, linea_id) DO UPDATE SET activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteSemilleroLineaAsync(int semilleroId, int lineaId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE semillero_linea 
            SET activo = FALSE 
            WHERE semillero_id = @SemilleroId AND linea_id = @LineaId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { SemilleroId = semilleroId, LineaId = lineaId });
        return affected > 0;
    }

    // --- Línea Área Conocimiento (HU 9) ---
    public async Task<IEnumerable<LineaArea>> GetAllLineaAreaActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT la.linea_id AS LineaId, la.area_conocimiento_id AS AreaConocimientoId, la.activo,
                   l.nombre AS LineaNombre, a.disciplina AS AreaConocimientoDisciplina
            FROM linea_area la
            INNER JOIN linea_investigacion l ON la.linea_id = l.id
            INNER JOIN area_conocimiento a ON la.area_conocimiento_id = a.id
            WHERE la.activo = TRUE;";
        return await connection.QueryAsync<LineaArea>(sql);
    }

    public async Task<bool> CreateLineaAreaAsync(LineaArea entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO linea_area (linea_id, area_conocimiento_id, activo)
            VALUES (@LineaId, @AreaConocimientoId, TRUE)
            ON CONFLICT (linea_id, area_conocimiento_id) DO UPDATE SET activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteLineaAreaAsync(int lineaId, int areaConocimientoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE linea_area 
            SET activo = FALSE 
            WHERE linea_id = @LineaId AND area_conocimiento_id = @AreaConocimientoId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { LineaId = lineaId, AreaConocimientoId = areaConocimientoId });
        return affected > 0;
    }

    // --- Línea ODS (HU 9) ---
    public async Task<IEnumerable<LineaOds>> GetAllLineaOdsActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT lo.linea_id AS LineaId, lo.ods_id AS OdsId, lo.activo,
                   l.nombre AS LineaNombre, o.nombre AS OdsNombre
            FROM linea_ods lo
            INNER JOIN linea_investigacion l ON lo.linea_id = l.id
            INNER JOIN objetivo_desarrollo_sostenible o ON lo.ods_id = o.id
            WHERE lo.activo = TRUE;";
        return await connection.QueryAsync<LineaOds>(sql);
    }

    public async Task<bool> CreateLineaOdsAsync(LineaOds entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO linea_ods (linea_id, ods_id, activo)
            VALUES (@LineaId, @OdsId, TRUE)
            ON CONFLICT (linea_id, ods_id) DO UPDATE SET activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteLineaOdsAsync(int lineaId, int odsId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE linea_ods 
            SET activo = FALSE 
            WHERE linea_id = @LineaId AND ods_id = @OdsId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { LineaId = lineaId, OdsId = odsId });
        return affected > 0;
    }

    // --- Línea Área Aplicación (HU 9) ---
    public async Task<IEnumerable<LineaAreaAplicacion>> GetAllLineaAreaAplicacionActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT laa.linea_id AS LineaId, laa.area_aplicacion_id AS AreaAplicacionId, laa.activo,
                   l.nombre AS LineaNombre, ap.nombre AS AreaAplicacionNombre
            FROM linea_area_aplicacion laa
            INNER JOIN linea_investigacion l ON laa.linea_id = l.id
            INNER JOIN area_aplicacion ap ON laa.area_aplicacion_id = ap.id
            WHERE laa.activo = TRUE;";
        return await connection.QueryAsync<LineaAreaAplicacion>(sql);
    }

    public async Task<bool> CreateLineaAreaAplicacionAsync(LineaAreaAplicacion entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO linea_area_aplicacion (linea_id, area_aplicacion_id, activo)
            VALUES (@LineaId, @AreaAplicacionId, TRUE)
            ON CONFLICT (linea_id, area_aplicacion_id) DO UPDATE SET activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<bool> SoftDeleteLineaAreaAplicacionAsync(int lineaId, int areaAplicacionId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE linea_area_aplicacion 
            SET activo = FALSE 
            WHERE linea_id = @LineaId AND area_aplicacion_id = @AreaAplicacionId AND activo = TRUE;";
        var affected = await connection.ExecuteAsync(sql, new { LineaId = lineaId, AreaAplicacionId = areaAplicacionId });
        return affected > 0;
    }
}