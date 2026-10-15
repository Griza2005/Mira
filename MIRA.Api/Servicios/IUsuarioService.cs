using System.Collections.Generic;
using System.Threading.Tasks;
using MIRA.Api.Peticiones;

namespace MIRA.Api.Servicios
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponse>> ObtenerTodosAsync();
        Task<UsuarioResponse?> ObtenerPorIdAsync(int id);
        Task<UsuarioResponse> CrearAsync(UsuarioCrear peticion);
        Task<bool> EliminarAsync(int id);
    }
}