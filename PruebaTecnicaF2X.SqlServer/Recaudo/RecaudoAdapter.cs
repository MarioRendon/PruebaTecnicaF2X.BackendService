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
using PruebaTecnicaF2X.Model.Recaudo;
using PruebaTecnicaF2X.Model.Conteo;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;

namespace PruebaTecnicaF2X.SqlServer
{
    public class RecaudoAdapter: IRecaudosRepository
    {

        public readonly IDapperContext context;
        public readonly IMapper mapper;

        public RecaudoAdapter(IMapper mapper, IDapperContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<List<Recaudos>> ConsultaRecaudos()
        {
            string sqlQuery = $"SELECT  * FROM {Constants.NOMBRETABLARECAUDO}";
            using var conexion = context.CrearConexion();
            var result = await conexion.QueryAsync<RecaudosEntity>(sqlQuery);
            List<Recaudos> recaudos = mapper.Map<List<Recaudos>>(result);
            return recaudos;
        }

        public async Task<bool> IngresarDatosRecaudo(List<RecaudoVehiculo> recaudos)
        {
            string valores = "('{0}','{1}',{2},'{3}',{4})";
            string queryValores = "";
            foreach (var recaudo in recaudos)
            {
                queryValores += $"{string.Format(valores,recaudo.Estacion,recaudo.Sentido,recaudo.Hora,recaudo.Categoria,recaudo.ValorTabulado)}{","}";
            }
             
            string sqlQuery = $"INSERT INTO {Constants.NOMBRETABLARECAUDO} ([Estacion],[Sentido],[Hora],[Categoria],[ValorTabulado])VALUES{queryValores.Substring(0,queryValores.Length-1)}";
            using var conexion = context.CrearConexion();
            var result = await conexion.ExecuteAsync(sqlQuery);
            return result > 0;
        }

        public async Task<bool> IngresarDatosConteo(List<ConteoVehiculos> conteos)
        {
            string valores = "('{0}','{1}',{2},'{3}',{4})";
            string queryValores = "";
            foreach (var conteo in conteos)
            {
                queryValores += $"{string.Format(valores, conteo.Estacion, conteo.Sentido, conteo.Hora, conteo.Categoria, conteo.Cantidad)}{","}";
            }
             
            string sqlQuery = $"INSERT INTO {Constants.NOMBRETABLARECAUDO} ([Estacion],[Sentido],[Hora],[Categoria],[Cantidad])VALUES{queryValores.Substring(0, queryValores.Length - 1)}";
            using var conexion = context.CrearConexion();
            var result = await conexion.ExecuteAsync(sqlQuery);
            return result > 0;
        }


        public async Task<bool> LimpiarData()
        {
            

            string sqlQuery = $"Delete from {Constants.NOMBRETABLARECAUDO} ";
            using var conexion = context.CrearConexion();
            var result = await conexion.ExecuteAsync(sqlQuery);
            return result > 0;
        }
    }
}
