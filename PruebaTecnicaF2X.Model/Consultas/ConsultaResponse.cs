using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Model.Consultas
{
    public class ConsultaResponse
    {
        public List<Data> Data { get; set; }
    }


    public class Data {

        public string Fecha { get; set; }

        public string Estacion { get; set; }

        public decimal Conteo { get; set; }

        public decimal Recaudo { get; set; }
    }
}
