using CardsLand_Api.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace CardsLand_Api.Implementations
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly IConfiguration _configuration;
        public DbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
