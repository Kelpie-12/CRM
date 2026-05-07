using System.Data.SqlClient;

namespace CRM.Repositories.Implementations
{
    public class BaseRepository
    {
        private static readonly string DEFAULT_CONNECTION_STRING = "CRM";
        // private static readonly string DEFAULT_CONNECTION_STRING = "Kelpie";
        private readonly string _connectionSttring;

        public BaseRepository(IConfiguration configuration)
        {
            string? defaultConnectionString = configuration.GetConnectionString(DEFAULT_CONNECTION_STRING);
            if (defaultConnectionString == null)
            {
                throw new Exception("Ошибка с строкой подключения");
            }
            _connectionSttring = defaultConnectionString;
        }
        protected SqlConnection CreateConnection()
        {
            //расширить
            return new SqlConnection(_connectionSttring);
        }
    }
}
