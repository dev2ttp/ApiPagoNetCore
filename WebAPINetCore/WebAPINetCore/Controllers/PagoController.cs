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
        TransaccionService transaccion= new TransaccionService();
        private readonly IConfiguration _configuration;

        public PagoController(IConfiguration configuration)
        {
            _configuration = configuration;
            Globals._config = _configuration;
            //var algo = _configuration["Urls:Impresion"];
            //var algo2 = _configuration["JWT:key"];
        }

        // GET api/Pago/IniciarPago
        [HttpGet("IniciarPago")]
        public  ActionResult<IEnumerable<bool>> IniciarPago()
        {
            transaccion.InicioTransaccion();
            Globals.ComprobanteImpresoContador = 0;
            Globals.ComprobanteImpreso = false;
            Globals.ComprobanteImpresoVuelto = false;
            Globals.Vuelto = new EstadoVuelto();
            Globals.Pago = new EstadoPago();
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

        [HttpPost("Hacerimpresion")]
        public ActionResult<IEnumerable<EstadoPagoResp>> Hacerimpresion([FromBody] EstadoPago PagoInfo)
        {
            pagoservice.ArmarDocuemntoPagoCompeltoAsync(PagoInfo);
            return Ok();
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
