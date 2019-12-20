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
        PermitirVueltoService vueltopuede = new PermitirVueltoService();
        private readonly IConfiguration _configuration;

        public PagoController(IConfiguration configuration)
        {
            _configuration = configuration;
            Globals._config = _configuration;
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
            Globals.TimersVueltoCancel = false;
            Globals.PagoFinalizado = false;
            Globals.VueltoUnaVEz = false;
            Globals.DandoVuelto = false;
            Globals.HayVuelto = true;
            Globals.PagoCompleto = false;
            Globals.VueltoPermitido = false;
            var resultado =  pagoservice.InicioPago();
            var mensaje = pagoservice.ConfigurarStatus();
            InicioOperacionService Status = new InicioOperacionService();
            Status.MensajeAmostrar = mensaje;
            if (Globals.EstadoDeSaludMaquina.Contains("00"))
            {
                Status.StatusMaquina = true;
            }
            else {
                if (Globals.NivelBloqueo)
                {
                    pagoservice.FinalizarPago();
                }
                Status.StatusMaquina = false;
            }
            Status.NivelBloqueo = Globals.NivelBloqueo;
            Status.Status = resultado;
            return Ok(Status);
        }


        // Post api/Pago/DineroIngresado
        [HttpPost("DineroIngresado")]
        public  ActionResult<IEnumerable<EstadoPagoResp>> EstadoDePago([FromBody] EstadoPago PagoInfo)
        {
           
            if (Globals.VueltoPermitido == false)
            {
                var VueltoPosible = vueltopuede.CalcularVueltoPosible(PagoInfo.MontoAPagar);


                if (VueltoPosible == true)
                {
                    Globals.VueltoPermitido = true;
                }
                else
                {
                    EstadoPagoResp vueltonoposible = new EstadoPagoResp();
                    vueltonoposible.PagoStatus = false;
                    vueltonoposible.Status = false;
                    return Ok(vueltonoposible);
                }
            }
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
