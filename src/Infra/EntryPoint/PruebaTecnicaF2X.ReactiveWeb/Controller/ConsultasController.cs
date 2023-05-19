using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.UseCase.Consultas;
using PruebaTecnicaF2X.UseCase.ProcesarInformacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnicaF2X.ReactiveWeb.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController:ControllerBase
    {
        IConsultaUseCase consultaUseCase;
        
        public ConsultasController(IConsultaUseCase consultaUseCase)
        {
            this.consultaUseCase = consultaUseCase;
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultaResponse))]
        [ProducesResponseType(400)]
        [HttpGet("ConsultaRecaudos")]
        public async Task<ActionResult> ConsultaInformacion(string? categoria,string? sentido,int? hora,string? estacion,string? registro)
        {
            try
            {
                ConsultaRequest consultaRequest = new()
                {
                    Categoria = categoria,
                    Estacion = estacion,
                    Hora = hora,
                    Sentido = sentido,
                    Registro = registro
                };
                ConsultaResponse result = await consultaUseCase.ConsultarInformacion(consultaRequest);
                return StatusCode(StatusCodes.Status200OK,JsonConvert.SerializeObject(result));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,  ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultaResponse))]
        [ProducesResponseType(400)]
        [HttpGet("Reporte")]
        public async Task<ActionResult> Reporte()
        {
            try
            {
                InformeResponse result = await consultaUseCase.GenerarInforme();
                return StatusCode(StatusCodes.Status200OK, result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}
