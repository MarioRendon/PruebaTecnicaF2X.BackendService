﻿using Newtonsoft.Json;
using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Conteo;
using PruebaTecnicaF2X.Model.Recaudo;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;

namespace PruebaTecnicaF2X.UseCase.ProcesarInformacion
{
    public class ProcesarUseCase : IProcesarUseCase
    {

        private readonly IConexionApiAdapter conexionApiAdapter;
        private readonly IRecaudosRepository recaudosRepository;

        public ProcesarUseCase(
            IConexionApiAdapter conexionApiAdapter,
            IRecaudosRepository recaudosRepository)
        {
            this.conexionApiAdapter = conexionApiAdapter;
            this.recaudosRepository = recaudosRepository;
        }

        /// <summary>
        /// metodo para procesar la informacion
        /// </summary>
        /// <returns></returns>
        public async Task Procesar()
        {
            await recaudosRepository.LimpiarData();


            string token = await Login();
            DateTime i = new DateTime(2021, 8, 1);
            while (i < new DateTime(2021, 10, 6))
            {
                await Consultar(token, i.ToString("yyyy-MM-dd"));
                i = i.AddDays(1);
            }
        }

        /// <summary>
        /// metodo para loguearse al servicio para obtener la informacion
        /// </summary>
        /// <returns></returns>
        private async Task<string> Login()
        {
            return await conexionApiAdapter.Conectar(Constants.USUARIO, Constants.CLAVE);
        }


        /// <summary>
        /// metodo para hacer la consulta de los datos y la union de los registros, donde cantidad y valor quedando en un solo registros
        /// </summary>
        /// <param name="token"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        private async Task<bool> Consultar(string token, string fecha)
        {

            try
            {
                var resultConteo = await conexionApiAdapter.Consumir(Constants.APICONTEO, token, fecha);
               var resultRecaudo = await conexionApiAdapter.Consumir(Constants.APIRECAUDO, token, fecha);
                if (!string.IsNullOrEmpty(resultConteo.Content) && !string.IsNullOrEmpty(resultRecaudo.Content))
                {
                    List<ConteoVehiculos> conteoVehiculos = (from rVehiculos in JsonConvert.DeserializeObject<List<ConteoVehiculos>>(resultConteo.Content)
                                                             group rVehiculos by new
                                                             {
                                                                 rVehiculos.Sentido,
                                                                 rVehiculos.Estacion,
                                                                 rVehiculos.Hora,
                                                                 rVehiculos.Categoria
                                                             } into _rVehiculos
                                                             select new ConteoVehiculos()
                                                             {
                                                                 Categoria = _rVehiculos.First().Categoria,
                                                                 Estacion = _rVehiculos.First().Estacion,
                                                                 Hora = _rVehiculos.First().Hora,
                                                                 Sentido = _rVehiculos.First().Sentido,
                                                                 Cantidad = _rVehiculos.Sum(x => x.Cantidad)
                                                             }
                                                            ).ToList();

                    

                    List<RecaudoVehiculo> recaudoVehiculo = (from rVehiculos in JsonConvert.DeserializeObject<List<RecaudoVehiculo>>(resultRecaudo.Content)
                                                             group rVehiculos by new
                                                             {
                                                                 rVehiculos.Sentido,
                                                                 rVehiculos.Estacion,
                                                                 rVehiculos.Hora,
                                                                 rVehiculos.Categoria
                                                             } into _rVehiculos
                                                             select new RecaudoVehiculo()
                                                             {
                                                                 Categoria = _rVehiculos.First().Categoria,
                                                                 Estacion = _rVehiculos.First().Estacion,
                                                                 Hora = _rVehiculos.First().Hora,
                                                                 Sentido = _rVehiculos.First().Sentido,
                                                                 ValorTabulado = _rVehiculos.Sum(x => x.ValorTabulado)
                                                             }
                                                            ).ToList();




                    List<Recaudos> lRecaudos = (from _conteoVehiculos in conteoVehiculos
                                                join _recaudoVehiculo in recaudoVehiculo on
                                                 new
                                                 {
                                                     _conteoVehiculos.Estacion,
                                                     _conteoVehiculos.Hora,
                                                     _conteoVehiculos.Categoria,
                                                     _conteoVehiculos.Sentido
                                                 } equals new
                                                 {
                                                     _recaudoVehiculo.Estacion,
                                                     _recaudoVehiculo.Hora,
                                                     _recaudoVehiculo.Categoria,
                                                     _recaudoVehiculo.Sentido
                                                 }
                                                select new Recaudos()
                                                {
                                                    Estacion = _conteoVehiculos.Estacion,
                                                    Hora = _conteoVehiculos.Hora,
                                                    Categoria = _conteoVehiculos.Categoria,
                                                    Sentido = _conteoVehiculos.Sentido,
                                                    Cantidad = _conteoVehiculos.Cantidad,
                                                    ValorTabulado = _recaudoVehiculo.ValorTabulado
                                                }).ToList();
                    return await Guardar(lRecaudos);
                }
                return false;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// metodo para guardar la informacion consultada los servicios
        /// </summary>
        /// <param name="recaudo"></param>
        /// <returns></returns>
        private async Task<bool> Guardar(List<Recaudos> recaudo)
        {
            return await recaudosRepository.IngresarDatosRecaudo(recaudo);
            
        }
        


    }
}
