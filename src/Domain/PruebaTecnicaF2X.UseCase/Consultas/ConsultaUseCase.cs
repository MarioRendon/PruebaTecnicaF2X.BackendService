using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.UseCase.Consultas
{
    public class ConsultaUseCase: IConsultaUseCase
    {
        private readonly IConexionApiAdapter conexionApiAdapter;
        private readonly IRecaudosRepository recaudosRepository;

        public ConsultaUseCase(
            IConexionApiAdapter conexionApiAdapter,
            IRecaudosRepository recaudosRepository)
        {
            this.conexionApiAdapter = conexionApiAdapter;
            this.recaudosRepository = recaudosRepository;
        }
        
        /// <summary>
        /// Consulta la informacion de los datos
        /// </summary>
        /// <param name="consultaRequest"></param>
        /// <returns></returns>
        public async Task<ConsultaResponse> ConsultarInformacion(ConsultaRequest consultaRequest)
        {
            List<Recaudos> recaudos= await recaudosRepository.ConsultaRecaudos(consultaRequest);

            ConsultaResponse consultaResponse = new ConsultaResponse() { Datos = recaudos.ToList() };
           return consultaResponse;
        
        }

        /// <summary>
        /// Genera el reporte
        /// </summary>
        /// <returns></returns>
        public async Task<InformeResponse> GenerarInforme()
        {
            List<Recaudos> recaudos = await recaudosRepository.ConsultaRecaudos( new ConsultaRequest() );

            if (recaudos.Count > 0)
            {
                List<string> lEstaciones = (from estaciones in recaudos
                                            group estaciones by estaciones.Estacion into estaciones
                                            select estaciones.Key).ToList();

                List<Recaudos> recaudosTotales = (from tRecaudos in recaudos
                                                  group tRecaudos by new { tRecaudos.Estacion, tRecaudos.Hora } into _tRecaudos
                                                  orderby _tRecaudos.Key.Hora ascending
                                                  select new Recaudos()
                                                  {
                                                      Hora = _tRecaudos.Key.Hora,
                                                      Estacion = _tRecaudos.Key.Estacion.ToString(),
                                                      Cantidad = _tRecaudos.Sum(x => x.Cantidad),
                                                      ValorTabulado = _tRecaudos.Sum(x => x.ValorTabulado),
                                                  }).ToList();


                return new InformeResponse() { reporte = CreacionPLantilla(lEstaciones, recaudosTotales) };
            }
            return new InformeResponse() { reporte = Constants.RESPUESTANOGENERACIONREPORTE };
        }

        /// <summary>
        /// Para organizar lo encabezados del reporte
        /// </summary>
        /// <param name="lEstaciones"></param>
        /// <returns></returns>
        private string CrearEncabezadoEstacion(List<string> lEstaciones)
        {
            string pRegistroEstacion = "<th colspan='2'>{0}</th>";

            string registroEstacion = "";
            foreach (string item in lEstaciones)
            {
                registroEstacion += string.Format(pRegistroEstacion, item);
            }

            return registroEstacion;
        }

        /// <summary>
        /// Crear cada uno de los registros del reporte
        /// </summary>
        /// <param name="recaudosTotales"></param>
        /// <returns></returns>
        private string CrearRegistroFecha(List<Recaudos> recaudosTotales)
        {
            string pFechas = "<tr> <th>Fecha {0}</th> {1}</tr>";
            string pRegistroCantidadRecaudo = " <th>{0}</th> <th>{1}</th>";

            string registroXFecha = "";
            string acumuladoRegistroXFecha = "";
            int fecha = recaudosTotales.FirstOrDefault().Hora;
            foreach (Recaudos item in recaudosTotales)
            {
                if (fecha.Equals(item.Hora))
                    registroXFecha += string.Format(pRegistroCantidadRecaudo, item.Cantidad?.ToString("N0"), item.ValorTabulado?.ToString("N0"));
                else
                {
                    acumuladoRegistroXFecha += string.Format(pFechas, fecha, registroXFecha);
                    registroXFecha = string.Format(pRegistroCantidadRecaudo, item.Cantidad?.ToString("N0"), item.ValorTabulado?.ToString("N0"));
                }

                fecha = item.Hora;
            }
            #region Organizar totales x estacion
            acumuladoRegistroXFecha += CrearTotales(recaudosTotales);
            #endregion
            return acumuladoRegistroXFecha;
        }
        /// <summary>
        /// Crea los totales
        /// </summary>
        /// <param name="recaudos"></param>
        /// <returns></returns>
        private string CrearTotales(List<Recaudos> recaudos)
        {

            List<Recaudos> recaudosTotales = (from tRecaudos in recaudos
                                              group tRecaudos by new { tRecaudos.Estacion } into _tRecaudos
                                              orderby _tRecaudos.Key.Estacion ascending
                                              select new Recaudos()
                                              {
                                                  Estacion = _tRecaudos.Key.Estacion.ToString(),
                                                  Cantidad = _tRecaudos.Sum(x => x.Cantidad),
                                                  ValorTabulado = _tRecaudos.Sum(x => x.ValorTabulado),
                                              }).ToList();
            string pFechas = "<tr> <th>Total</th> {0}</tr>";
            string pRegistroCantidadRecaudo = " <th>{0}</th> <th>{1}</th>";
            string registroTotal = "";
            foreach (Recaudos item in recaudosTotales)
            {
                registroTotal += string.Format(pRegistroCantidadRecaudo, item.Cantidad?.ToString("N0"), item.ValorTabulado?.ToString("N0"));
            }
            string result = string.Format(pFechas, registroTotal).ToString();
            return result;
        }

        /// <summary>
        /// Consolida toda la plantilla del reporte
        /// </summary>
        /// <param name="lEstaciones"></param>
        /// <param name="recaudosTotales"></param>
        /// <returns></returns>
        private string CreacionPLantilla(List<string> lEstaciones, List<Recaudos> recaudosTotales) {

            string estaciones = CrearEncabezadoEstacion(lEstaciones);
            string registros =  CrearRegistroFecha(recaudosTotales);
            double? totalCantidad = recaudosTotales.Sum(x => x.Cantidad);
            double? totalRecaudo = recaudosTotales.Sum(x => x.ValorTabulado);
            string reporte = string.Format("<table border='1'><tr><th></th>{0}</tr>{1}<table><table border='1'><tr><th rowspan='2'>Totales</th><th>{2}</th></tr><tr ><th>{3}</th></tr></table>", estaciones, registros, totalCantidad?.ToString("N0"), totalRecaudo?.ToString("N0"));


            return reporte;
        }
    }
}
