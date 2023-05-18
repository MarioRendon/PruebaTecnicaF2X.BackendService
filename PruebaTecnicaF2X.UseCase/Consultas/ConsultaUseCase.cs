using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        

        public async Task<ConsultaResponse> ConsultarInformacion()
        {
            List<Recaudos> recaudos= await recaudosRepository.ConsultaRecaudos();
            

            return new ConsultaResponse() { Datos = recaudos };
        
        }


        public async Task<InformeResponse> GenerarInforme()
        {
            List<Recaudos> recaudos = await recaudosRepository.ConsultaRecaudos();

            List<string> lEstaciones = (from estaciones in recaudos
                                   group estaciones by estaciones.Estacion into estaciones
                                   select estaciones.Key).ToList();

            List<Recaudos> recaudosTotales = (from tRecaudos in recaudos
                                              group tRecaudos by new { tRecaudos.Estacion, tRecaudos.Hora } into _tRecaudos
                                              orderby _tRecaudos.Key.Hora ascending
                                              select new Recaudos()
                                              {
                                                  Hora=_tRecaudos.Key.Hora,
                                                  Estacion=_tRecaudos.Key.Estacion.ToString(),
                                                  Cantidad= _tRecaudos.Sum(x=>x.Cantidad),
                                                  ValorTabulado=_tRecaudos.Sum(x=>x.ValorTabulado),
                                              }).ToList();
            

            return new InformeResponse() { reporte = await CreacionPLantilla(lEstaciones, recaudosTotales)};
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
                if (fecha == item.Hora)
                    registroXFecha += string.Format(pRegistroCantidadRecaudo, item.Cantidad, item.ValorTabulado);
                else
                {
                    acumuladoRegistroXFecha += string.Format(pFechas, fecha, registroXFecha);
                    registroXFecha = string.Format(pRegistroCantidadRecaudo, item.Cantidad, item.ValorTabulado);
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
                registroTotal += string.Format(pRegistroCantidadRecaudo, item.Cantidad, item.ValorTabulado);
            }
            string result = string.Format(pFechas, registroTotal).ToString();
            return result;
        }

        private async Task<string> CreacionPLantilla(List<string> lEstaciones, List<Recaudos> recaudosTotales) {

            string estaciones = await CrearEncabezadoEstacion(lEstaciones);
            string registros = await CrearRegistroFecha(recaudosTotales);
            int? totalCantidad = recaudosTotales.Sum(x => x.Cantidad);
            decimal? totalRecaudo = recaudosTotales.Sum(x => x.ValorTabulado);
            string reporte = string.Format("<html><body><table border='1'><tr><th></th>{0}</tr>{1}<table><table border='1'><tr><th rowspan='2'>Totales</th><th>{2}</th></tr><tr ><th>{3}</th></tr></table></body></html>", estaciones, registros, totalCantidad,totalRecaudo);


            return reporte;
        }
    }
}
