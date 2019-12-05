using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;
using WebAPINetCore.PipeServer;
using System.Timers;

namespace WebAPINetCore.Services
{
    public class PagoService
    {
        static object thisLock = new object();
        System.Timers.Timer EsperarMonedas = new System.Timers.Timer() { AutoReset = false };
        System.Timers.Timer EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
        private EstadoPago montoapagar = new EstadoPago();
        private EstadoVuelto montodeVuelto = new EstadoVuelto();

        public  bool InicioPago()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Ini_Agregar_dinero, Globals.data);
            var vuelta =  Globals.Servicio2.SendMessage(ServicioPago.Comandos.Ini_Agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2.Resultado.Data[0].Contains("OK"))
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
                return false;
            }
        }

        public  bool FinalizarPago()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, Globals.data);
            var vuelta =  Globals.Servicio2.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2.Resultado.Data[0].Contains("OK"))
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
                return false;
            }
        }

        public EstadoPagoResp EstadoDelPAgo(EstadoPago PagoInfo)
        {
            lock (thisLock)
            {
                EstadoPagoResp estadopago = new EstadoPagoResp();
                Globals.data = new List<string>();
                Globals.data.Add("");
                Globals.Servicio2Pago = new PipeClient2();
                Globals.Servicio2Pago.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Cons_dinero_ingre, Globals.data);
                var vuelta = Globals.Servicio2Pago.SendMessage(ServicioPago.Comandos.Cons_dinero_ingre);
                if (vuelta)
                {
                    try
                    {
                        var DineroIngresado = int.Parse(Globals.Servicio2Pago.Resultado.Data[0]);
                        PagoInfo.DineroIngresado = DineroIngresado;
                        PagoInfo.DineroFaltante = PagoInfo.MontoAPagar - DineroIngresado;
                        if (PagoInfo.DineroFaltante < 0)
                        {
                            Globals.data = new List<string>();
                            var vueltoadar = PagoInfo.DineroFaltante * -1;
                            montodeVuelto.VueltoTotal = vueltoadar;
                            Globals.data.Add(vueltoadar.ToString());
                            Globals.Servicio2Vuelto = new PipeClient2();
                            Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                            var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                            if (DarVuelto)
                            {
                                if (Globals.Servicio2Vuelto.Resultado.Data[0].Contains("OK"))
                                {
                                    estadopago.data = PagoInfo;
                                    estadopago.Status = true;
                                    return estadopago;
                                }
                                else
                                {
                                    estadopago.data = PagoInfo;
                                    estadopago.Status = false;
                                    return estadopago;
                                }
                            }
                            else {
                                estadopago.data = PagoInfo;
                                estadopago.Status = false;
                                return estadopago;
                            }
                           
                        }
                        else if (PagoInfo.DineroFaltante == 0)
                        {
                            var finalizar = FinalizarPago();
                            if (finalizar == true)
                            {
                                EsperarMonedas = new System.Timers.Timer() { AutoReset = false };
                                EsperarMonedas.Elapsed += new ElapsedEventHandler(Timer_EstadoVueltoMonedas);
                                EsperarMonedas.AutoReset = false;
                                EsperarMonedas.Interval = 1000;
                                EsperarMonedas.Enabled = true;
                                EsperarMonedas.Start();
                                montoapagar = PagoInfo;
                                estadopago.data = PagoInfo;
                                estadopago.Status = true;
                                return estadopago;
                            }
                            else
                            {
                                estadopago.data = PagoInfo;
                                estadopago.Status = false;
                                return estadopago;
                            }
                        }
                        estadopago.data = PagoInfo;
                        estadopago.Status = true;
                        return estadopago;
                    }
                    catch (Exception ex)
                    {
                        estadopago.Status = false;
                        return estadopago;
                    }
                }

                else
                {
                    estadopago.Status = false;
                    return estadopago;
                }
            }
        }


        public EstadoVueltoResp EstadoDelVuelto(EstadoVuelto VueltoInfo)
        {
            lock (thisLock)
            {
                EstadoVueltoResp estavuelto = new EstadoVueltoResp();
                Globals.data = new List<string>();
                Globals.data.Add("");
                Globals.Servicio2Vuelto = new PipeClient2();
                Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.EstadoVuelto, Globals.data);
                var vuelta = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.EstadoVuelto);
                if (vuelta)
                {
                    try
                    {
                        
                        var VueltoEntregado = int.Parse(Globals.Servicio2Vuelto.Resultado.Data[0]);
                        VueltoInfo.DineroRegresado = VueltoEntregado;
                        VueltoInfo.DineroFaltante = VueltoInfo.VueltoTotal - VueltoEntregado;
                        if (Globals.Servicio2Vuelto.Resultado.Data[1] == "OK")
                        {
                            VueltoInfo.VueltoFinalizado = true;
                            var finalizar = FinalizarPago();
                            if (finalizar == true)
                            {
                                estavuelto.Status = true;
                            }
                            else
                            {
                                estavuelto.Status = false;
                            }
                        }
                        else
                        {
                            VueltoInfo.VueltoFinalizado = false;
                            estavuelto.Status = true;
                        }
                        estavuelto.data = VueltoInfo;
                        return estavuelto;
                    }
                    catch (Exception)
                    {
                        estavuelto.Status = false;
                        return estavuelto;
                    }
                }

                else
                {
                    estavuelto.Status = false;
                    return estavuelto;
                }
            }
        }

        protected void Timer_EstadoVueltoMonedas(object sender, ElapsedEventArgs e)
        {
            EsperarMonedas.Enabled = false;
            EsperarMonedas.Stop();

            try
            {
                
                    EstadoPagoResp estadopago = new EstadoPagoResp();
                    estadopago = EstadoDelPAgo(montoapagar);
                    if (estadopago.data.DineroFaltante < 0 )
                    {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        Globals.data = new List<string>();
                        Globals.data.Add(montodeVuelto.VueltoTotal.ToString());
                        Globals.Servicio2Vuelto = new PipeClient2();
                        Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                        var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                        if (DarVuelto)
                        {
                            EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
                            EsperarVueltoMonedas.Elapsed += new ElapsedEventHandler(Timer_VueltoMonedas);
                            EsperarVueltoMonedas.AutoReset = false;
                            EsperarVueltoMonedas.Interval = 2000;
                            EsperarVueltoMonedas.Enabled = true;
                            EsperarVueltoMonedas.Start();
                        }
                     }

                    }
                

            }
            catch (Exception ex)
            {
               //
            }
        }

        protected void Timer_VueltoMonedas(object sender, ElapsedEventArgs e)
        {
            EsperarMonedas.Enabled = false;
            EsperarMonedas.Stop();

            try
            {
                EstadoVueltoResp estadovuelto = new EstadoVueltoResp();
                estadovuelto = EstadoDelVuelto(montodeVuelto);
                if (estadovuelto.data.VueltoFinalizado == false)
                {
                    EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
                    EsperarVueltoMonedas.Elapsed += new ElapsedEventHandler(Timer_VueltoMonedas);
                    EsperarVueltoMonedas.AutoReset = false;
                    EsperarVueltoMonedas.Interval = 2000;
                    EsperarVueltoMonedas.Enabled = true;
                    EsperarVueltoMonedas.Start();
                }
                else {
                    var finalizar = FinalizarPago();
                    if (finalizar == true)
                    {
                        EsperarMonedas.Enabled = false;
                        EsperarMonedas.Stop();
                    }
                    else
                    {
                        montodeVuelto = new EstadoVuelto();
                        montoapagar = new EstadoPago();
                        EsperarMonedas.Enabled = false;
                        EsperarMonedas.Stop();
                    }
                }
            }
            catch (Exception ex )
            {

                //throw;
            }
        }
     }
}
