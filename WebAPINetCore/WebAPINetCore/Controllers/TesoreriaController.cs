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

    }
}
