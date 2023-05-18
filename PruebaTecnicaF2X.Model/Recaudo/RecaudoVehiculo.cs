using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Model.Recaudo
{
    public class RecaudoVehiculo
    {
        public string Estacion { get; set; }

        public string Sentido { get; set; }

        public int Hora { get; set; }

        public string Categoria { get; set; }

        public double ValorTabulado { get; set; }
    }
}
