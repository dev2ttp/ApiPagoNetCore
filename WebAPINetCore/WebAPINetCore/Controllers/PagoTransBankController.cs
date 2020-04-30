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
using WebAPINetCore.Pago;

namespace WebAPINetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoTransBankController : ControllerBase
    {
        [HttpPost]
        [Route("tbk", Order = 1, Name = "pagoTbk")]
        [ActionName("pagoTbk")]
        public ActionResult<IEnumerable<ResponsePagoTbk>>  PagoTbk([FromBody] DetallePagoCliReq req)
        {
            Movimiento service = new Movimiento();
            var response = service.RealizarPagoTbk(req);
            return Ok(response);
        }

        [HttpGet]
        [Route("cierre", Order = 1, Name = "cierreTbk")]
        [ActionName("cierreTbk")]
        public ActionResult<IEnumerable<ResponseTbk>>  CierreTbk()
        {
            Movimiento service = new Movimiento();
            var response = service.cierreTbk();
            return Ok(response);
        }

        [HttpGet]
        [Route("inicializacion", Order = 1, Name = "inicializacionTbk")]
        [ActionName("inicializacionTbk")]
        public ActionResult<IEnumerable<ResponseTbk>>  InicializacionTbk()
        {
            Movimiento service = new Movimiento();
            var response = service.inicializacionTbk();
            return Ok(response);
        }

        [HttpGet]
        [Route("cargaLLaves", Order = 1, Name = "cargaLlavesTbk")]
        [ActionName("cargaLlavesTbk")]
        public ActionResult<IEnumerable<ResponseTbk>>  CargaLlavesTbk()
        {
            Movimiento service = new Movimiento();
            var response = service.cargaLlavesTbk();
            return Ok(response);
        }

        [HttpGet]
        [Route("pooling", Order = 1, Name = "verificarEstado")]
        [ActionName("verificarEstado")]
        public ActionResult<IEnumerable<Boolean>> PoolingTbk()
        {
            Movimiento service = new Movimiento();
            string msg = string.Empty;
            if (Globals._config["Impresora:Tipo"].Equals("TUP"))
            {
                if (!service.EstadoTransbank(Pago.ServicioPago.Comandos.Pooling))
                    return Ok(false);

                if (!service.EstadoPrinterTup())
                    return Ok(false);

                return Ok(true);
            }
            else
            {
                if (!service.EstadoTransbank(Pago.ServicioPago.Comandos.Pooling))
                    return Ok(false);

                if (!service.EstadoPrinter())
                    return Ok(false);

                return Ok(true);
            }
           
        }

        [HttpGet]
        [Route("anular", Order = 1, Name = "anularPagoTbk")]
        [ActionName("anularPagoTbk")]
        public ActionResult<IEnumerable<ResponseTbk>>  AnularPagoTbk()
        {
            Movimiento service = new Movimiento();
            var response = service.anularPagoTbk();
            return Ok(response);
        }

        [HttpGet]
        [Route("ultima", Order = 1, Name = "ultimaVentaTbk")]
        [ActionName("ultimaVentaTbk")]
        public ActionResult<IEnumerable<ResponseTbk>>  UltimaVentaTbk()
        {
            Movimiento service = new Movimiento();
            var response = service.ultimaVentaTbk();
            return Ok(response);
        }
    }
}
