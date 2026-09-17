using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Modelos;
using MIRA.Api.Servicios;

namespace MIRA.Api.Peticiones;

[ApiController]
[Route("api/termino_clave")]
public class TerminoClaveController : ControllerBase
{
    private readonly ITerminoClaveService _service;

    public TerminoClaveController(ITerminoClaveService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TerminoClave>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActivosAsync();
        return Ok(result);
    }

    [HttpGet("{termino}")]
    [ProducesResponseType(typeof(TerminoClave), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTermino(string termino)
    {
        var item = await _service.GetByTerminoAsync(termino);
        if (item == null)
            return NotFound(new { mensaje = $"Término clave '{termino}' no encontrado o inactivo." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TerminoClave), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] TerminoClave entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetByTermino), new { termino = created.Termino }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{termino}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string termino, [FromBody] TerminoClave entity)
    {
        try
        {
            var updated = await _service.UpdateAsync(termino, entity);
            if (!updated)
                return NotFound(new { mensaje = $"Término clave '{termino}' no encontrado para actualizar." });

            return Ok(new { mensaje = "Término clave actualizado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{termino}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string termino)
    {
        var deleted = await _service.SoftDeleteAsync(termino);
        if (!deleted)
            return NotFound(new { mensaje = $"Término clave '{termino}' no encontrado para eliminar." });

        return Ok(new { mensaje = "Término clave eliminado lógicamente." });
    }
}

