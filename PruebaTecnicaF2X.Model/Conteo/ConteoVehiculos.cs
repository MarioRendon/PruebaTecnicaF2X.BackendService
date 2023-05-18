using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.Model.Conteo
{
    public class ConteoVehiculos
    {
        public string Estacion { get; set; }

        public string Sentido { get; set; }

        public int Hora { get; set; }

        public string Categoria { get; set; }

        public double Cantidad { get; set; }
    }
}
