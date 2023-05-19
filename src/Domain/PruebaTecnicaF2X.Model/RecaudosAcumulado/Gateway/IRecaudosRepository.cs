using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.Model.Conteo;
using PruebaTecnicaF2X.Model.Recaudo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway
{
    public interface IRecaudosRepository
    {
        Task<bool> IngresarDatosRecaudo(List<Recaudos> recaudos);

        Task<bool> IngresarDatosConteo(List<ConteoVehiculos> conteos);

        Task<List<Recaudos>> ConsultaRecaudos(ConsultaRequest consultaRequest);

        Task<bool> LimpiarData();
    }
}
