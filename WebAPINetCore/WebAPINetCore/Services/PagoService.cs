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
        System.Timers.Timer TimerDarVuelto = new System.Timers.Timer() { AutoReset = false };
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
                    Globals.MaquinasActivadas = true;
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
                    Globals.MaquinasActivadas = false;
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
                        Globals.MaquinasActivadas = false;
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

        public async void ArmarDocuemntoPagoCompeltoAsync(EstadoPago PagoInfo)
        {
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
                comprobante = comprobante.Replace("XXXAPAGAR", "$ " + Globals.Pago.MontoAPagar.ToString());
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

        public EstadoPagoResp EstadoDelPAgo(EstadoPago PagoInfo)// se encarga de consultar el estado en que se encuentra el pago 
        {
            Globals.ComprobanteImpresoContador++;// utilizado para que no se tomen datos del pago anterior
            lock (thisLock)
            {
                bool volveraUno = false;
                EstadoPagoResp estadopago = new EstadoPagoResp();
                Globals.data = new List<string>();
                Globals.data.Add("");
                Globals.Servicio2Pago = new PipeClient2();
                Globals.Servicio2Pago.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Cons_dinero_ingre, Globals.data);
                var vuelta = Globals.Servicio2Pago.SendMessage(ServicioPago.Comandos.Cons_dinero_ingre);
                // llamar al servicio pipe para traer los datos

                if (vuelta)
                {
                    try
                    {
                        
                        var DineroIngresado = int.Parse(Globals.Servicio2Pago.Resultado.Data[0]);
                        PagoInfo.DineroIngresado = DineroIngresado;
                        PagoInfo.DineroFaltante = PagoInfo.MontoAPagar - DineroIngresado;
                        //Actualizar los datos de pago

                        if (PagoInfo.DineroIngresado == 999999)// si la llamada al servicio viene de un timer de vuelto
                        {
                            Globals.DandoVuelto = true;
                            volveraUno = true;
                        }
                        else {// Actualizar el estado de pago
                            Globals.Pago = PagoInfo;
                        }
                        
                        if (PagoInfo.DineroFaltante < 0)
                        {// si hay que dar vuelto
                            FinalizarPago();
                            Globals.RespaldoParaVuelto = PagoInfo;
                            
                            if (Globals.DandoVuelto == false)// si actualmente se estan realizand los procesor para dar vuelto
                            {
                                PagoInfo.DineroFaltante = 1;
                                
                            }
                            
                            
                            estadopago.data = PagoInfo;
                            estadopago.Status = true;
                            estadopago.PagoStatus = false;
                            // estado que se retornan al front

                            if (Globals.VueltoUnaVEz == false)// timer de dar vuelto que se ejecuta una sola vez 
                            {
                                TimerDarVuelto = new System.Timers.Timer() { AutoReset = false };
                                TimerDarVuelto.Elapsed += new ElapsedEventHandler(Timer_DarVuelto);
                                TimerDarVuelto.AutoReset = false;
                                TimerDarVuelto.Interval = 3000;
                                TimerDarVuelto.Enabled = true;
                                TimerDarVuelto.Start();
                            }
                            Globals.VueltoUnaVEz = true;

                            if (volveraUno == true )// se devuelven los flags de vuelto al estado original para  siguientes consultas 
                            {
                                Globals.DandoVuelto = false;
                            }
                            if (Globals.DandoVuelto == true)// obtener concretos de los vueltos a dar ( esto si si se entraga dinero extra mientras se cancela la operacion)
                            {
                                estadopago.data.DineroFaltante = Globals.Vuelto.VueltoTotal;
                            }
                            volveraUno = false;

                            return estadopago;
                        }
                        else if (PagoInfo.DineroFaltante == 0)// si el pago se realizo completamente
                        {
                            var finalizar = FinalizarPago();
                            if (finalizar == true)
                            {
                                if (Globals.TimersVueltoCancel == false)// para mandar a realizar una pause y luego verificar si se ingreso dinero extra y si es asi se devolverlo
                                {
                                    EsperarMonedas = new System.Timers.Timer() { AutoReset = false };
                                    EsperarMonedas.Elapsed += new ElapsedEventHandler(Timer_EstadoVueltoMonedas);
                                    EsperarMonedas.AutoReset = false;
                                    EsperarMonedas.Interval = 2000;
                                    EsperarMonedas.Enabled = true;
                                    EsperarMonedas.Start();
                                }

                                montoapagar = PagoInfo;
                                estadopago.data = PagoInfo;
                                estadopago.Status = true;
                                // datos usado para retornar 

                                if (Globals.ComprobanteImpreso == false && Globals.ComprobanteImpresoContador > 5)// para realizar la impresion del ticket el contador es pq las primeras veces puede traer datos  de un pago anterior
                                {
                                    ArmarDocuemntoPagoCompeltoAsync(PagoInfo);// armar datos de ticket

                                    Globals.ComprobanteImpreso = true;
                                }
                                estadopago.PagoStatus = true;

                                return estadopago;
                            }
                            else
                            {// si al enviar un estado de apagado de maquina esta no contesta correctamewnte
                                estadopago.data = PagoInfo;
                                estadopago.Status = false;
                                estadopago.PagoStatus = false;
                                return estadopago;
                            }
                        }// si no se ha completado un pago
                        estadopago.data = PagoInfo;
                        estadopago.Status = true;
                        estadopago.PagoStatus = false;
                        if (Globals.DandoVuelto == true)// para devolver el vuelto que se debe enviar al timer
                        {
                            estadopago.data.DineroFaltante = Globals.Vuelto.VueltoTotal*-1;
                        }
                        return estadopago;
                    }
                    catch (Exception ex)
                    {
                        estadopago.Status = false;
                        estadopago.PagoStatus = false;
                        return estadopago;
                    }
                }

                else // si no se enviaron los datos correctos dele stado de pago
                {
                    estadopago.Status = false;
                    estadopago.PagoStatus = false;
                    return estadopago;
                }
            }
        }

        public EstadoVueltoResp EstadoDelVuelto(EstadoVuelto VueltoInfo)// ver estado del vuelto a dar
        {
            lock (thisLock)
            {
                EstadoVueltoResp estavuelto = new EstadoVueltoResp();
                Globals.data = new List<string>();
                Globals.data.Add("");
                Globals.Servicio2Vuelto = new PipeClient2();
                Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.EstadoVuelto, Globals.data);
                var vuelta = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.EstadoVuelto);// enviar los datos al servicio y esperar el retorno de los mismos
                if (vuelta)
                {
                    try
                    {
                        var VueltoEntregado = int.Parse(Globals.Servicio2Vuelto.Resultado.Data[0]);
                        VueltoInfo.DineroRegresado = VueltoEntregado;
                        VueltoInfo.DineroFaltante = VueltoInfo.VueltoTotal - VueltoEntregado;
                        // actualizar los datos del vuelto 

                        if (Globals.Servicio2Vuelto.Resultado.Data[1] == "OK")// si es OK significa que el vuelto ya se completo
                        {
                            VueltoInfo.VueltoFinalizado = true;
                            Globals.Vuelto = VueltoInfo;// Guardar la variable del estado del vuelto globalmente

                            if (Globals.EstadodeCancelacion.EntregandoVuelto)// si la consulta viene por una cancelacion se devuelve el dinero total ingresado
                            {
                                Globals.Vuelto.VueltoTotal = VueltoInfo.DineroRegresado;
                            }
                            var finalizar = FinalizarPago();
                            if (finalizar == true)
                            {// si la maquina se apaag corectamente
                                estavuelto.Status = true;
                                estavuelto.PagoStatus = true;
                                if (Globals.ComprobanteImpresoVuelto == false)// imprimir el comprobante
                                {
                                    Globals.ComprobanteImpresoVuelto = true;
                                    ArmarDocuemntoPagoConVueltoAsync();
                                }
                                transaccion.FinTransaccion();// finalizar la transaccion actual
                                Globals.EstadodeCancelacion = new CancelarPago();
                                estavuelto.data = VueltoInfo;
                                Globals.Vuelto = VueltoInfo;
                                return estavuelto;
                            }
                            else
                            {// si la maquina no finaliza correctamente
                                estavuelto.PagoStatus = false;
                                estavuelto.Status = false;
                            }
                        }
                        else
                        {// si el vuelto aun no se ha terminado de dar
                            VueltoInfo.VueltoFinalizado = false;
                            Globals.Vuelto = VueltoInfo;
                            estavuelto.Status = true;
                            estavuelto.PagoStatus = false;
                        }
                        // si no se pudo comunicar correctamente io no hay vuelto
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
                }// si no se comunica correctamente con el servicio
                else
                {
                    estavuelto.Status = false;
                    estavuelto.PagoStatus = false;
                    return estavuelto;
                }
            }
        }

        protected void Timer_EstadoVueltoMonedas(object sender, ElapsedEventArgs e)// timer llamado cuando se finaliza una operacion completa o se cancela
        {
            EsperarMonedas.Enabled = false;
            EsperarMonedas.Stop();

            try
            {

                EstadoPagoResp estadopago = new EstadoPagoResp();
                estadopago = EstadoDelPAgo(montoapagar);
                if (estadopago.data.DineroFaltante < 0)// si despues de finalizar se ingreso dinero extra se llama el vuelto 
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        Globals.data = new List<string>();
                        Globals.data.Add(montodeVuelto.VueltoTotal.ToString());
                        Globals.Servicio2Vuelto = new PipeClient2();
                        Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                        var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                        if (DarVuelto)// si se empieza  a dar vuelto  se llama a un timer de consulta para que luego apague la maquina
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
                else
                {// se actualiza la transaxxion  y se cambia a que la cancelacion no ha comensado
                    Globals.TimersVueltoCancel = true;
                    transaccion.FinTransaccion();
                }



            }
            catch (Exception ex)
            {
                //
            }
        }

        protected void Timer_DarVuelto(object sender, ElapsedEventArgs e)// timer usado para dar un vuelto normal
        {
            TimerDarVuelto.Stop();
            TimerDarVuelto.Enabled = false;

            try
            {

                EstadoPagoResp estadopago = new EstadoPagoResp();
                Globals.RespaldoParaVuelto.DineroFaltante = 0;
                Globals.RespaldoParaVuelto.DineroIngresado = 999999;// monto enviado para hacer saber  a la funcion que la llamada se realiza de este timer( esto por cuestiones de llamadas asyncronas)
                estadopago = EstadoDelPAgo(Globals.RespaldoParaVuelto);// llamada para saber que monto actual de vuelto hay que realizar 
                if (estadopago.data.DineroFaltante < 0)
                {
                    Globals.Vuelto.VueltoTotal = estadopago.data.DineroFaltante * -1;
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {

                        estadopago.data.DineroFaltante *= -1;
                        while (Globals.MaquinasActivadas == false)// si por alguna razon la maquina se encuentra desactivada al realizar el pago
                        {
                            InicioPago();
                        }
                        Globals.data = new List<string>();
                        Globals.data.Add(estadopago.data.DineroFaltante.ToString());
                        Globals.Servicio2Vuelto = new PipeClient2();
                        Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                        var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                        if (DarVuelto)// si la maquina devuelve que esta dando vueltos
                        {
                            Globals.DandoVuelto = true;
                        }
                        else {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                            Globals.DandoVuelto = false;
                            Globals.VueltoUnaVEz = false;
                        }
                    }
                    else
                    {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                        Globals.DandoVuelto = false;
                        Globals.VueltoUnaVEz = false;
                    }
                }                
            }
            catch (Exception ex)
            {
                //
            }

        }

        protected void Timer_VueltoMonedas(object sender, ElapsedEventArgs e)// timer ingresado cuando se ingresan monedas o billetes de mas 
        {
            EsperarMonedas.Enabled = false;
            EsperarMonedas.Stop();


            try
            {
                EstadoVueltoResp estadovuelto = new EstadoVueltoResp();
                //var auxvuelto = Globals.Vuelto;
                estadovuelto = EstadoDelVuelto(montodeVuelto);
                if (estadovuelto.data.VueltoFinalizado == false)// si el vuelto no se finaliza el timer se reinicia
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
                {// si el vuelto esta listo se apaga la maquina  y se actualizan los estados correspondientes
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

        protected void Timer_EstadoCancelarPago(object sender, ElapsedEventArgs e)// timer utilizado cuando se cancela, para devolver todo el dinero ingresado 
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
                else
                {
                    transaccion.FinTransaccion();
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
