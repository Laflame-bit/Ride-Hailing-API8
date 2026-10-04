using Microsoft.Data.SqlClient;

namespace RideHailingAPI.Data;

public class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        return new SqlConnection(connectionString);
    }
}