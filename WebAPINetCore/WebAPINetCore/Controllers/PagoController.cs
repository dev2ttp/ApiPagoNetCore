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
        TransaccionService transaccion = new TransaccionService();
        PermitirVueltoService vueltopuede = new PermitirVueltoService();
        private readonly IConfiguration _configuration;

        public PagoController(IConfiguration configuration)
        {
            _configuration = configuration;
            Globals._config = _configuration;
        }

        // GET api/Pago/IniciarPago
        [HttpPost("IniciarPago")]
        public ActionResult<IEnumerable<InicioOperacionService>> IniciarPago([FromBody] EstadoPago PagoInfo)
        {
            Globals.ImpresoraMontoEntregado = 0;
            Globals.ImpresoraMontoIngresado = 0;
            Globals.ImpresoraMontoPagar = 0;
            Globals.ImpresoraMontoVueltoEntregar = 0;
            Globals.ImpresoraMontoPagar = PagoInfo.MontoAPagar;

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
            Globals.VueltosSinIniciar = 0;
            Globals.DineroIngresadoSolicitado = false;
            Globals.dineroIngresado = "0";
            InicioOperacionService Status = new InicioOperacionService();
            if (Globals.VueltoPermitido == false)
            {
                var VueltoPosible = vueltopuede.CalcularVueltoPosible(PagoInfo.MontoAPagar);

                if (VueltoPosible == true)
                {
                    Globals.VueltoPermitido = true;
                }
                else
                {
                    Status = new InicioOperacionService();
                    Status.Status = false;
                    pagoservice.ConfigurarStatus();
                    Status.StatusMaquina = Globals.SaludMaquina;
                    Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
                    Status.BloqueoTransbank = Globals.BloqueoTransbank;
                    return Ok(Status);                  
                }
            }
            var resultado = pagoservice.InicioPago();
            pagoservice.ConfigurarStatus();
            Status = new InicioOperacionService();
            Status.Status = resultado;
            pagoservice.ConfigurarStatus();
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }


        // Post api/Pago/DineroIngresado
        [HttpPost("DineroIngresado")]
        public ActionResult<IEnumerable<EstadoPagoResp>> EstadoDePago([FromBody] EstadoPago PagoInfo)
        {

            var resultado = pagoservice.EstadoDelPAgo(PagoInfo);
            EstadoPagoResp estado = new EstadoPagoResp();
            estado = resultado;
            pagoservice.ConfigurarStatus();
            estado.StatusMaquina = Globals.SaludMaquina;
            estado.BloqueoEfectivo = Globals.BloqueoEfectivo;
            estado.BloqueoTransbank = Globals.BloqueoTransbank;
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
            var resultado = pagoservice.EstadoDelVuelto(VueltoInfo);
            EstadoVueltoResp estado = new EstadoVueltoResp();
            estado = resultado;
            pagoservice.ConfigurarStatus();
            estado.StatusMaquina = Globals.SaludMaquina;
            estado.BloqueoEfectivo = Globals.BloqueoEfectivo;
            estado.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(estado);
        }


        // GET api/Pago/FinalizarPago
        [HttpGet("FinalizarPago")]
        public ActionResult<IEnumerable<string>> FinalizarPago()
        {
            var resultado = pagoservice.FinalizarPago();
            InicioOperacionService Status = new InicioOperacionService();
            pagoservice.ConfigurarStatus();
            Status.Status = resultado;
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }

        // GET api/Pago/FinalizarPago
        [HttpGet("Cancelarpago")]
        public ActionResult<IEnumerable<bool>> CancelarPago()
        {
            var resultado = pagoservice.CancelarPago();
            InicioOperacionService Status = new InicioOperacionService();
            pagoservice.ConfigurarStatus();
            Status.Status = resultado;
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }

        [HttpGet("EstadoCancelacionPago")]
        public ActionResult<IEnumerable<CancelarPago>> EstadoCancelacionPago()
        {
            var resultado = pagoservice.EstadoDeCancelacion();
            pagoservice.ConfigurarStatus();
            resultado.StatusMaquina = Globals.SaludMaquina;
            resultado.BloqueoEfectivo = Globals.BloqueoEfectivo;
            resultado.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(resultado);
        }
        // GET api/Pago/EstadoSalud
        [HttpGet("EstadoSalud")]
        public ActionResult<IEnumerable<bool>> EstadoSalud()
        {
            var resultado = pagoservice.EstadoSalud();
            InicioOperacionService Status = new InicioOperacionService();
            Status.Status = resultado;
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }
        // GET api/Pago/Detener vuelto
        [HttpGet("DetenerVuelto")]
        public ActionResult<IEnumerable<bool>> DetenerVuelto()
        {
            var resultado = pagoservice.DetenerVuelto();
            InicioOperacionService Status = new InicioOperacionService();
            Status.Status = resultado;
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }

        // GET api/Pago/Float
        [HttpGet("Float")]
        public ActionResult<IEnumerable<string>> Float()
        {
           var resultado =  pagoservice.FloatByDenomination();
            InicioOperacionService Status = new InicioOperacionService();
            Status.Status = resultado;
            Status.StatusMaquina = Globals.SaludMaquina;
            Status.BloqueoEfectivo = Globals.BloqueoEfectivo;
            Status.BloqueoTransbank = Globals.BloqueoTransbank;
            return Ok(Status);
        }


        // GET api/Pago/Float
        [HttpGet("estadoBloqueo")]
        public void EstadoBloqueo() {
            if (Globals.BloqueoEfectivo == true | Globals.BloqueoTransbank == true)
            {
                Globals.log.Info("Se ha producido una Alteracion en el estado de salud de la maquina: " + Globals.EstadoDeSaludMaquina);
            }
        }
    }
}
