using Microsoft.AspNetCore.Authorization;
namespace MIRA.Api.Peticiones;

using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Modelos;
using MIRA.Api.Servicios;

[ApiController]
[Route("api")]
[Authorize]
public class RelacionesController : ControllerBase
{
    private readonly IRelacionesService _service;

    public RelacionesController(IRelacionesService service)
    {
        _service = service;
    }

    // 1. Docente - Grupo (HU 5)
    [HttpGet("docente_grupo")]
    public async Task<IActionResult> GetAllDocenteGrupo() =>
        Ok(await _service.GetAllDocenteGrupoActivosAsync());

    [HttpPost("docente_grupo")]
    public async Task<IActionResult> CreateDocenteGrupo([FromBody] DocenteGrupo entity)
    {
        try
        {
            await _service.CreateDocenteGrupoAsync(entity);
            return Ok(new { mensaje = "Docente vinculado al grupo de investigación exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("docente_grupo")]
    public async Task<IActionResult> DeleteDocenteGrupo([FromQuery] int docenteCedula, [FromQuery] int grupoId)
    {
        var deleted = await _service.SoftDeleteDocenteGrupoAsync(docenteCedula, grupoId);
        if (!deleted) return NotFound(new { mensaje = "Relación docente-grupo no encontrada para desactivar." });
        return Ok(new { mensaje = "Vinculación desactivada exitosamente (borrado lógico)." });
    }

    // 2. Docente - Semillero (HU 6)
    [HttpGet("docente_semillero")]
    public async Task<IActionResult> GetAllDocenteSemillero() =>
        Ok(await _service.GetAllDocenteSemilleroActivosAsync());

    [HttpPost("docente_semillero")]
    public async Task<IActionResult> CreateDocenteSemillero([FromBody] DocenteSemillero entity)
    {
        try
        {
            await _service.CreateDocenteSemilleroAsync(entity);
            return Ok(new { mensaje = "Tutor asignado al semillero exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("docente_semillero")]
    public async Task<IActionResult> DeleteDocenteSemillero([FromQuery] int docenteCedula, [FromQuery] int semilleroId, [FromQuery] string periodoAcademico)
    {
        var deleted = await _service.SoftDeleteDocenteSemilleroAsync(docenteCedula, semilleroId, periodoAcademico);
        if (!deleted) return NotFound(new { mensaje = "Relación docente-semillero no encontrada para desactivar." });
        return Ok(new { mensaje = "Tutoría de semillero desactivada exitosamente (borrado lógico)." });
    }

    // 3. Grupo - Línea (HU 8)
    [HttpGet("grupo_linea")]
    public async Task<IActionResult> GetAllGrupoLinea() =>
        Ok(await _service.GetAllGrupoLineaActivosAsync());

    [HttpPost("grupo_linea")]
    public async Task<IActionResult> CreateGrupoLinea([FromBody] GrupoLinea entity)
    {
        try
        {
            await _service.CreateGrupoLineaAsync(entity);
            return Ok(new { mensaje = "Grupo articulado a la línea de investigación exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("grupo_linea")]
    public async Task<IActionResult> DeleteGrupoLinea([FromQuery] int grupoId, [FromQuery] int lineaId)
    {
        var deleted = await _service.SoftDeleteGrupoLineaAsync(grupoId, lineaId);
        if (!deleted) return NotFound(new { mensaje = "Articulación grupo-línea no encontrada para desactivar." });
        return Ok(new { mensaje = "Articulación desactivada exitosamente (borrado lógico)." });
    }

    // 4. Semillero - Línea (HU 8)
    [HttpGet("semillero_linea")]
    public async Task<IActionResult> GetAllSemilleroLinea() =>
        Ok(await _service.GetAllSemilleroLineaActivosAsync());

    [HttpPost("semillero_linea")]
    public async Task<IActionResult> CreateSemilleroLinea([FromBody] SemilleroLinea entity)
    {
        try
        {
            await _service.CreateSemilleroLineaAsync(entity);
            return Ok(new { mensaje = "Semillero articulado a la línea de investigación exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("semillero_linea")]
    public async Task<IActionResult> DeleteSemilleroLinea([FromQuery] int semilleroId, [FromQuery] int lineaId)
    {
        var deleted = await _service.SoftDeleteSemilleroLineaAsync(semilleroId, lineaId);
        if (!deleted) return NotFound(new { mensaje = "Articulación semillero-línea no encontrada para desactivar." });
        return Ok(new { mensaje = "Articulación desactivada exitosamente (borrado lógico)." });
    }

    // 5. Línea - Área Conocimiento (HU 9)
    [HttpGet("linea_area")]
    public async Task<IActionResult> GetAllLineaArea() =>
        Ok(await _service.GetAllLineaAreaActivosAsync());

    [HttpPost("linea_area")]
    public async Task<IActionResult> CreateLineaArea([FromBody] LineaArea entity)
    {
        try
        {
            await _service.CreateLineaAreaAsync(entity);
            return Ok(new { mensaje = "Línea articulada al área de conocimiento exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("linea_area")]
    public async Task<IActionResult> DeleteLineaArea([FromQuery] int lineaId, [FromQuery] int areaConocimientoId)
    {
        var deleted = await _service.SoftDeleteLineaAreaAsync(lineaId, areaConocimientoId);
        if (!deleted) return NotFound(new { mensaje = "Articulación línea-área no encontrada para desactivar." });
        return Ok(new { mensaje = "Articulación desactivada exitosamente (borrado lógico)." });
    }

    // 6. Línea - ODS (HU 9)
    [HttpGet("linea_ods")]
    public async Task<IActionResult> GetAllLineaOds() =>
        Ok(await _service.GetAllLineaOdsActivosAsync());

    [HttpPost("linea_ods")]
    public async Task<IActionResult> CreateLineaOds([FromBody] LineaOds entity)
    {
        try
        {
            await _service.CreateLineaOdsAsync(entity);
            return Ok(new { mensaje = "Línea articulada al ODS exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("linea_ods")]
    public async Task<IActionResult> DeleteLineaOds([FromQuery] int lineaId, [FromQuery] int odsId)
    {
        var deleted = await _service.SoftDeleteLineaOdsAsync(lineaId, odsId);
        if (!deleted) return NotFound(new { mensaje = "Articulación línea-ODS no encontrada para desactivar." });
        return Ok(new { mensaje = "Articulación desactivada exitosamente (borrado lógico)." });
    }

    // 7. Línea - Área Aplicación (HU 9)
    [HttpGet("linea_area_aplicacion")]
    public async Task<IActionResult> GetAllLineaAreaAplicacion() =>
        Ok(await _service.GetAllLineaAreaAplicacionActivosAsync());

    [HttpPost("linea_area_aplicacion")]
    public async Task<IActionResult> CreateLineaAreaAplicacion([FromBody] LineaAreaAplicacion entity)
    {
        try
        {
            await _service.CreateLineaAreaAplicacionAsync(entity);
            return Ok(new { mensaje = "Línea articulada al área de aplicación exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("linea_area_aplicacion")]
    public async Task<IActionResult> DeleteLineaAreaAplicacion([FromQuery] int lineaId, [FromQuery] int areaAplicacionId)
    {
        var deleted = await _service.SoftDeleteLineaAreaAplicacionAsync(lineaId, areaAplicacionId);
        if (!deleted) return NotFound(new { mensaje = "Articulación línea-área de aplicación no encontrada para desactivar." });
        return Ok(new { mensaje = "Articulación desactivada exitosamente (borrado lógico)." });
    }
}