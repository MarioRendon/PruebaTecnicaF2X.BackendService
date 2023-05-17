using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaF2X.Model.Consultas;
using PruebaTecnicaF2X.UseCase.Consultas;
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
        [HttpGet()]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                ConsultaResponse result = await consultaUseCase.ConsultarInformacion();
                return StatusCode(StatusCodes.Status200OK, result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,  ex.Message);
            }
        }
    }
}
