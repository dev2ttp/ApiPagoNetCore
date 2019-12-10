using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;
using WebAPINetCore.PipeServer;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Collections;

namespace WebAPINetCore.Services
{
    public class PagoService
    {
        static object thisLock = new object();
        System.Timers.Timer EsperarMonedas = new System.Timers.Timer() { AutoReset = false };
        System.Timers.Timer EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
        System.Timers.Timer TimerCancerlarPago = new System.Timers.Timer() { AutoReset = false };
        private EstadoPago montoapagar = new EstadoPago();
        private static readonly HttpClient client = new HttpClient();
        private EstadoVuelto montodeVuelto = new EstadoVuelto();
        TransaccionService transaccion = new TransaccionService();

        public bool InicioPago()
        {
            Globals.log.Debug("Inicio de Pago");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2Inicio = new PipeClient2();
            Globals.Servicio2Inicio.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Ini_Agregar_dinero, Globals.data);
            var vuelta = Globals.Servicio2Inicio.SendMessage(ServicioPago.Comandos.Ini_Agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2Inicio.Resultado.Data[0].Contains("OK"))
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

        public bool FinalizarPago()
        {
            Globals.log.Debug("Finalizar Pago");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, Globals.data);
            var vuelta = Globals.Servicio2.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
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

        public bool CancelarPago()
        {
            lock (thisLock)
            {
                Globals.log.Debug("Cancerlar de Pago");
                Globals.data = new List<string>();
                Globals.data.Add("");
                Globals.Servicio2Cancelar = new PipeClient2();
                Globals.Servicio2Cancelar.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, Globals.data);
                var vuelta = Globals.Servicio2Cancelar.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
                if (vuelta)
                {
                    if (Globals.Servicio2Cancelar.Resultado.Data[0].Contains("OK"))
                    {
                        TimerCancerlarPago = new System.Timers.Timer() { AutoReset = false };
                        TimerCancerlarPago.Elapsed += new ElapsedEventHandler(Timer_EstadoCancelarPago);
                        TimerCancerlarPago.AutoReset = false;
                        TimerCancerlarPago.Interval = 6000;
                        TimerCancerlarPago.Enabled = true;
                        TimerCancerlarPago.Start();
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
        }

        public async Task<bool> ImprimirComprobanteAsync(String Documento)
        {
            var url = string.Format(Globals._config["Urls:Impresion"]);
            var json = JsonConvert.SerializeObject(Documento);
            var request = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, request))
            {
                var content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return true;
        }

        public async void ArmarDocuemntoPagoCompeltoAsync(EstadoPago PagoInfo) {
            try
            {
                StreamReader objReader = new StreamReader("./Documentos/Comprobante_Pago_completado.txt");
                string sLine = "";
                string comprobante = "";

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                    {
                        string oldvalue = sLine;
                        comprobante += oldvalue + "\r\n";
                    }
                }
                objReader.Close();

                DateTime fechaHoy = DateTime.Now;
                comprobante = comprobante.Replace("dd/mm/aaaa hh:mm:ss PM", fechaHoy.ToString());
                comprobante = comprobante.Replace("XXXAPAGAR", "$ " + Globals.Pago.MontoAPagar.ToString());
                comprobante = comprobante.Replace("XXXPAGADO", "$ " + Globals.Pago.DineroIngresado.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);

                var respuesta = await ImprimirComprobanteAsync(comprobante);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async void ArmarDocuemntoPagoConVueltoAsync()
        {
            try
            {
                StreamReader objReader = new StreamReader("./Documentos/Comprobante_Pago_completado_vuelto.txt");
                string sLine = "";
                string comprobante = "";

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                    {
                        string oldvalue = sLine;
                        comprobante += oldvalue + "\r\n";
                    }
                }
                objReader.Close();

                DateTime fechaHoy = DateTime.Now;
                comprobante = comprobante.Replace("dd/mm/aaaa hh:mm:ss PM", fechaHoy.ToString());
                comprobante = comprobante.Replace("XXXAPAGAR","$ " +  Globals.Pago.MontoAPagar.ToString());
                comprobante = comprobante.Replace("XXXPAGADO", "$ " + Globals.Pago.DineroIngresado.ToString());
                comprobante = comprobante.Replace("XXXVAENTREGAR", "$ " + Globals.Vuelto.DineroRegresado.ToString());
                comprobante = comprobante.Replace("XXXVENTREGADO", "$ " + Globals.Vuelto.VueltoTotal.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);

                var respuesta = await ImprimirComprobanteAsync(comprobante);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public CancelarPago EstadoDeCancelacion()
        {
            var estado = Globals.EstadodeCancelacion;
            if (Globals.EstadodeCancelacion.CancelacionCompleta == true)
            {
                Globals.EstadodeCancelacion = new CancelarPago();
            }
            return estado;
        }

        public EstadoPagoResp EstadoDelPAgo(EstadoPago PagoInfo)
        {
            Globals.ComprobanteImpresoContador++;
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
                        Globals.Pago = PagoInfo;
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
                                    estadopago.PagoStatus = false;
                                    return estadopago;
                                }
                                else
                                {
                                    estadopago.data = PagoInfo;
                                    estadopago.Status = false;
                                    estadopago.PagoStatus = false;
                                    return estadopago;
                                }
                            }
                            else
                            {
                                estadopago.data = PagoInfo;
                                estadopago.Status = false;
                                estadopago.PagoStatus = false;
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
                                if (Globals.ComprobanteImpreso == false && Globals.ComprobanteImpresoContador > 5)
                                {
                                    ArmarDocuemntoPagoCompeltoAsync(PagoInfo);
                                    
                                    Globals.ComprobanteImpreso = true;
                                }
                                estadopago.PagoStatus = true;
                                return estadopago;
                            }
                            else
                            {
                                estadopago.data = PagoInfo;
                                estadopago.Status = false;
                                estadopago.PagoStatus = false;
                                return estadopago;
                            }
                        }
                        estadopago.data = PagoInfo;
                        estadopago.Status = true;
                        estadopago.PagoStatus = false;
                        return estadopago;
                    }
                    catch (Exception ex)
                    {
                        estadopago.Status = false;
                        estadopago.PagoStatus = false;
                        return estadopago;
                    }
                }

