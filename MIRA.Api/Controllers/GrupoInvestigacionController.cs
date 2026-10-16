using Microsoft.AspNetCore.Authorization;
namespace MIRA.Api.Peticiones;

using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Modelos;
using MIRA.Api.Servicios;

[ApiController]
[Route("api/grupo_investigacion")]
[Authorize]
public class GrupoInvestigacionController : ControllerBase
{
    private readonly IGrupoInvestigacionService _service;

    public GrupoInvestigacionController(IGrupoInvestigacionService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GrupoInvestigacion>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActivosAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GrupoInvestigacion), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { mensaje = $"Grupo de investigación con ID {id} no encontrado o inactivo." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GrupoInvestigacion), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] GrupoInvestigacion entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] GrupoInvestigacion entity)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, entity);
            if (!updated)
                return NotFound(new { mensaje = $"Grupo de investigación con ID {id} no encontrado para actualizar." });

            return Ok(new { mensaje = "Grupo de investigación actualizado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.SoftDeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Grupo de investigación con ID {id} no encontrado para eliminar." });

        return Ok(new { mensaje = "Grupo de investigación desactivado exitosamente (borrado lógico)." });
    }
}