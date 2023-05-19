using Moq;
using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.Constantes;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;
using PruebaTecnicaF2X.UseCase.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.UseCase.Test.Consultas
{
    public class ConsultaUseCaseTest
    {
        private readonly Mock<IConexionApiAdapter> conexionApiAdapter = new();
        private readonly Mock<IRecaudosRepository> recaudosRepository = new();
        private readonly IConsultaUseCase consultaUseCase;
        public ConsultaUseCaseTest()
        {
            this.consultaUseCase = new ConsultaUseCase(
               conexionApiAdapter.Object,
               recaudosRepository.Object
           );
        }

        [Fact]
        public async Task ConsultarInformacion_ok()
        {
            ConsultaRequest consultaRequest = new();
            List<Recaudos> lRecaudos = new();
            lRecaudos.Add(new Recaudos() { Cantidad=1,Categoria="VI",Estacion= "ANDES",Hora=1,Sentido= "BOG-CHI",ValorTabulado= 1255800 });

            this.recaudosRepository.Setup(x => x.ConsultaRecaudos(It.IsAny<ConsultaRequest>())).ReturnsAsync(lRecaudos);

            ConsultaResponse consultaResponse= await this.consultaUseCase.ConsultarInformacion(consultaRequest);

            Assert.NotNull(consultaResponse);
        }

        [Fact]
        public async Task ConsultarInformacion_null()
        {
            ConsultaRequest consultaRequest = new();
            List<Recaudos> lRecaudos =new();

            this.recaudosRepository.Setup(x => x.ConsultaRecaudos(It.IsAny<ConsultaRequest>())).ReturnsAsync(lRecaudos);

            ConsultaResponse consultaResponse = await this.consultaUseCase.ConsultarInformacion(consultaRequest);

            Assert.True(consultaResponse.Datos.Count==0);


        }


        [Fact]
        public async Task ConsultarGeneraInforme_ok()
        {
            ConsultaRequest consultaRequest = new();
            List<Recaudos> lRecaudos = new();
            lRecaudos.Add(new Recaudos() { Cantidad = 1, Categoria = "VI", Estacion = "ANDES", Hora = 1, Sentido = "BOG-CHI", ValorTabulado = 1255800 });

            this.recaudosRepository.Setup(x => x.ConsultaRecaudos(It.IsAny<ConsultaRequest>())).ReturnsAsync(lRecaudos);

            InformeResponse informeResponse = await this.consultaUseCase.GenerarInforme();

            Assert.NotNull(informeResponse);
        }

        [Fact]
        public async Task ConsultarGeneraInforme_vacio()
        {
            ConsultaRequest consultaRequest = new();
            List<Recaudos> lRecaudos = new();

            this.recaudosRepository.Setup(x => x.ConsultaRecaudos(It.IsAny<ConsultaRequest>())).ReturnsAsync(lRecaudos);

            InformeResponse informeResponse = await this.consultaUseCase.GenerarInforme();

            Assert.True(informeResponse.reporte.Equals(Constants.RESPUESTANOGENERACIONREPORTE));
        }
    }
}
