using System.Data;
using System.Data.SqlClient;

namespace PruebaTecnicaF2X.SqlServer
{
    public class DapperContext : IDapperContext
    {
        private readonly string dbSqlConexion;

        public DapperContext(string dbSqlConexion)
        {
            this.dbSqlConexion = dbSqlConexion;
        }

        public IDbConnection CrearConexion() => new SqlConnection(dbSqlConexion);
    }
}