                else
                {
                    estadopago.Status = false;
                    estadopago.PagoStatus = false;
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
                            Globals.Vuelto = VueltoInfo;
                            if (Globals.EstadodeCancelacion.EntregandoVuelto )
                            {
                                Globals.Vuelto.VueltoTotal = VueltoInfo.DineroRegresado;
                            }
                            var finalizar = FinalizarPago();
                            if (finalizar == true)
                            {
                                estavuelto.Status = true;
                                estavuelto.PagoStatus = true;
                                if (Globals.ComprobanteImpresoVuelto == false)
                                {
                                    Globals.ComprobanteImpresoVuelto = true;
                                    ArmarDocuemntoPagoConVueltoAsync();
                                }
                                transaccion.FinTransaccion();
                                Globals.EstadodeCancelacion = new CancelarPago();
                            }
                            else
                            {
                                estavuelto.PagoStatus = false;
                                estavuelto.Status = false;
                            }
                        }
                        else
                        {
                            VueltoInfo.VueltoFinalizado = false;
                            Globals.Vuelto = VueltoInfo;
                            estavuelto.Status = true;
                            estavuelto.PagoStatus = false;
                        }
                        estavuelto.data = VueltoInfo;
                        Globals.Vuelto = VueltoInfo;
                        estavuelto.PagoStatus = false;
                        return estavuelto;
                    }
                    catch (Exception)
                    {
                        estavuelto.Status = false;
                        estavuelto.PagoStatus = false;
                        return estavuelto;
                    }
                }

                else
                {
                    estavuelto.Status = false;
                    estavuelto.PagoStatus = false;
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
                if (estadopago.data.DineroFaltante < 0)
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
                else {
                    transaccion.FinTransaccion();
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
                //var auxvuelto = Globals.Vuelto;
                estadovuelto = EstadoDelVuelto(montodeVuelto);
                if (estadovuelto.data.VueltoFinalizado == false)
                {
                    //Globals.Vuelto = auxvuelto;
                    Globals.EstadodeCancelacion.CancelacionCompleta = false;
                    Globals.EstadodeCancelacion.EntregandoVuelto = true;
                    EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
                    EsperarVueltoMonedas.Elapsed += new ElapsedEventHandler(Timer_VueltoMonedas);
                    EsperarVueltoMonedas.AutoReset = false;
                    EsperarVueltoMonedas.Interval = 2000;
                    EsperarVueltoMonedas.Enabled = true;
                    EsperarVueltoMonedas.Start();
                }
                else
                {
                    var finalizar = FinalizarPago();
                    if (finalizar == true)
                    {
                        //Globals.Vuelto = auxvuelto;
                        Globals.EstadodeCancelacion.CancelacionCompleta = true;
                        Globals.EstadodeCancelacion.EntregandoVuelto = false;
                        EsperarVueltoMonedas.Enabled = false;
                        EsperarVueltoMonedas.Stop();
                        EsperarMonedas.Enabled = false;
                        EsperarMonedas.Stop();
                    }
                    else
                    {
                        montodeVuelto = new EstadoVuelto();
                        montoapagar = new EstadoPago();
                        EsperarMonedas.Enabled = false;
                        EsperarMonedas.Stop();
                        EsperarVueltoMonedas.Enabled = false;
                        EsperarVueltoMonedas.Stop();
                    }
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
        }

        protected void Timer_EstadoCancelarPago(object sender, ElapsedEventArgs e)
        {
            TimerCancerlarPago.Enabled = false;
            TimerCancerlarPago.Stop();

            try
            {

                EstadoPagoResp estadopago = new EstadoPagoResp();
                montoapagar.MontoAPagar = 999999;
                var auxPago = Globals.Pago;
                estadopago = EstadoDelPAgo(montoapagar);
                if (estadopago.data.DineroIngresado > 0)
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        Globals.Pago = auxPago;
                        Globals.data = new List<string>();
                        Globals.data.Add(estadopago.data.DineroIngresado.ToString());
                        Globals.Servicio2Vuelto = new PipeClient2();
                        Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                        var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                        if (DarVuelto)
                        {
                            Globals.Vuelto.VueltoTotal = estadopago.data.DineroIngresado;
                            Globals.Vuelto.DineroRegresado = estadopago.data.DineroIngresado;
                            EsperarVueltoMonedas = new System.Timers.Timer() { AutoReset = false };
                            EsperarVueltoMonedas.Elapsed += new ElapsedEventHandler(Timer_VueltoMonedas);
                            EsperarVueltoMonedas.AutoReset = false;
                            EsperarVueltoMonedas.Interval = 2000;
                            EsperarVueltoMonedas.Enabled = true;
                            EsperarVueltoMonedas.Start();
                        }
                    }

                }
                else {
                    Globals.EstadodeCancelacion.CancelacionCompleta = true;
                    Globals.EstadodeCancelacion.EntregandoVuelto = false;
                }


            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}
