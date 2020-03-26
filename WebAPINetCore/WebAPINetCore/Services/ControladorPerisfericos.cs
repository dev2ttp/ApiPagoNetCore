using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class ControladorPerisfericos
    {
        public bool InicioPayout()
        {
            Globals.log.Debug("Inicio de Payout");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServInicioPayou = new PipeClient2();
            Globals.ServInicioPayou.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.InicioPayout, Globals.data);
            var respuesta = Globals.ServInicioPayou.SendMessage(ServicioPago.Comandos.InicioPayout);
            if (respuesta)
            {
                if (Globals.ServInicioPayou.Resultado.Data[0] == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Globals.log.Error("Error al activar la payout Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return false;
            }
        }
        public bool InicioHopper()
        {
            Globals.log.Debug("Inicio de Hopper");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServInicioHopper = new PipeClient2();
            Globals.ServInicioHopper.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.InicioHopper, Globals.data);
            var respuesta = Globals.ServInicioHopper.SendMessage(ServicioPago.Comandos.InicioHopper);
            if (respuesta)
            {
                if (Globals.ServInicioHopper.Resultado.Data[0] == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Globals.log.Error("Error al activar la hopper Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return false;
            }
        }
        public bool FinPayout()
        {
            Globals.log.Debug("deshabilitar de payout");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServFinPayou = new PipeClient2();
            Globals.ServFinPayou.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.FinPayout, Globals.data);
            var respuesta = Globals.ServFinPayou.SendMessage(ServicioPago.Comandos.FinPayout);
            if (respuesta)
            {
                if (Globals.ServFinPayou.Resultado.Data[0] == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Globals.log.Error("Error al deshabilitar  payout Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return false;
            }
        }
        public bool FinHopper()
        {
            Globals.log.Debug("deshabilitar de hopper");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServFinHopper = new PipeClient2();
            Globals.ServFinHopper.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.FinHopper, Globals.data);
            var respuesta = Globals.ServFinHopper.SendMessage(ServicioPago.Comandos.FinHopper);
            if (respuesta)
            {
                if (Globals.ServFinHopper.Resultado.Data[0] == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Globals.log.Error("Error al deshabilitar  hopper Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return false;
            }
        }
        public string GetAllLevelsNota()
        {
            Globals.log.Debug("nivel de payout");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServNivelNota = new PipeClient2();
            Globals.ServNivelNota.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.GetAllLevelsNota, Globals.data);
            var respuesta = Globals.ServNivelNota.SendMessage(ServicioPago.Comandos.GetAllLevelsNota);
            if (respuesta)
            {
                return Globals.ServNivelNota.Resultado.Data[0];
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return "false";
            }
        }
        public string GetAllLevelsCoin()
        {
            Globals.log.Debug("nivel de hopper");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.ServNivelMoneda = new PipeClient2();
            Globals.ServNivelMoneda.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.GetAllLevelsCoin, Globals.data);
            var respuesta = Globals.ServNivelMoneda.SendMessage(ServicioPago.Comandos.GetAllLevelsCoin);
            if (respuesta)
            {
                return Globals.ServNivelMoneda.Resultado.Data[0];
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return "false";
            }
        }
    }
}
