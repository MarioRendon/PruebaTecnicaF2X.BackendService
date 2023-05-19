using PruebaTecnicaF2X.Model.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.UseCase.Consultas
{
    public interface IConsultaUseCase
    {
        Task<ConsultaResponse> ConsultarInformacion(ConsultaRequest consultaRequest);
        Task<InformeResponse> GenerarInforme();
    }
}
