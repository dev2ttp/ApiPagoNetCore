using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class ControladorPerisfericos
    {
        public bool IniciarPago()
        {
            Globals.log.Debug("Inicio de Pago");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServInicioPago = new PipeClient2();
            ServInicioPago.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Ini_Agregar_dinero, data);
            var respuesta = ServInicioPago.SendMessage(ServicioPago.Comandos.Ini_Agregar_dinero);
            if (respuesta)
            {
                if (ServInicioPago.Resultado.Data[0].Contains("OK"))
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
                Globals.log.Error("Error al Empezaar el pago Transaccion: " + Globals.IDTransaccion + " Error:" + ServInicioPago.Resultado.CodigoError + " Respuesta completa " + ServInicioPago._Resp);
                return false;
            }
        }
        public bool FinalizarPago()
        {
            Globals.log.Debug("Finalizar de Pago");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServFiPago = new PipeClient2();
            ServFiPago.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, data);
            var respuesta = ServFiPago.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
            if (respuesta)
            {
                if (ServFiPago.Resultado.Data[0].Contains("OK"))
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
                Globals.log.Error("Error al Empezaar el pago Transaccion: " + Globals.IDTransaccion + " Error:" + ServFiPago.Resultado.CodigoError + " Respuesta completa " + ServFiPago._Resp);
                return false;
            }
        }
        public bool InicioPayout()
        {
            Globals.log.Debug("Inicio de Payout");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServInicioPayou = new PipeClient2();
            ServInicioPayou.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.InicioPayout, data);
            var respuesta = ServInicioPayou.SendMessage(ServicioPago.Comandos.InicioPayout);
            if (respuesta)
            {
                if (ServInicioPayou.Resultado.Data[0] == "True")
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
                Globals.log.Error("Error al activar la payout Transaccion: " + Globals.IDTransaccion + " Error:" + ServInicioPayou.Resultado.CodigoError + " Respuesta completa " + ServInicioPayou._Resp);
                return false;
            }
        }
        public bool InicioHopper()
        {
            Globals.log.Debug("Inicio de Hopper");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServInicioHopper = new PipeClient2();
            ServInicioHopper.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.InicioHopper,data);
            var respuesta = ServInicioHopper.SendMessage(ServicioPago.Comandos.InicioHopper);
            if (respuesta)
            {
                if (ServInicioHopper.Resultado.Data[0] == "True")
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
                Globals.log.Error("Error al activar la hopper Transaccion: " + Globals.IDTransaccion + " Error:" + ServInicioHopper.Resultado.CodigoError + " Respuesta completa " + ServInicioHopper._Resp);
                return false;
            }
        }
        public bool FinPayout()
        {
            Globals.log.Debug("deshabilitar de payout");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServFinPayout = new PipeClient2();
            ServFinPayout.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.FinPayout, data);
            var respuesta = ServFinPayout.SendMessage(ServicioPago.Comandos.FinPayout);
            if (respuesta)
            {
                if (ServFinPayout.Resultado.Data[0] == "True")
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
                Globals.log.Error("Error al deshabilitar  payout Transaccion: " + Globals.IDTransaccion + " Error:" + ServFinPayout.Resultado.CodigoError + " Respuesta completa " + ServFinPayout._Resp);
                return false;
            }
        }
        public bool FinHopper()
        {
            Globals.log.Debug("deshabilitar de hopper");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServFinHopper = new PipeClient2();
            ServFinHopper.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.FinHopper, data);
            var respuesta = ServFinHopper.SendMessage(ServicioPago.Comandos.FinHopper);
            if (respuesta)
            {
                if (ServFinHopper.Resultado.Data[0] == "True")
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
                Globals.log.Error("Error al deshabilitar  hopper Transaccion: " + Globals.IDTransaccion + " Error:" + ServFinHopper.Resultado.CodigoError + " Respuesta completa " + ServFinHopper._Resp);
                return false;
            }
        }
        public bool FloatByDenomination()
        {
            Globals.log.Debug("Inicio del float");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 Float = new PipeClient2();
            Float.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Float, data);
            var respuesta = Float.SendMessage(ServicioPago.Comandos.Float);
            if (respuesta)
            {
                return true;
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + Float.Resultado.CodigoError + " Respuesta completa " + Float._Resp);
                return false;
            }
        }
        public bool EstadoSalud()
        {
            Globals.log.Debug("Inicio de Estado de Salud");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 EstadoSalud = new PipeClient2();
            EstadoSalud.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.EstadoSalud, data);
            var respuesta = EstadoSalud.SendMessage(ServicioPago.Comandos.EstadoSalud);
            if (respuesta)
            {
                return respuesta;
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + EstadoSalud.Resultado.CodigoError + " Respuesta completa " + EstadoSalud._Resp);
                return false;
            }
        }
        public bool DetenerVuelto()
        {
            Globals.log.Debug("Inicio de Estado de Salud");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 detenerVuelto = new PipeClient2();
            detenerVuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DetenerVuelto, data);
            var respuesta = detenerVuelto.SendMessage(ServicioPago.Comandos.DetenerVuelto);
            if (respuesta)
            {
                return respuesta;
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + detenerVuelto.Resultado.CodigoError + " Respuesta completa " + detenerVuelto._Resp);
                return false;
            }
        }
        public string GetAllLevelsNota()
        {
            Globals.log.Debug("nivel de payout");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServLevlNota = new PipeClient2();
            ServLevlNota.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.GetAllLevelsNota, data);
            var respuesta = ServLevlNota.SendMessage(ServicioPago.Comandos.GetAllLevelsNota);
            if (respuesta)
            {
                return ServLevlNota.Resultado.Data[0];
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + ServLevlNota.Resultado.CodigoError + " Respuesta completa " + ServLevlNota._Resp);
                return "false";
            }
        }
        public string GetAllLevelsCoin()
        {
            Globals.log.Debug("nivel de hopper");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServNivelMoneda = new PipeClient2();
            ServNivelMoneda.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.GetAllLevelsCoin, data);
            var respuesta = ServNivelMoneda.SendMessage(ServicioPago.Comandos.GetAllLevelsCoin);
            if (respuesta)
            {
                return ServNivelMoneda.Resultado.Data[0];
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + ServNivelMoneda.Resultado.CodigoError + " Respuesta completa " + ServNivelMoneda._Resp);
                return "false";
            }
        }
        public string DinerIngresado()
        {
            Globals.log.Debug("Dinero Ingreado");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServDinerIngresado = new PipeClient2();
            ServDinerIngresado.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Cons_dinero_ingre, data);
            var respuesta = ServDinerIngresado.SendMessage(ServicioPago.Comandos.Cons_dinero_ingre);
            if (respuesta)
            {
                Globals.log.Debug("Dinero Ingresado " + ServDinerIngresado.Resultado.Data[0]);
                return ServDinerIngresado.Resultado.Data[0];
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + ServDinerIngresado.Resultado.CodigoError + " Respuesta completa " + ServDinerIngresado._Resp);
                return "false";
            }
        }
        public string VueltoRegresado()
        {
            Globals.log.Debug("vuelto regresado");
            List<string> data = new List<string>();
            data.Add("");
            PipeClient2 ServVueltoRegresado = new PipeClient2();
            ServVueltoRegresado.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.EstadoVuelto, data);
            var respuesta = ServVueltoRegresado.SendMessage(ServicioPago.Comandos.EstadoVuelto);
            if (respuesta)
            {
                Globals.log.Debug("vuelto regresado " + ServVueltoRegresado.Resultado.Data[0]);
                if (ServVueltoRegresado.Resultado.Data[1] == "OK")
                {
                    return ServVueltoRegresado.Resultado.Data[0] + "OK";
                }
                else
                {
                    return ServVueltoRegresado.Resultado.Data[0];
                }
                
            }
            else
            {
                Globals.log.Error("Error al obtener nivel  hopper Transaccion: /* + Globals.IDTransaccion + */ Error:" + ServVueltoRegresado.Resultado.CodigoError + " Respuesta completa " + ServVueltoRegresado._Resp);
                return "false";
            }
        }
        public bool DarVuelto(float vuelto)
        {
            Globals.log.Debug("Dar Vuelto");
            List<string> data = new List<string>();
            data.Add(vuelto.ToString());
            PipeClient2 ServDarVuelto = new PipeClient2();
            ServDarVuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, data);
            var respuesta = ServDarVuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
            if (respuesta)
            {
                if (ServDarVuelto.Resultado.Data[0] == "True")
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
                Globals.log.Error("Error al Dar Vuelto Transaccion: " + Globals.IDTransaccion + " Error:" + ServDarVuelto.Resultado.CodigoError + " Respuesta completa " + ServDarVuelto._Resp);
                return false;
            }
        }
        

    }
}
