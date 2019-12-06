using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;
using WebAPINetCore.PipeServer;
namespace WebAPINetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {

        PagoService pagoservice = new PagoService();
        public PagoController()
        {

        }

        // GET api/Pago/IniciarPago
        [HttpGet("IniciarPago")]
        public  ActionResult<IEnumerable<bool>> IniciarPago()
        {
            var resultado =  pagoservice.InicioPago();
            return Ok(resultado);
        }


        // Post api/Pago/DineroIngresado
        [HttpPost("DineroIngresado")]
        public  ActionResult<IEnumerable<EstadoPagoResp>> EstadoDePago([FromBody] EstadoPago PagoInfo)
        {
            var resultado =  pagoservice.EstadoDelPAgo(PagoInfo);
            EstadoPagoResp estado = new EstadoPagoResp();
            estado = resultado;
            return Ok(estado);
        }

        [HttpPost("VueltoRegresado")]
        public ActionResult<IEnumerable<EstadoVueltoResp>> EstadoDeVuelto([FromBody] EstadoVuelto VueltoInfo)
        {
            var resultado =  pagoservice.EstadoDelVuelto(VueltoInfo);
            EstadoVueltoResp estado = new EstadoVueltoResp();
            estado = resultado;
            return Ok(estado);
        }


        // GET api/Pago/FinalizarPago
        [HttpGet("FinalizarPago")]
        public  ActionResult<IEnumerable<string>> FinalizarPago()
        {
            var resultado =  pagoservice.FinalizarPago();
            return Ok(resultado);
        }

        // GET api/Pago/FinalizarPago
        [HttpGet("Cancelarpago")]
        public ActionResult<IEnumerable<bool>> CancelarPago()
        {
            var resultado = pagoservice.CancelarPago();
            return Ok(resultado);
        }

        [HttpGet("EstadoCancelacionPago")]
        public ActionResult<IEnumerable<CancelarPago>> EstadoCancelacionPago()
        {
            var resultado = pagoservice.EstadoDeCancelacion();
            return Ok(resultado);
        }


    }
}
