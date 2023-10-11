using System.Data;

namespace CardsLand_Api.Interfaces
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
