using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Modelos;
using MIRA.Api.Servicios;

namespace MIRA.Api.Peticiones;

[ApiController]
[Route("api/objetivo_desarrollo_sostenible")]
public class ObjetivoDesarrolloSostenibleController : ControllerBase
{
    private readonly IObjetivoDesarrolloSostenibleService _service;

    public ObjetivoDesarrolloSostenibleController(IObjetivoDesarrolloSostenibleService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ObjetivoDesarrolloSostenible>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActivosAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ObjetivoDesarrolloSostenible), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { mensaje = $"Objetivo de desarrollo sostenible con ID {id} no encontrado o inactivo." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ObjetivoDesarrolloSostenible), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ObjetivoDesarrolloSostenible entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ObjetivoDesarrolloSostenible entity)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, entity);
            if (!updated)
                return NotFound(new { mensaje = $"Objetivo de desarrollo sostenible con ID {id} no encontrado para actualizar." });

            return Ok(new { mensaje = "Objetivo de desarrollo sostenible actualizado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.SoftDeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Objetivo de desarrollo sostenible con ID {id} no encontrado para eliminar." });

        return Ok(new { mensaje = "Objetivo de desarrollo sostenible eliminado lógicamente." });
    }
}

