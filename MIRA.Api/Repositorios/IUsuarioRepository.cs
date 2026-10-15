using System.Collections.Generic;
using System.Threading.Tasks;
using MIRA.Api.Modelos;

namespace MIRA.Api.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorUsernameAsync(string username);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<bool> ExisteUsernameOEmailAsync(string username, string email);
        Task<int> CrearAsync(Usuario usuario, List<int> rolesIds);
        Task<bool> EliminarLogicoAsync(int id);
    }
}