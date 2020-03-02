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

        // Administrar maquina Abrir apagar reiniciar

        [HttpGet("AbrirPuerta")]
        public ActionResult<IEnumerable<string>> AbrirPuerta()
        {
           var respuesta = Tesoservice.abrirpuerta();
            return Ok(true);
        }

        [HttpGet("ApagarPC")]
        public ActionResult<IEnumerable<string>> ApagarPC()
        {
            apagarPC apagar = new apagarPC("/s", DateTime.Now);
            var respuesta = apagar.Shut_Down();
            return Ok(respuesta);
        }

        [HttpGet("ReiniciarPC")]
        public ActionResult<IEnumerable<string>> ReiniciarPC()
        {
            apagarPC apagar = new apagarPC("/r", DateTime.Now);
            var respuesta = apagar.Shut_Down();
            return Ok(respuesta);
        }

         //Saldo  de maquina y base de datos
        [HttpGet("ObtenerSaldosMaquina")]
        public ActionResult<IEnumerable<string>> ObtenerSaldosMaquina()
        {
            Tesoservice.SaldoMaquinaTrans();
            return Ok(Globals.SaldosMaquina);
        }

        [HttpGet("ObtenerSaldos")]
        public ActionResult<IEnumerable<string>> ObtenerSaldos()
        {
            Tesoservice.SaldoTransaccion();
            return Ok(Globals.Saldos);
        }

        // Realizar cierre Z y consulta
        [HttpPost("RealizarCierreZ")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RealizarCierreZ([FromBody] int mantisa)
        {
            Tesoservice.RealizarCierreZ(mantisa);
            return Ok(Globals.Cierrez);
        }

        [HttpPost("ObtenerReporteCierreZ")]
        public ActionResult<IEnumerable<IDReportesCierre>> ObtenerReporteCierreZ([FromBody] int mantisa)
        {
            Tesoservice.ObtenerReporteCierreZ(mantisa);
            return Ok(Globals.fechasIDs);
        }

        [HttpPost("ObtenerReportebyID")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ObtenerReportebyID([FromBody] UserToken user)
        {
            Tesoservice.ObtenerReportebyID(user);
            return Ok(Globals.DatosCierre);
        }


        // Vaciado de Gavetas
        [HttpPost("VaciarGaveta")]
        public ActionResult<IEnumerable<EstadoPagoResp>> VaciarGaveta([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.VaciarGaveta(gavs.Gavo, gavs.GavD,gavs.Tipo);
            return Ok(respuesta);
        }

        [HttpPost("Estadovaciado")]
        public ActionResult<IEnumerable<EstadoPagoResp>> Estadovaciado([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.Estadovaciado(gavs.Gavo,gavs.Tipo);
            return Ok(respuesta);
        }

        // Cargar de dinero Dinero 
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

        // adm Dispositivos
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
        // Retirar gavetas
        [HttpPost("AgregarGav")]
        public ActionResult<IEnumerable<EstadoPagoResp>> AgregarGav([FromBody] GavReq gav)
        {
            var respuesta = Tesoservice.AgregarGav(gav);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavB")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RetirarGavB([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavB(tipogav.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavM")]
        public ActionResult<IEnumerable<EstadoPagoResp>> RetirarGavM([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavM(tipogav.Idgav);
            return Ok(respuesta);
        }

        // Impresiones de Ticketcs 
        [HttpPost("ImprimirCierreZ")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImprimirCierreZ([FromBody] DatosCierreZ Cierre)
        {
            var respuesta = Tesoservice.ImprecionCierreZAsync(Cierre);
            return Ok(respuesta);
        }

        [HttpPost("ImprimirCargaDinero")] 
        public ActionResult<IEnumerable<EstadoPagoResp>> ImprimirCargaDinero([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsync(Carga);
            return Ok(respuesta);
        }

        [HttpPost("ImpresionReporteCierrez")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImpresionReporteCierrez([FromBody] UserToken user)
        {
            var respuesta = Tesoservice.ImpresionReporteCierrez(user);
            return Ok(respuesta);
        }

        [HttpPost("ImprimirGavAntesAhora")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImprimirGavAntesAhora([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprimirGavAntesAhora(Carga, "Vaciado Gaveta de Vueltos");
            return Ok(respuesta);
        }

        [HttpPost("ImprimirGavAntesAhoraR")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImprimirGavAntesAhoraR([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsyncR(Carga, "Vaciado Alcancias");
            return Ok(respuesta);
        }

        [HttpPost("ImpGavAntesAhoraD")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImpGavAntesAhoraD([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprimirGavAntesAhora(Carga, "Retiro Disp Vueltos");
            return Ok(true);
        }


        [HttpPost("ImpGavAntesAhoraDReciclaje")]
        public ActionResult<IEnumerable<EstadoPagoResp>> ImpGavAntesAhoraDReciclaje([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsyncR(Carga, "Retiro Disp Alcancia");
            return Ok(true);
        }















        }
    }
