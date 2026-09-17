using MIRA.Api.Modelos;

namespace MIRA.Api.Servicios;

public interface ITerminoClaveService
{
    Task<IEnumerable<TerminoClave>> GetAllActivosAsync();
    Task<TerminoClave?> GetByTerminoAsync(string termino);
    Task<TerminoClave> CreateAsync(TerminoClave entity);
    Task<bool> UpdateAsync(string termino, TerminoClave entity);
    Task<bool> SoftDeleteAsync(string termino);
}

