using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Login;
using PruebaTecnicaF2X.ObjectsUtils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Http.Api
{
    public class ConexionApiAdapter : IConexionApiAdapter
    {
        private readonly IOptionsMonitor<ConfiguratorAppSettings> appSettings;

        public ConexionApiAdapter(IOptionsMonitor<ConfiguratorAppSettings> appSettings)
        {
            this.appSettings = appSettings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public async Task<string> Conectar(string usuario, string pass)
        {
            try
            {


                var restClient = new RestClient($"{appSettings.CurrentValue.UrlApis}{Constants.APILOGIN}");
                var request = new RestRequest("", method: Method.Post);
                request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("userName", usuario);
                //request.AddParameter("password", pass);

                request.AddParameter("application/json", "{\"userName\":\"user\",\"password\":\"1234\"}", ParameterType.RequestBody);

                RestResponse response = restClient.Execute(request);
                return JsonConvert.DeserializeObject<Login>(response.Content).Token;
            }
            catch (Exception ex)
            {
                string error=ex.Message;
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="token"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public async Task<RestResponse> Consumir(string api, string token, string fecha)
        {
            var restClient = new RestClient($"{appSettings.CurrentValue.UrlApis}{api}/{fecha}");
            var request = new RestRequest("", method: Method.Get);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Bearer " + token);

            RestResponse response = restClient.Execute(request);
            
            return response;

        }
    }
}
