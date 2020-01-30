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
    public class TesoreriaController : ControllerBase
    {
        TesoreriaService Tesoservice = new TesoreriaService();
        TransaccionService transaccion = new TransaccionService();
        PermitirVueltoService vueltopuede = new PermitirVueltoService();
        private readonly IConfiguration _configuration;

        public TesoreriaController(IConfiguration configuration)
        {
            _configuration = configuration;
            Globals._config = _configuration;
        }

        // GET api/Pago/FinalizarPago
        [HttpGet("ObtenerSaldos")]
        public ActionResult<IEnumerable<string>> ObtenerSaldos()
        {
            Tesoservice.SaldoTransaccion();
            return Ok(Globals.Saldos);
        }

        // Post api/Tesoreria/DineroIngresado
        [HttpPost("RealizarCierreZ")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RealizarCierreZ([FromBody] int mantisa)
        {
            Tesoservice.RealizarCierreZ(mantisa);
            return Ok(Globals.Cierrez);
        }

        [HttpGet("ObtenerReporteCierreZ")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ObtenerReporteCierreZ()
        {
            Tesoservice.ObtenerReporteCierreZ();
            return Ok(Globals.fechasIDs);
        }

        [HttpPost("ObtenerReportebyID")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ObtenerReportebyID([FromBody] int idz)
        {
            Tesoservice.ObtenerReportebyID(idz.ToString());
            return Ok(Globals.DatosCierre);
        }

        [HttpPost("VaciarGaveta")]
        public ActionResult<IEnumerable<EstadoPagoResp>> VaciarGaveta([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.VaciarGaveta(gavs.Gavo, gavs.GavD);
            return Ok(respuesta);
        }

        [HttpPost("Estadovaciado")]
        public ActionResult<IEnumerable<EstadoPagoResp>> Estadovaciado([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.Estadovaciado(gavs.Gavo);
            return Ok(respuesta);
        }

        [HttpGet("Iniciocarga")]
        public ActionResult<IEnumerable<EstadoPagoResp>> Iniciocarga()
        {
            var resultado = Tesoservice.Iniciocarga();
            return Ok(resultado);
        }

        [HttpGet("FinalizarCarga")]
        public ActionResult<IEnumerable<EstadoPagoResp>> FinalizarCarga()
        {
            var resultado = Tesoservice.FinalizarCarga();
            return Ok(resultado);
        }

        [HttpGet("EstadoCarga")]
        public ActionResult<IEnumerable<EstadoPagoResp>> EstadoCarga()
        {
            var resultado = Tesoservice.EstadoCarga();
            return Ok(resultado);
        }


        [HttpPost("CargarMoneda")]
        public ActionResult<IEnumerable<EstadoPagoResp>> CargarMoneda([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.CargarMoneda(MonIngresadas);
            return Ok(respuesta);
        }

        [HttpPost("InsertarDisp")]
        public ActionResult<IEnumerable<EstadoPagoResp>> InsertarDisp([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.InsertarDisp(MonIngresadas.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("RetirararDisp")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RetirararDisp([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.RetirararDisp(MonIngresadas.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("AgregarGav")]
        public ActionResult<IEnumerable<EstadoPagoResp>> AgregarGav([FromBody] GavReq gav)
        {
            var respuesta = Tesoservice.AgregarGav(gav);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavB")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RetirarGavB([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavB(tipogav.Tipo);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavM")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RetirarGavM([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavM(tipogav.Tipo);
            return Ok(respuesta);
        }

        

            

            









    }
}
