namespace MIRA.Api.Servicios;

using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

/// <summary>
/// Servicio que valida la existencia y estado activo de ambas entidades
/// antes de autorizar la inserción en cualquiera de las 7 tablas puente.
/// </summary>
public class RelacionesService : IRelacionesService
{
    private readonly IRelacionesRepository _relacionesRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IGrupoInvestigacionRepository _grupoRepository;
    private readonly ISemilleroRepository _semilleroRepository;
    private readonly ILineaInvestigacionRepository _lineaRepository;
    private readonly IAreaConocimientoRepository _areaConocimientoRepository;
    private readonly IObjetivoDesarrolloSostenibleRepository _odsRepository;
    private readonly IAreaAplicacionRepository _areaAplicacionRepository;

    public RelacionesService(
        IRelacionesRepository relacionesRepository,
        IDocenteRepository docenteRepository,
        IGrupoInvestigacionRepository grupoRepository,
        ISemilleroRepository semilleroRepository,
        ILineaInvestigacionRepository lineaRepository,
        IAreaConocimientoRepository areaConocimientoRepository,
        IObjetivoDesarrolloSostenibleRepository odsRepository,
        IAreaAplicacionRepository areaAplicacionRepository)
    {
        _relacionesRepository = relacionesRepository;
        _docenteRepository = docenteRepository;
        _grupoRepository = grupoRepository;
        _semilleroRepository = semilleroRepository;
        _lineaRepository = lineaRepository;
        _areaConocimientoRepository = areaConocimientoRepository;
        _odsRepository = odsRepository;
        _areaAplicacionRepository = areaAplicacionRepository;
    }

    // 1. Docente - Grupo (HU 5)
    public async Task<IEnumerable<DocenteGrupo>> GetAllDocenteGrupoActivosAsync() =>
        await _relacionesRepository.GetAllDocenteGrupoActivosAsync();

