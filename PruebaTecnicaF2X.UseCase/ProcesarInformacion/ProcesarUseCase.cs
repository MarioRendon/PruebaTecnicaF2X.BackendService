using Newtonsoft.Json;
using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Conteo;
using PruebaTecnicaF2X.Model.Recaudo;
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

        public async Task Procesar()
        {
            await recaudosRepository.LimpiarData();


            string token = await Login();
            DateTime i = new DateTime(2021, 8, 1);
            while (i < new DateTime(2021, 10, 6))
            {
                Consultar(token, i.ToString("yyyy-MM-dd"));
                i = i.AddDays(1);
            }
        }
        private async Task<string> Login()
        {
            return await conexionApiAdapter.Conectar(Constants.USUARIO, Constants.CLAVE);
        }

        private async Task<bool> Consultar(string token, string fecha)
        {

            try
            {
                var result = await conexionApiAdapter.Consumir(Constants.APICONTEO, token, fecha);
                List<ConteoVehiculos> conteoVehiculos = JsonConvert.DeserializeObject<List<ConteoVehiculos>>(result.Content);
                result = await conexionApiAdapter.Consumir(Constants.APIRECAUDO, token, fecha);
                List<RecaudoVehiculo> recaudoVehiculo = JsonConvert.DeserializeObject<List<RecaudoVehiculo>>(result.Content);


                return await Guardar(conteoVehiculos, recaudoVehiculo);

                //List<Recaudos> lRecaudos = (from _conteoVehiculos in conteoVehiculos 
                //                            join _recaudoVehiculo in recaudoVehiculo on
                //                             new
                //                             {
                //                                 _conteoVehiculos.Estacion,
                //                                 _conteoVehiculos.Hora,
                //                                 _conteoVehiculos.Categoria,
                //                                 _conteoVehiculos.Sentido
                //                             } equals new
                //                             {
                //                                 _recaudoVehiculo.Estacion,
                //                                 _recaudoVehiculo.Hora,
                //                                 _recaudoVehiculo.Categoria,
                //                                 _recaudoVehiculo.Sentido
                //                             }
                //                            select new Recaudos()
                //                            {
                //                                Estacion = _conteoVehiculos.Estacion,
                //                                Hora =_conteoVehiculos.Hora,
                //                                Categoria = _conteoVehiculos.Categoria,
                //                                Sentido = _conteoVehiculos.Sentido,
                //                                Cantidad= _conteoVehiculos.Cantidad,
                //                                ValorTabulado= _recaudoVehiculo.ValorTabulado
                //                            }).ToList();
                //return "";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        private async Task<bool> Guardar(List<ConteoVehiculos> conteo, List<RecaudoVehiculo> recaudo)
        {
            bool result = await recaudosRepository.IngresarDatosRecaudo(recaudo);
            result = await recaudosRepository.IngresarDatosConteo(conteo);
            return result;
        }


    }
}
