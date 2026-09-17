using System.Data;
using Npgsql;

namespace MIRA.Api.Configuracion;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'PostgreSql' en la configuración.");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}

