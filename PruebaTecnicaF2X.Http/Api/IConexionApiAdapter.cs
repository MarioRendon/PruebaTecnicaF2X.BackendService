using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Http.Api
{
    public interface IConexionApiAdapter
    {
        Task<string> Conectar(string usuario, string pass);
        Task<RestResponse> Consumir(string api, string token, string fecha);
    }
}