    public async Task<bool> CreateDocenteGrupoAsync(DocenteGrupo entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Rol))
            throw new ArgumentException("El campo 'rol' es obligatorio.");

        var docente = await _docenteRepository.GetByIdAsync(entity.DocenteCedula);
        if (docente == null || !docente.Activo)
            throw new ArgumentException($"El docente con cédula {entity.DocenteCedula} no existe o se encuentra inactivo.");

        var grupo = await _grupoRepository.GetByIdAsync(entity.GrupoId);
        if (grupo == null || !grupo.Activo)
            throw new ArgumentException($"El grupo de investigación con ID {entity.GrupoId} no existe o se encuentra inactivo.");

        entity.Rol = entity.Rol.Trim();
        return await _relacionesRepository.CreateDocenteGrupoAsync(entity);
    }

    public async Task<bool> SoftDeleteDocenteGrupoAsync(int docenteCedula, int grupoId) =>
        await _relacionesRepository.SoftDeleteDocenteGrupoAsync(docenteCedula, grupoId);

    // 2. Docente - Semillero (HU 6)
    public async Task<IEnumerable<DocenteSemillero>> GetAllDocenteSemilleroActivosAsync() =>
        await _relacionesRepository.GetAllDocenteSemilleroActivosAsync();

    public async Task<bool> CreateDocenteSemilleroAsync(DocenteSemillero entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Rol))
            throw new ArgumentException("El campo 'rol' es obligatorio.");
        if (string.IsNullOrWhiteSpace(entity.PeriodoAcademico))
            throw new ArgumentException("El campo 'periodoAcademico' es obligatorio.");

        var docente = await _docenteRepository.GetByIdAsync(entity.DocenteCedula);
        if (docente == null || !docente.Activo)
            throw new ArgumentException($"El docente con cédula {entity.DocenteCedula} no existe o se encuentra inactivo.");

        var semillero = await _semilleroRepository.GetByIdAsync(entity.SemilleroId);
        if (semillero == null || !semillero.Activo)
            throw new ArgumentException($"El semillero con ID {entity.SemilleroId} no existe o se encuentra inactivo.");

        entity.Rol = entity.Rol.Trim();
        entity.PeriodoAcademico = entity.PeriodoAcademico.Trim();
        return await _relacionesRepository.CreateDocenteSemilleroAsync(entity);
    }

    public async Task<bool> SoftDeleteDocenteSemilleroAsync(int docenteCedula, int semilleroId, string periodoAcademico) =>
        await _relacionesRepository.SoftDeleteDocenteSemilleroAsync(docenteCedula, semilleroId, periodoAcademico);

    // 3. Grupo - Línea (HU 8)
    public async Task<IEnumerable<GrupoLinea>> GetAllGrupoLineaActivosAsync() =>
        await _relacionesRepository.GetAllGrupoLineaActivosAsync();

    public async Task<bool> CreateGrupoLineaAsync(GrupoLinea entity)
    {
        var grupo = await _grupoRepository.GetByIdAsync(entity.GrupoId);
        if (grupo == null || !grupo.Activo)
            throw new ArgumentException($"El grupo de investigación con ID {entity.GrupoId} no existe o se encuentra inactivo.");

        var linea = await _lineaRepository.GetByIdAsync(entity.LineaId);
        if (linea == null || !linea.Activo)
            throw new ArgumentException($"La línea de investigación con ID {entity.LineaId} no existe o se encuentra inactiva.");

        return await _relacionesRepository.CreateGrupoLineaAsync(entity);
    }

    public async Task<bool> SoftDeleteGrupoLineaAsync(int grupoId, int lineaId) =>
        await _relacionesRepository.SoftDeleteGrupoLineaAsync(grupoId, lineaId);

    // 4. Semillero - Línea (HU 8)
    public async Task<IEnumerable<SemilleroLinea>> GetAllSemilleroLineaActivosAsync() =>
        await _relacionesRepository.GetAllSemilleroLineaActivosAsync();

    public async Task<bool> CreateSemilleroLineaAsync(SemilleroLinea entity)
    {
        var semillero = await _semilleroRepository.GetByIdAsync(entity.SemilleroId);
        if (semillero == null || !semillero.Activo)
            throw new ArgumentException($"El semillero con ID {entity.SemilleroId} no existe o se encuentra inactivo.");

        var linea = await _lineaRepository.GetByIdAsync(entity.LineaId);
        if (linea == null || !linea.Activo)
            throw new ArgumentException($"La línea de investigación con ID {entity.LineaId} no existe o se encuentra inactiva.");

        return await _relacionesRepository.CreateSemilleroLineaAsync(entity);
    }

    public async Task<bool> SoftDeleteSemilleroLineaAsync(int semilleroId, int lineaId) =>
        await _relacionesRepository.SoftDeleteSemilleroLineaAsync(semilleroId, lineaId);

    // 5. Línea - Área Conocimiento (HU 9)
    public async Task<IEnumerable<LineaArea>> GetAllLineaAreaActivosAsync() =>
        await _relacionesRepository.GetAllLineaAreaActivosAsync();

    public async Task<bool> CreateLineaAreaAsync(LineaArea entity)
    {
        var linea = await _lineaRepository.GetByIdAsync(entity.LineaId);
        if (linea == null || !linea.Activo)
            throw new ArgumentException($"La línea de investigación con ID {entity.LineaId} no existe o se encuentra inactiva.");

        var area = await _areaConocimientoRepository.GetByIdAsync(entity.AreaConocimientoId);
        if (area == null || !area.Activo)
            throw new ArgumentException($"El área de conocimiento con ID {entity.AreaConocimientoId} no existe o se encuentra inactiva.");

        return await _relacionesRepository.CreateLineaAreaAsync(entity);
    }

    public async Task<bool> SoftDeleteLineaAreaAsync(int lineaId, int areaConocimientoId) =>
        await _relacionesRepository.SoftDeleteLineaAreaAsync(lineaId, areaConocimientoId);

    // 6. Línea - ODS (HU 9)
    public async Task<IEnumerable<LineaOds>> GetAllLineaOdsActivosAsync() =>
        await _relacionesRepository.GetAllLineaOdsActivosAsync();

    public async Task<bool> CreateLineaOdsAsync(LineaOds entity)
    {
        var linea = await _lineaRepository.GetByIdAsync(entity.LineaId);
        if (linea == null || !linea.Activo)
            throw new ArgumentException($"La línea de investigación con ID {entity.LineaId} no existe o se encuentra inactiva.");

        var ods = await _odsRepository.GetByIdAsync(entity.OdsId);
        if (ods == null || !ods.Activo)
            throw new ArgumentException($"El ODS con ID {entity.OdsId} no existe o se encuentra inactivo.");

        return await _relacionesRepository.CreateLineaOdsAsync(entity);
    }

    public async Task<bool> SoftDeleteLineaOdsAsync(int lineaId, int odsId) =>
        await _relacionesRepository.SoftDeleteLineaOdsAsync(lineaId, odsId);

    // 7. Línea - Área Aplicación (HU 9)
    public async Task<IEnumerable<LineaAreaAplicacion>> GetAllLineaAreaAplicacionActivosAsync() =>
        await _relacionesRepository.GetAllLineaAreaAplicacionActivosAsync();

    public async Task<bool> CreateLineaAreaAplicacionAsync(LineaAreaAplicacion entity)
    {
        var linea = await _lineaRepository.GetByIdAsync(entity.LineaId);
        if (linea == null || !linea.Activo)
            throw new ArgumentException($"La línea de investigación con ID {entity.LineaId} no existe o se encuentra inactiva.");

        var areaApp = await _areaAplicacionRepository.GetByIdAsync(entity.AreaAplicacionId);
        if (areaApp == null || !areaApp.Activo)
            throw new ArgumentException($"El área de aplicación con ID {entity.AreaAplicacionId} no existe o se encuentra inactiva.");

        return await _relacionesRepository.CreateLineaAreaAplicacionAsync(entity);
    }

    public async Task<bool> SoftDeleteLineaAreaAplicacionAsync(int lineaId, int areaAplicacionId) =>
        await _relacionesRepository.SoftDeleteLineaAreaAplicacionAsync(lineaId, areaAplicacionId);
}