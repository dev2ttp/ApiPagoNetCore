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


        public PagoController()
        {

        }

        // GET api/Pago/IniciarPago
        [HttpGet("IniciarPago")]
        public async Task<ActionResult<IEnumerable<string>>> IniciarPago()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Ini_Agregar_dinero, Globals.data);
            var vuelta = await Globals.Servicio2.SendMessage(ServicioPago.Comandos.Ini_Agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2.Resultado.Data[0].Contains("OK"))
                {
                    return Ok("Maquinas activadas correctamente");
                }
                else {
                    return Ok("Error al activar las maquinas Cod Error:" + Globals.Servicio2.Resultado.CodigoError);
                }

            }
            else {
                return Ok("Error al activar las maquinas Cod Error:" + Globals.Servicio2.Resultado.CodigoError);
            }
        }


        // Post api/Pago/DineroIngresado
        [HttpPost("DineroIngresado")]
        public async Task<ActionResult<IEnumerable<EstadoPago>>> EstadoDePago([FromBody] EstadoPago PagoInfo)
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Cons_dinero_ingre, Globals.data);
            var vuelta = await Globals.Servicio2.SendMessage(ServicioPago.Comandos.Cons_dinero_ingre);
            if (vuelta)
            {
                try
                {
                    var DineroIngresado = int.Parse(Globals.Servicio2.Resultado.Data[0]);
                    PagoInfo.DineroIngresado = DineroIngresado;
                    PagoInfo.DineroFaltante = PagoInfo.MontoAPagar -  DineroIngresado;
                    if (PagoInfo.DineroFaltante < 0)
                    {
                        PagoInfo.DineroFaltante = 0;
                    }
                    return Ok(PagoInfo);
                }
                catch (Exception)
                {
                    return Ok("Error al devolver el dinero COD:" + Globals.Servicio2.Resultado.CodigoError);
                }
            }

            else
            {
                return Ok("Error al devolver el dinero COD:" + Globals.Servicio2.Resultado.CodigoError);
            }
        }


        // GET api/Pago/FinalizarPago
        [HttpGet("FinalizarPago")]
        public async Task<ActionResult<IEnumerable<string>>> FinalizarPago()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, Globals.data);
            var vuelta = await Globals.Servicio2.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2.Resultado.Data[0].Contains("OK"))
                {
                    return Ok("Operacionde Pago Finalizada Correctamente");
                }
                else
                {
                    return Ok("Error al devolver el dinero COD:" + Globals.Servicio2.Resultado.CodigoError);
                }

            }
            else
            {
                return Ok("Error al devolver el dinero COD:" + Globals.Servicio2.Resultado.CodigoError);
            }
        }



    }
}
