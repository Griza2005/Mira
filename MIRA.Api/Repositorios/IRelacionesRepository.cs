namespace MIRA.Api.Repositorios;

using MIRA.Api.Modelos;

/// <summary>
/// Contrato de acceso a datos para las 7 tablas puente de articulación académica.
/// </summary>
public interface IRelacionesRepository
{
    // Docente - Grupo (HU 5)
    Task<IEnumerable<DocenteGrupo>> GetAllDocenteGrupoActivosAsync();
    Task<bool> CreateDocenteGrupoAsync(DocenteGrupo entity);
    Task<bool> SoftDeleteDocenteGrupoAsync(int docenteCedula, int grupoId);

    // Docente - Semillero (HU 6)
    Task<IEnumerable<DocenteSemillero>> GetAllDocenteSemilleroActivosAsync();
    Task<bool> CreateDocenteSemilleroAsync(DocenteSemillero entity);
    Task<bool> SoftDeleteDocenteSemilleroAsync(int docenteCedula, int semilleroId, string periodoAcademico);

    // Grupo - Línea (HU 8)
    Task<IEnumerable<GrupoLinea>> GetAllGrupoLineaActivosAsync();
    Task<bool> CreateGrupoLineaAsync(GrupoLinea entity);
    Task<bool> SoftDeleteGrupoLineaAsync(int grupoId, int lineaId);

    // Semillero - Línea (HU 8)
    Task<IEnumerable<SemilleroLinea>> GetAllSemilleroLineaActivosAsync();
    Task<bool> CreateSemilleroLineaAsync(SemilleroLinea entity);
    Task<bool> SoftDeleteSemilleroLineaAsync(int semilleroId, int lineaId);

    // Línea - Área Conocimiento (HU 9)
    Task<IEnumerable<LineaArea>> GetAllLineaAreaActivosAsync();
    Task<bool> CreateLineaAreaAsync(LineaArea entity);
    Task<bool> SoftDeleteLineaAreaAsync(int lineaId, int areaConocimientoId);

    // Línea - ODS (HU 9)
    Task<IEnumerable<LineaOds>> GetAllLineaOdsActivosAsync();
    Task<bool> CreateLineaOdsAsync(LineaOds entity);
    Task<bool> SoftDeleteLineaOdsAsync(int lineaId, int odsId);

    // Línea - Área Aplicación (HU 9)
    Task<IEnumerable<LineaAreaAplicacion>> GetAllLineaAreaAplicacionActivosAsync();
    Task<bool> CreateLineaAreaAplicacionAsync(LineaAreaAplicacion entity);
    Task<bool> SoftDeleteLineaAreaAplicacionAsync(int lineaId, int areaAplicacionId);
}