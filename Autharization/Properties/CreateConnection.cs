using System.Data.SqlClient;

namespace DataBase
{
    public static class CreateConnection
    {
        public static SqlConnection Create()
        {
            return new SqlConnection(Properties.ConnectionString);
        }
    }
}
