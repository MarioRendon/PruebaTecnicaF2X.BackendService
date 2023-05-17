using PruebaTecnicaF2X.Model.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.UseCase.Consultas
{
    public class ConsultaUseCase: IConsultaUseCase
    {
        public ConsultaUseCase() { }

        public async Task<ConsultaResponse> ConsultarInformacion()
        {
            List<Data> lRegistros = new List<Data>();
            lRegistros.Add(new Data { Estacion="1",Conteo=3,Recaudo=10000,Fecha="2023-05-16" });

            return new ConsultaResponse() { Data = lRegistros };
        
        }
    }
}
