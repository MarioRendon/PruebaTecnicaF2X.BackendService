using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Model.RecaudosAcumulado
{
    public class Recaudos
    {
        public string Estacion { get; set; }

        public string Sentido { get; set; }

        public Timer hora { get; set; }

        public string Categoria { get; set; }

        public double ValorTabulado { get; set; }

        public double Cantidad { get; set; }
    }
}
