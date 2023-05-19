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
        

        public async Task<ConsultaResponse> ConsultarInformacion(ConsultaRequest consultaRequest)
        {
            List<Recaudos> recaudos= await recaudosRepository.ConsultaRecaudos(consultaRequest);

            ConsultaResponse consultaResponse = new ConsultaResponse() { Datos = recaudos.ToList() };
           return consultaResponse;
        
        }


        public async Task<InformeResponse> GenerarInforme()
        {
            List<Recaudos> recaudos = await recaudosRepository.ConsultaRecaudos( new ConsultaRequest());

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


                return new InformeResponse() { reporte = await CreacionPLantilla(lEstaciones, recaudosTotales) };
            }
            return new InformeResponse() { reporte = Constants.RESPUESTANOGENERACIONREPORTE };
        }


        private async Task<string> CrearEncabezadoEstacion(List<string> lEstaciones)
        {
            string pRegistroEstacion = "<th colspan='2'>{0}</th>";

            string registroEstacion = "";
            foreach (string item in lEstaciones)
            {
                registroEstacion += string.Format(pRegistroEstacion, item);
            }

            return registroEstacion;
        }

        private async Task<string> CrearRegistroFecha(List<Recaudos> recaudosTotales)
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
            acumuladoRegistroXFecha += await CrearTotales(recaudosTotales);
            #endregion
            return acumuladoRegistroXFecha;
        }

        private async Task<string> CrearTotales(List<Recaudos> recaudos)
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

        private async Task<string> CreacionPLantilla(List<string> lEstaciones, List<Recaudos> recaudosTotales) {

            string estaciones = await CrearEncabezadoEstacion(lEstaciones);
            string registros = await CrearRegistroFecha(recaudosTotales);
            double? totalCantidad = recaudosTotales.Sum(x => x.Cantidad);
            double? totalRecaudo = recaudosTotales.Sum(x => x.ValorTabulado);
            string reporte = string.Format("<table border='1'><tr><th></th>{0}</tr>{1}<table><table border='1'><tr><th rowspan='2'>Totales</th><th>{2}</th></tr><tr ><th>{3}</th></tr></table>", estaciones, registros, totalCantidad?.ToString("N0"), totalRecaudo?.ToString("N0"));


            return reporte;
        }
    }
}
