using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.SqlServer.Recaudo
{
    public class RecaudosEntity
    {
        public string Estacion { get; set; }

        public string Sentido { get; set; }

        public string hora { get; set; }

        public string Categoria { get; set; }

        public double ValorTabulado { get; set; }

        public double Cantidad { get; set; }
    }
}
