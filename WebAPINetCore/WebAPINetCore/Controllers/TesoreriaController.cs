﻿using Microsoft.AspNetCore.Identity;
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
        public ActionResult<IEnumerable<bool>> AbrirPuerta()
        {
            var respuesta = Tesoservice.abrirpuerta();
            return Ok(true);
        }

        [HttpGet("ApagarPC")]
        public ActionResult<IEnumerable<bool>> ApagarPC()
        {
            apagarPC apagar = new apagarPC("/s", DateTime.Now);
            var respuesta = apagar.Shut_Down();
            return Ok(respuesta);
        }

        [HttpGet("ReiniciarPC")]
        public ActionResult<IEnumerable<bool>> ReiniciarPC()
        {
            apagarPC apagar = new apagarPC("/r", DateTime.Now);
            var respuesta = apagar.Shut_Down();
            return Ok(respuesta);
        }

        //Saldo  de maquina y base de datos
        [HttpGet("ObtenerSaldosMaquina")]
        public ActionResult<IEnumerable<SaldoGaveta>> ObtenerSaldosMaquina()
        {
            Tesoservice.SaldoMaquinaTrans();
            return Ok(Globals.SaldosMaquina);
        }

        [HttpGet("ObtenerSaldos")]
        public ActionResult<IEnumerable<SaldoGaveta>> ObtenerSaldos()
        {
            Tesoservice.SaldoTransaccion();
            return Ok(Globals.Saldos);
        }

        // Realizar cierre Z y consulta
        [HttpPost("RealizarCierreZ")]
        public ActionResult<IEnumerable<DatosCierreZ>> RealizarCierreZ([FromBody] int mantisa)
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
        public ActionResult<IEnumerable<ReportesCierre>> ObtenerReportebyID([FromBody] UserToken user)
        {
            Tesoservice.ObtenerReportebyID(user);
            return Ok(Globals.DatosCierre);
        }


        // Vaciado de Gavetas
        [HttpPost("VaciarGaveta")]
        public ActionResult<IEnumerable<string>> VaciarGaveta([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.VaciarGaveta(gavs.Gavo, gavs.GavD, gavs.Tipo);
            return Ok(respuesta);
        }

        [HttpPost("Estadovaciado")]
        public ActionResult<IEnumerable<string>> Estadovaciado([FromBody] GavsRetiro gavs)
        {
            var respuesta = Tesoservice.Estadovaciado(gavs.Gavo, gavs.Tipo);
            return Ok(respuesta);
        }

        // Cargar de dinero Dinero 
        [HttpGet("Iniciocarga")]
        public ActionResult<IEnumerable<string>> Iniciocarga()
        {
            var resultado = Tesoservice.Iniciocarga();
            return Ok(resultado);
        }

        [HttpGet("FinalizarCarga")]
        public ActionResult<IEnumerable<string>> FinalizarCarga()
        {
            var resultado = Tesoservice.FinalizarCarga();
            return Ok(resultado);
        }

        [HttpGet("EstadoCarga")]
        public ActionResult<IEnumerable<string>> EstadoCarga()
        {
            var resultado = Tesoservice.EstadoCarga();
            return Ok(resultado);
        }


        [HttpPost("CargarMoneda")]
        public ActionResult<IEnumerable<string>> CargarMoneda([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.CargarMoneda(MonIngresadas);
            return Ok(respuesta);
        }

        // adm Dispositivos
        [HttpPost("InsertarDisp")]
        public ActionResult<IEnumerable<string>> InsertarDisp([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.InsertarDisp(MonIngresadas.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("RetirararDisp")]
        public ActionResult<IEnumerable<string>> RetirararDisp([FromBody] GavMR MonIngresadas)
        {
            var respuesta = Tesoservice.RetirararDisp(MonIngresadas.Idgav);
            return Ok(respuesta);
        }
        // Retirar gavetas
        [HttpPost("AgregarGav")]
        public ActionResult<IEnumerable<string>> AgregarGav([FromBody] GavReq gav)
        {
            var respuesta = Tesoservice.AgregarGav(gav);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavB")]
        public ActionResult<IEnumerable<string>> RetirarGavB([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavB(tipogav.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("RetirarGavM")]
        public ActionResult<IEnumerable<string>> RetirarGavM([FromBody] GavReq tipogav)
        {
            var respuesta = Tesoservice.RetirarGavM(tipogav.Idgav);
            return Ok(respuesta);
        }

        [HttpPost("DiscrepanciaBillete")]
        public ActionResult<IEnumerable<string>> DiscrepanciaBillete([FromBody] Discrepancia Gavs)
        {
            var respuesta = Tesoservice.DiscrepanciaB(Gavs);
            return Ok(respuesta);
        }

        [HttpPost("DiscrepanciaMoneda")]
        public ActionResult<IEnumerable<string>> DiscrepanciaMoneda([FromBody] Discrepancia Gavs)
        {
            var respuesta = Tesoservice.DiscrepanciaM(Gavs);
            return Ok(respuesta);
        }

        // Impresiones de Ticketcs 
        [HttpPost("ImprimirCierreZ")]
        public ActionResult<IEnumerable<bool>> ImprimirCierreZ([FromBody] DatosCierreZ Cierre)
        {
            var respuesta = Tesoservice.ImprecionCierreZAsync(Cierre);
            return Ok(respuesta);
        }

        [HttpPost("ImprimirCargaDinero")]
        public ActionResult<IEnumerable<bool>> ImprimirCargaDinero([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsync(Carga);
            return Ok(respuesta);
        }

        [HttpPost("ImpresionReporteCierrez")]
        public ActionResult<IEnumerable<bool>> ImpresionReporteCierrez([FromBody] UserToken user)
        {
            var respuesta = Tesoservice.ImpresionReporteCierrez(user);
            return Ok(respuesta);
        }

        [HttpPost("ImprimirGavAntesAhora")]
        public ActionResult<IEnumerable<bool>> ImprimirGavAntesAhora([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprimirGavAntesAhora(Carga, "Vaciado Gaveta de Vueltos");
            return Ok(respuesta);
        }

        [HttpPost("ImprimirGavAntesAhoraR")]
        public ActionResult<IEnumerable<bool>> ImprimirGavAntesAhoraR([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsyncR(Carga, "Vaciado Alcancias");
            return Ok(respuesta);
        }

        [HttpPost("ImpGavAntesAhoraD")]
        public ActionResult<IEnumerable<bool>> ImpGavAntesAhoraD([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprimirGavAntesAhora(Carga, "Retiro Disp Vueltos");
            return Ok(true);
        }


        [HttpPost("ImpGavAntesAhoraDReciclaje")]
        public ActionResult<IEnumerable<bool>> ImpGavAntesAhoraDReciclaje([FromBody] CargaDinero Carga)
        {
            var respuesta = Tesoservice.ImprecionComprobantePagoAsyncR(Carga, "Retiro Disp Alcancia");
            return Ok(true);
        }

    }
}
