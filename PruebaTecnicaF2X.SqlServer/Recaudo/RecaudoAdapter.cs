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
using PruebaTecnicaF2X.Model.Consultas;

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

        public async Task<List<Recaudos>> ConsultaRecaudos(ConsultaRequest consultaRequest)
        {
            string sqlCondicion = (string.IsNullOrEmpty(consultaRequest.Sentido) ? "" :$"{"Sentido like '%"}{consultaRequest.Sentido}{"%' AND "}");
            sqlCondicion += (string.IsNullOrEmpty(consultaRequest.Categoria) ? "" : $"{"Categoria like '%"}{consultaRequest.Categoria}{"%' AND "}");
            sqlCondicion += (consultaRequest.Hora==null ? "" : $"{"Hora like '%"}{consultaRequest.Hora}{"%' AND "}");
            sqlCondicion += (string.IsNullOrEmpty(consultaRequest.Estacion) ? "" : $"{"Estacion like '%"}{consultaRequest.Estacion}{"%' AND "}");
            
            if (sqlCondicion.Length > 0)
            {
                sqlCondicion = $"{" WHERE "}{sqlCondicion.Substring(0, sqlCondicion.Length - 4)}";
            }

            string sqlQuery = $"SELECT TOP 1000 * FROM {Constants.NOMBRETABLARECAUDO}{sqlCondicion}";
            using var conexion = context.CrearConexion();
            var result = await conexion.QueryAsync<RecaudosEntity>(sqlQuery);
            List<Recaudos> recaudos = mapper.Map<List<Recaudos>>(result);
            return recaudos;
        }

        public async Task<bool> IngresarDatosRecaudo(List<Recaudos> recaudos)
        {
            string valores = "('{0}','{1}','{2}','{3}',{4},{5})";
            string queryValores = "";
            foreach (var recaudo in recaudos)
            {
                queryValores += $"{string.Format(valores,recaudo.Estacion,recaudo.Sentido,recaudo.Hora,recaudo.Categoria,recaudo.ValorTabulado,recaudo.Cantidad)}{","}";
            }
             
            string sqlQuery = $"INSERT INTO {Constants.NOMBRETABLARECAUDO} ([Estacion],[Sentido],[Hora],[Categoria],[ValorTabulado],[Cantidad])VALUES{queryValores.Substring(0,queryValores.Length-1)}";
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
