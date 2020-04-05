using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.PipeServer;
using System.Timers;
using System.Net.Http;
using System.Text;
using System.IO;
using Newtonsoft.Json;

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
        ControladorPerisfericos controPeri = new ControladorPerisfericos();

        public bool InicioPago()
        {
            var inicio = controPeri.IniciarPago();
            var resPayout = controPeri.InicioPayout();
            var resHooper = controPeri.InicioHopper();
            if (inicio && resPayout && resHooper)
            {
                Globals.MaquinasActivadas = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FloatByDenomination()
        {
            var Resfloat = controPeri.FloatByDenomination();
            return Resfloat;
        }

        public bool EstadoSalud()
        {
            var Ressalud = controPeri.EstadoSalud();
            if (Ressalud)
            {
                EstadoDeSaludService estadoSalud = new EstadoDeSaludService();
                estadoSalud.DescifrarEstadoDeSalud();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DetenerVuelto()
        {
            var RespDtVulto = controPeri.DetenerVuelto();
            if (RespDtVulto)
            {
                EstadoDeSaludService estadoSalud = new EstadoDeSaludService();
                estadoSalud.DescifrarEstadoDeSalud();
                ArmarDocuemntoPagoConVueltoAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ConfigurarStatus()
        {

            EstadoDeSaludService estadoSalud = new EstadoDeSaludService();
            estadoSalud.DescifrarEstadoDeSalud();
        }

        public bool FinalizarPago()
        {
            var ResfinPag = controPeri.FinalizarPago();
            var resPayout = controPeri.FinPayout();
            if (ResfinPag && resPayout)
            {

                Globals.MaquinasActivadas = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FinalizarOperacion()
        {
            Globals.log.Debug("Finalizar hopper");
            var resPayout = controPeri.FinHopper();
            if (resPayout)
            {
                Globals.MaquinasActivadas = false;
                return true;
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
                        Globals.log.Error("Ha ocurrido un error al Cancelar  el Pago Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Cancelar.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Cancelar._Resp);
                        return false;
                    }

                }
                else
                {
                    Globals.log.Error("Ha ocurrido un error al Cancelar  el Pago Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Cancelar.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Cancelar._Resp);
                    return false;
                }
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



        public async Task<bool> ImprimirComprobanteAsync(String Documento)
        {
            var url = string.Format(Globals._config["Urls:Impresion"]);
            Impresion documento = new Impresion();
            documento.document = Documento;
            var json = JsonConvert.SerializeObject(documento);
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
                comprobante = comprobante.Replace("XXXAPAGAR", "$ " + Globals.ImpresoraMontoPagar.ToString());
                comprobante = comprobante.Replace("XXXPAGADO", "$ " + Globals.ImpresoraMontoIngresado.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);

                var respuesta = await ImprimirComprobanteAsync(comprobante);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
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
                comprobante = comprobante.Replace("XXXAPAGAR", "$ " + Globals.ImpresoraMontoPagar.ToString());
                comprobante = comprobante.Replace("XXXPAGADO", "$ " + (Globals.ImpresoraMontoIngresado).ToString());
                comprobante = comprobante.Replace("XXXVAENTREGAR", "$ " + Globals.ImpresoraMontoEntregado.ToString());
                comprobante = comprobante.Replace("XXXVENTREGADO", "$ " + Globals.ImpresoraMontoVueltoEntregar.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);

                var respuesta = await ImprimirComprobanteAsync(comprobante);
            }
            catch (Exception ex)
            {

                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }

        }


        public EstadoPagoResp EstadoDelPAgo(EstadoPago PagoInfo)// se encarga de consultar el estado en que se encuentra el pago 
        {
            lock (thisLock)
            {
                EstadoPagoResp EstadoDelPAgo = new EstadoPagoResp();// variable que se retorna
                var DineroIngresado = controPeri.DinerIngresado();
                if (DineroIngresado != "false")
                {
                    int Dinero = int.Parse(DineroIngresado);
                    PagoInfo.DineroIngresado = Dinero;
                    PagoInfo.DineroFaltante = PagoInfo.MontoAPagar - Dinero;
                    // Actualizar Datos
                    if (PagoInfo.DineroFaltante < 0)
                    {// si hay que dar vuelto
                        FinalizarPago();
                        EstadoDelPAgo.Status = true;
                        EstadoDelPAgo.PagoStatus = false;// estado que se retornan al front
                        Task.Delay(3000).Wait();
                        PagoInfo = RealizarCalculoVuelto(PagoInfo.MontoAPagar);
                        EstadoDelPAgo.data = PagoInfo;
                        return EstadoDelPAgo;
                    }
                    else if (PagoInfo.DineroFaltante == 0)// si el pago se realizo completamente
                    {
                        var finalizar = FinalizarPago();
                        if (finalizar == true)
                        {
                            Task.Delay(3000).Wait();
                            PagoInfo.DineroFaltante = ConsultarDineroExtra(PagoInfo.MontoAPagar);

                            if (PagoInfo.DineroFaltante == 0)
                            {
                                ArmarDocuemntoPagoCompeltoAsync(PagoInfo);// armar datos de ticket
                                Globals.ComprobanteImpreso = true;
                                EstadoDelPAgo.data = PagoInfo;
                                EstadoDelPAgo.Status = true;
                                EstadoDelPAgo.PagoStatus = true;
                                var finHopper = FinalizarOperacion();
                                return EstadoDelPAgo;
                            }

                            else if (PagoInfo.DineroFaltante < 0)
                            {
                                EstadoDelPAgo.data = PagoInfo;
                                EstadoDelPAgo.Status = true;
                                EstadoDelPAgo.PagoStatus = false;
                                return EstadoDelPAgo;
                            }
                        }
                        else
                        {
                            EstadoDelPAgo.Status = false;
                            EstadoDelPAgo.PagoStatus = false;
                            return EstadoDelPAgo;
                        }
                    }// si no se ha completado un pago
                    EstadoDelPAgo.data = PagoInfo;
                    EstadoDelPAgo.Status = true;
                    EstadoDelPAgo.PagoStatus = false;
                    return EstadoDelPAgo;
                }
                else
                {
                    EstadoDelPAgo.Status = false;
                    EstadoDelPAgo.PagoStatus = false;
                    return EstadoDelPAgo;
                }
            }
        }

        public EstadoVueltoResp EstadoDelVuelto(EstadoVuelto VueltoInfo)// ver estado del vuelto a dar
        {
            bool vueltoTerminado = false;
            lock (thisLock)
            {
                EstadoVueltoResp estavuelto = new EstadoVueltoResp();
                var Vuelto = controPeri.VueltoRegresado();

                if (Vuelto != "false")
                {
                    if (Vuelto.Contains("OK"))// si termina de dr vuelto
                    {
                        vueltoTerminado = true;
                        Vuelto.Replace("OK", "");
                    }
                    var VueltoEntregado = int.Parse(Vuelto);
                    VueltoInfo.DineroRegresado = VueltoEntregado;
                    VueltoInfo.DineroFaltante = VueltoInfo.VueltoTotal - VueltoEntregado;
                    VueltoInfo.VueltoFinalizado = vueltoTerminado;

                    if (vueltoTerminado)
                    {
                        var finalizar = FinalizarPago();
                        var finHopper = FinalizarOperacion();
                        if (finalizar == true && finHopper == true)
                        {// si la maquina se apaag corectamente
                            estavuelto.Status = true;
                            estavuelto.PagoStatus = true;
                            if (Globals.ComprobanteImpresoVuelto == false)// imprimir el comprobante
                            {
                                Globals.ComprobanteImpresoVuelto = true;
                                ArmarDocuemntoPagoConVueltoAsync();
                            }
                            transaccion.FinTransaccion();// finalizar la transaccion actual
                            estavuelto.data = VueltoInfo;
                            return estavuelto;
                        }
                        else
                        {// si la maquina no finaliza correctamente
                            estavuelto.PagoStatus = false;
                            estavuelto.Status = false;
                            return estavuelto;
                        }
                    }
                    else
                    {// si vuelto aun no termina 
                        estavuelto.Status = true;
                        estavuelto.PagoStatus = false;
                        estavuelto.data = VueltoInfo;
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
                Globals.log.Error("Ha ocurrido un exepcion en el proceso Timer_VueltoMonedas Transaccion: " + Globals.IDTransaccion + "Mensaje de error: " + ex.Message);
                //throw;
            }
        }

        protected void Timer_EstadoCancelarPago(object sender, ElapsedEventArgs e)// timer utilizado cuando se cancela, para devolver todo el dinero ingresado 
        {
            TimerCancerlarPago.Enabled = false;
            TimerCancerlarPago.Stop();

            try
            {

                if (Globals.HayVuelto == false)
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {

                        Globals.data = new List<string>();
                        Globals.data.Add(Globals.dineroIngresado);
                        Globals.Servicio2Vuelto = new PipeClient2();
                        Globals.Servicio2Vuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DarVuelto, Globals.data);
                        var DarVuelto = Globals.Servicio2Vuelto.SendMessage(ServicioPago.Comandos.DarVuelto);
                        if (DarVuelto)
                        {
                            Globals.ImpresoraMontoVueltoEntregar = int.Parse(Globals.dineroIngresado);
                            Globals.Vuelto.VueltoTotal = int.Parse(Globals.dineroIngresado);
                            Globals.Vuelto.DineroRegresado = int.Parse(Globals.dineroIngresado);
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

                    EstadoPagoResp estadopago = new EstadoPagoResp();
                    montoapagar.MontoAPagar = 9999999;
                    var auxPago = Globals.Pago;
                    estadopago = EstadoDelPAgo(montoapagar);
                    if (estadopago.data.DineroIngresado > 0)
                    {
                        Globals.dineroIngresado = estadopago.data.DineroIngresado.ToString();
                        Globals.DineroIngresadoSolicitado = true;
                        var IniciooPago = InicioPago();
                        if (IniciooPago == true)
                        {
                            Globals.Pago = auxPago;
                            Globals.data = new List<string>();
                            Globals.data.Add(estadopago.data.DineroIngresado.ToString());
                            Globals.ImpresoraMontoVueltoEntregar = estadopago.data.DineroIngresado;
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

            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un exepcion en el proceso Timer_EstadoCancelarPago Transaccion: " + Globals.IDTransaccion + "Mensaje de error: " + ex.Message);
                //
            }
        }

        public int ConsultarDineroExtra(int MontoApagar)
        {
            try
            {
                EstadoPago estadopago = new EstadoPago();
                estadopago.MontoAPagar = MontoApagar;
                estadopago = MicroConsultaPago(estadopago);

                int dineroF = new int();
                dineroF = MontoApagar - estadopago.DineroIngresado;
                if (dineroF < 0)// si despues de finalizar se ingreso dinero extra se llama el vuelto 
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        while (Globals.MaquinasActivadas == false)// si por alguna razon la maquina se encuentra desactivada al realizar el pago
                        {
                            InicioPago();
                        }
                        dineroF = dineroF * -1;
                        var DarVuelto = controPeri.DarVuelto();
                        if (DarVuelto)// si se empieza  a dar vuelto  se llama a un timer de consulta para que luego apague la maquina
                        {
                            return dineroF;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return dineroF;
                    }
                }
                else
                {// se actualiza la transaxxion  y se cambia a que la cancelacion no ha comensado
                    transaccion.FinTransaccion();
                    return dineroF;
                }
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un exepcion en el proceso consultar dinero extra Transaccion: " + Globals.IDTransaccion + "Mensaje de error: " + ex.Message);
                return -1;
            }
        }

        public EstadoPago RealizarCalculoVuelto(int MontoApagar) // Proceso que se encarga de preparar la maquina para entregar vuelto 
        {
            try
            {

                EstadoPago estadopago = new EstadoPago();
                estadopago.MontoAPagar = MontoApagar;
                estadopago = MicroConsultaPago(estadopago);// llamada para saber que monto actual de vuelto hay que realizar
                Globals.dineroIngresado = estadopago.DineroIngresado.ToString();
                if (estadopago.DineroFaltante < 0 && estadopago.DineroFaltante != -1)
                {
                    Globals.Vuelto.VueltoTotal = estadopago.DineroFaltante * -1;
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        estadopago.DineroFaltante *= -1;
                        var DarVuelto = controPeri.DarVuelto();
                        if (DarVuelto)// si la maquina devuelve que esta dando vueltos
                        {
                            return estadopago;
                        }
                        else
                        {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                            return estadopago;
                        }
                    }
                    else
                    {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                        estadopago.DineroFaltante = -1;// si tiene agun error 
                        return estadopago;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un exepcion en el proceso DarVuelto Transaccion: " + Globals.IDTransaccion + "Mensaje de error: " + ex.Message);
                return false;
            }
            return true;
        }
        public EstadoPago MicroConsultaPago(EstadoPago PagoInfo)
        {
            var EstadoPgo = controPeri.DinerIngresado(); // llamar al servicio pipe para traer los datos
            if (EstadoPgo != "false")
            {
                try
                {
                    //Actualizar los datos de pago
                    var DineroIngresado = int.Parse(EstadoPgo);
                    PagoInfo.DineroIngresado = DineroIngresado;
                    PagoInfo.DineroFaltante = PagoInfo.MontoAPagar - DineroIngresado;
                    return PagoInfo;
                }
                catch (Exception ex)
                {
                    Globals.log.Error("Ha ocurrido un exepcion en el proceso Estado del pago Transaccion: " + Globals.IDTransaccion + "Mensaje de error: " + ex.Message);
                    PagoInfo.DineroIngresado = -1;
                    return PagoInfo;
                }
            }
            else // si no se enviaron los datos correctos dele stado de pago
            {
                Globals.log.Error("Ha ocurrido un error al Consultar el estado del Pago Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Pago.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Pago._Resp);
                PagoInfo.DineroIngresado = -1;
                return PagoInfo;
            }
        }
    }
}
