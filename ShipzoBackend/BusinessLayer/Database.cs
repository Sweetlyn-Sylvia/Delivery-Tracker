
using System.Data;
using Microsoft.Data.SqlClient;
namespace ShipzoBackend.BusinessLayer
{
    public class Database
    {
        private readonly string connectionString;

        public Database(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public DataTable ExecuteProcedure(string procedureName, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(procedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(table);
            }
            return table;
        }
    }
}
