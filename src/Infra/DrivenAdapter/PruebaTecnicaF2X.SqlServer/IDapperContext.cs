using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.SqlServer
{
    public interface IDapperContext
    {
        public IDbConnection CrearConexion();
    }
}
