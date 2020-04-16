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
        private static readonly HttpClient client = new HttpClient();
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
                ArmarDocuemntoPagoCanceladosync();
                var finalizar = FinalizarPago();
                var finHopper = FinalizarOperacion();
                return true;
            }
            else
            {
                var finalizar = FinalizarPago();
                var finHopper = FinalizarOperacion();
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
            EstadoPago PagoInfo = new EstadoPago();
            Globals.ImpresoraMontoEnCancelar = Globals.ImpresoraMontoIngresado - Globals.ImpresoraMontoEntregado;
            lock (thisLock)
            {
                var finalizar = FinalizarPago();
                if (Globals.ImpresoraMontoEnCancelar > 0)
                {
                    if (finalizar)
                    {
                        Task.Delay(int.Parse(Globals._config["TiempoDelay:EsperarVuelto"])).Wait();
                        var IniciooPago = InicioPago();
                        if (IniciooPago == true)
                        {
                            var vuelto = Globals.ImpresoraMontoEnCancelar;
                            var DarVuelto = controPeri.DarVuelto(vuelto);
                            if (DarVuelto)// si la maquina devuelve que esta dando vueltos
                            {
                                return true;
                            }
                            else
                            {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                                return false;
                            }
                        }
                        else
                        {// para decir que la maquina no esta dando vuelto y que hay que volver a llamar a al timer
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                
            }
        }

        public CancelarPago EstadoDeCancelacion()
        {
            CancelarPago estado = new CancelarPago();
            bool vueltoTerminado = false;
            if (Globals.ImpresoraMontoEnCancelar == 0)
            {
                ConfigurarStatus();
                estado.StatusMaquina = Globals.SaludMaquina;
                estado.BloqueoEfectivo = Globals.BloqueoEfectivo;
                estado.BloqueoTransbank = Globals.BloqueoTransbank;
                estado.CancelacionCompleta = true;
                estado.EntregandoVuelto = false;
                var finalizar = FinalizarPago();
                var finHopper = FinalizarOperacion();
            }
            lock (thisLock)
            {
                var Vuelto = controPeri.VueltoRegresado();
                ConfigurarStatus();
                estado.StatusMaquina = Globals.SaludMaquina;
                estado.BloqueoEfectivo = Globals.BloqueoEfectivo;
                estado.BloqueoTransbank = Globals.BloqueoTransbank;
                if (Vuelto != "false")
                {
                    if (Vuelto.Contains("OK"))// si termina de dr vuelto
                    {
                        vueltoTerminado = true;
                        Vuelto = Vuelto.Replace("OK", "");
                    }
                    Globals.ImpresoraMontoEntregadoCancelar = float.Parse(Vuelto);
                    if (vueltoTerminado)
                    {
                        estado.CancelacionCompleta = true;
                        estado.EntregandoVuelto = false;
                        var finalizar = FinalizarPago();
                        var finHopper = FinalizarOperacion();
                        return estado;
                    }
                    else
                    {// si vuelto aun no termina 
                        estado.CancelacionCompleta = false;
                        estado.EntregandoVuelto = true;
                        return estado;
                    }
                }
                else
                {
                    estado.CancelacionCompleta = false;
                    estado.EntregandoVuelto = false;
                    var finalizar = FinalizarPago();
                    var finHopper = FinalizarOperacion();
                    return estado;
                }
            }
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
                comprobante = comprobante.Replace("xxidm", Globals.idmaquina);
                comprobante = comprobante.Replace("xxnommaq", Globals.nombremaquina);
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
                comprobante = comprobante.Replace("xxidm", Globals.idmaquina);
                comprobante = comprobante.Replace("xxnommaq", Globals.nombremaquina);
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

        public async void ArmarDocuemntoPagoCanceladosync()
        {
            try
            {
                StreamReader objReader = new StreamReader("./Documentos/Comprobante_Pago_Cancelado.txt");
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
                comprobante = comprobante.Replace("xxidm", Globals.idmaquina);
                comprobante = comprobante.Replace("xxnommaq", Globals.nombremaquina);
                comprobante = comprobante.Replace("XXXAPAGAR", "$ " + Globals.ImpresoraMontoPagar.ToString());
                comprobante = comprobante.Replace("XXXPAGADO", "$ " + (Globals.ImpresoraMontoIngresado).ToString());
                comprobante = comprobante.Replace("XXXVAENTREGAR", "$ " + Globals.ImpresoraMontoEntregado.ToString());
                comprobante = comprobante.Replace("XXXVENTREGADO", "$ " + Globals.ImpresoraMontoVueltoEntregar.ToString());
                comprobante = comprobante.Replace("XXXCAENTREGAR", "$ " + Globals.ImpresoraMontoEnCancelar.ToString());
                comprobante = comprobante.Replace("XXXCENTREGADO", "$ " + Globals.ImpresoraMontoEntregadoCancelar.ToString());
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
                    float Dinero = float.Parse(DineroIngresado);
                    PagoInfo.DineroIngresado = Dinero;
                    PagoInfo.DineroFaltante = PagoInfo.MontoAPagar - Dinero;
                    if (Dinero > Globals.ImpresoraMontoIngresado)
                    {
                        Globals.ImpresoraMontoIngresado = Dinero;
                    }
                    // Actualizar Datos
                    if (PagoInfo.DineroFaltante < 0)
                    {// si hay que dar vuelto
                        FinalizarPago();
                        EstadoDelPAgo.Status = true;
                        EstadoDelPAgo.PagoStatus = false;// estado que se retornan al front
                        Task.Delay(int.Parse(Globals._config["TiempoDelay:EsperarVuelto"])).Wait();
                        PagoInfo = RealizarCalculoVuelto(PagoInfo.MontoAPagar);
                        EstadoDelPAgo.data = PagoInfo;
                        return EstadoDelPAgo;
                    }
                    else if (PagoInfo.DineroFaltante == 0)// si el pago se realizo completamente
                    {
                        var finalizar = FinalizarPago();
                        if (finalizar == true)
                        {
                            Task.Delay(int.Parse(Globals._config["TiempoDelay:EsperarVuelto"])).Wait();
                            PagoInfo.DineroFaltante = ConsultarDineroExtra(PagoInfo.MontoAPagar);

                            if (PagoInfo.DineroFaltante == 0)
                            {
                                if (Globals.ComprobanteImpreso == false && Globals.ImpresoraMontoIngresado > 0)
                                {
                                    ArmarDocuemntoPagoCompeltoAsync(PagoInfo);// armar datos de ticket
                                    Globals.ComprobanteImpreso = true;
                                }

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
                        Vuelto = Vuelto.Replace("OK", "");
                    }
                    var VueltoEntregado = float.Parse(Vuelto);
                    VueltoInfo.DineroRegresado = VueltoEntregado;
                    VueltoInfo.DineroFaltante = VueltoInfo.VueltoTotal - VueltoEntregado;
                    VueltoInfo.VueltoFinalizado = vueltoTerminado;
                    if (VueltoEntregado > Globals.ImpresoraMontoEntregado)
                    {
                        Globals.ImpresoraMontoEntregado = VueltoEntregado;
                    }

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

        public float ConsultarDineroExtra(float MontoApagar)
        {
            try
            {
                EstadoPago estadopago = new EstadoPago();
                estadopago.MontoAPagar = MontoApagar;
                estadopago = MicroConsultaPago(estadopago);

                float dineroF = new float();
                dineroF = MontoApagar - estadopago.DineroIngresado;
                if (estadopago.DineroIngresado > Globals.ImpresoraMontoIngresado)
                { Globals.ImpresoraMontoIngresado = estadopago.DineroIngresado; }
                if (dineroF < 0)// si despues de finalizar se ingreso dinero extra se llama el vuelto 
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        dineroF = dineroF * -1;
                        var DarVuelto = controPeri.DarVuelto(dineroF);
                        if (dineroF > Globals.ImpresoraMontoVueltoEntregar)
                        { Globals.ImpresoraMontoVueltoEntregar = dineroF; }
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

        public EstadoPago RealizarCalculoVuelto(float MontoApagar) // Proceso que se encarga de preparar la maquina para entregar vuelto 
        {
            EstadoPago estadopago = new EstadoPago();
            try
            {
                estadopago.MontoAPagar = MontoApagar;
                estadopago = MicroConsultaPago(estadopago);// llamada para saber que monto actual de vuelto hay que realizar
                if (estadopago.DineroIngresado > Globals.ImpresoraMontoIngresado)
                { Globals.ImpresoraMontoIngresado = estadopago.DineroIngresado; }

                if (estadopago.DineroFaltante < 0 && estadopago.DineroFaltante != -1)
                {
                    var IniciooPago = InicioPago();
                    if (IniciooPago == true)
                    {
                        var vuelto = estadopago.DineroFaltante * -1;
                        var DarVuelto = controPeri.DarVuelto(vuelto);
                        if (vuelto > Globals.ImpresoraMontoVueltoEntregar)
                        { Globals.ImpresoraMontoVueltoEntregar = vuelto; }
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
                estadopago.DineroFaltante = -1;// si tiene agun error 
                return estadopago;
            }
            estadopago.DineroFaltante = -1;// si tiene agun error 
            return estadopago;
        }
        public EstadoPago MicroConsultaPago(EstadoPago PagoInfo)
        {
            var EstadoPgo = controPeri.DinerIngresado(); // llamar al servicio pipe para traer los datos
            if (EstadoPgo != "false")
            {
                try
                {
                    //Actualizar los datos de pago
                    var DineroIngresado = float.Parse(EstadoPgo);
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