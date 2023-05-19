using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ProcesarController: ControllerBase
    {
        IProcesarUseCase procesarUseCase;
        public ProcesarController( IProcesarUseCase procesarUseCase)
        {
            this.procesarUseCase = procesarUseCase;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultaResponse))]
        [ProducesResponseType(400)]
        [HttpPost()]
        public async Task<ActionResult> ProcesarInformacion()
        {
            try
            {
                await procesarUseCase.Procesar();
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}
