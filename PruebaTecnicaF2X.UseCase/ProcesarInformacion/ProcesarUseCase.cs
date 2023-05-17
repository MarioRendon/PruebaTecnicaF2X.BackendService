using Newtonsoft.Json;
using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.Model.Conteo;
using PruebaTecnicaF2X.Model.Login;
using PruebaTecnicaF2X.Model.Recaudos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.UseCase.ProcesarInformacion
{
    public class ProcesarUseCase: IProcesarUseCase
    {

        private readonly IConexionApiAdapter conexionApiAdapter;


        public ProcesarUseCase(IConexionApiAdapter conexionApiAdapter)
        { 
            this.conexionApiAdapter = conexionApiAdapter;
        }

        public async Task<string> Procesar()
        {
            string token =await  Login();
            DateTime i = new DateTime(2021,8,1);
            while ( i < new DateTime(2021, 10, 6))
            {
                Consultar(token, i.ToString("yyyy-MM-dd"));
                i.AddDays(1);
            }
             
           return "";  

        }
        private async Task<string> Login()
        {

            return await conexionApiAdapter.Conectar(Constants.USUARIO, Constants.CLAVE);

        }

        private async Task<string> Consultar(string token,string fecha) {

            var result = await conexionApiAdapter.Consumir(Constants.APICONTEO, token, fecha);
            ConteoVehiculos conteoVehiculos = JsonConvert.DeserializeObject<ConteoVehiculos>(result.Content);
            result = await conexionApiAdapter.Consumir(Constants.APIRECAUDO, token, fecha);
            RecaudoVehiculo recaudoVehiculo = JsonConvert.DeserializeObject<RecaudoVehiculo>(result.Content);
            

            return "";
        }

        private async Task<string> Guardar(ConteoVehiculos conteo, RecaudoVehiculo recaudo)
        {

            return "";
        }


    }
}
