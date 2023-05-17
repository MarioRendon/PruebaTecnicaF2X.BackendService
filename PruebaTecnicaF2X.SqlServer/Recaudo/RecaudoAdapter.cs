using AutoMapper;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.SqlServer.Recaudo;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PruebaTecnicaF2X.SqlServer
{
    public class RecaudoAdapter
    {

        public readonly IDapperContext context;
        public readonly IMapper mapper;

        public RecaudoAdapter(IMapper mapper, IDapperContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<List<Recaudos>> Consulta()
        {
            string sqlQuery = $"SELECT * FROM {Constants.NOMBRETABLARECAUDO}";
            using var conexion = context.CrearConexion();
            var result = await conexion.QueryAsync<RecaudosEntity>(sqlQuery);

            return mapper.Map<List<Recaudos>>(result);
        }
    }
}
