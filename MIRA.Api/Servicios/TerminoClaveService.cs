using MIRA.Api.Modelos;
using MIRA.Api.Repositorios;

namespace MIRA.Api.Servicios;

public class TerminoClaveService : ITerminoClaveService
{
    private readonly ITerminoClaveRepository _repository;

    public TerminoClaveService(ITerminoClaveRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TerminoClave>> GetAllActivosAsync()
    {
        return await _repository.GetAllActivosAsync();
    }

    public async Task<TerminoClave?> GetByTerminoAsync(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return null;
        return await _repository.GetByTerminoAsync(termino.Trim());
    }

    public async Task<TerminoClave> CreateAsync(TerminoClave entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Termino))
            throw new ArgumentException("El campo 'termino' es obligatorio.");

        entity.Termino = entity.Termino.Trim();
        entity.TerminoIngles = entity.TerminoIngles?.Trim();
        entity.Activo = true;

        var existing = await _repository.GetByTerminoAsync(entity.Termino);
        if (existing != null)
            throw new InvalidOperationException($"El término clave '{entity.Termino}' ya existe.");

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(string termino, TerminoClave entity)
    {
        if (string.IsNullOrWhiteSpace(termino)) return false;

        var existing = await _repository.GetByTerminoAsync(termino.Trim());
        if (existing == null) return false;

        entity.Termino = termino.Trim();
        entity.TerminoIngles = entity.TerminoIngles?.Trim();

        return await _repository.UpdateAsync(termino.Trim(), entity);
    }

    public async Task<bool> SoftDeleteAsync(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return false;

        var existing = await _repository.GetByTerminoAsync(termino.Trim());
        if (existing == null) return false;

        return await _repository.SoftDeleteAsync(termino.Trim());
    }
}

