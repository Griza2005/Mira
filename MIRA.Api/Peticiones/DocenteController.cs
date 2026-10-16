using Microsoft.AspNetCore.Authorization;
namespace MIRA.Api.Peticiones;

using Microsoft.AspNetCore.Mvc;
using MIRA.Api.Modelos;
using MIRA.Api.Servicios;

[ApiController]
[Route("api/docente")]
[Authorize]
public class DocenteController : ControllerBase
{
    private readonly IDocenteService _service;

    public DocenteController(IDocenteService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Docente>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActivosAsync();
        return Ok(result);
    }

    [HttpGet("{cedula:int}")]
    [ProducesResponseType(typeof(Docente), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int cedula)
    {
        var item = await _service.GetByIdAsync(cedula);
        if (item == null)
            return NotFound(new { mensaje = $"Docente con cédula {cedula} no encontrado o inactivo." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Docente), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] Docente entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { cedula = created.Cedula }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpPut("{cedula:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int cedula, [FromBody] Docente entity)
    {
        try
        {
            var updated = await _service.UpdateAsync(cedula, entity);
            if (!updated)
                return NotFound(new { mensaje = $"Docente con cédula {cedula} no encontrado para actualizar." });

            return Ok(new { mensaje = "Docente actualizado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = ex.Message });
        }
    }

    [HttpDelete("{cedula:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int cedula)
    {
        var deleted = await _service.SoftDeleteAsync(cedula);
        if (!deleted)
            return NotFound(new { mensaje = $"Docente con cédula {cedula} no encontrado para eliminar." });

        return Ok(new { mensaje = "Docente desactivado exitosamente (borrado lógico)." });
    }
}