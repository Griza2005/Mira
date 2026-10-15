using System.Threading.Tasks;
using MIRA.Api.Peticiones;

namespace MIRA.Api.Servicios
{
    public interface IAuthService
    {
        Task<LoginResponse?> AutenticarAsync(LoginRequest peticion);
    }
}