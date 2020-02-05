using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class TesoreriaService
    {
        private static readonly HttpClient client = new HttpClient();


        public void SaldoTransaccion()
        {
            Globals.data = new List<string>();
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.estadosaldo, Globals.data);
            var respuesta = Globals.Servicio2.SendMessage(ServicioPago.Comandos.estadosaldo);
            if (respuesta)
            {
                if (Globals.Servicio2.Resultado.Data.Count > 1)
                {
                    Globals.Saldos = new SaldoGaveta();
                    foreach (var item in Globals.Servicio2.Resultado.Data)
                    {
                        var saldo = item.Split("~");

                        if (saldo[1] == "BA")
                        {
                            Globals.Saldos.BA.Idgav = saldo[0];
                            ObtenerBilletes(saldo[5], "BA");
                        }
                        if (saldo[1] == "BR")
                        {
                            Globals.Saldos.BR.Idgav = saldo[0];
                            ObtenerBilletes(saldo[5], "BR");
                        }

                        if (saldo[1] == "MB")
                        {
                            Globals.Saldos.MB.Idgav = saldo[0];
                            ObtenerMonedsas(saldo[5], "MB");
                        }

                        if (saldo[1] == "MR")
                        {
                            Globals.Saldos.MR.Idgav = saldo[0];
                            ObtenerMonedsas(saldo[5], "MR");
                        }

                    }
                    Globals.Servicio2.Resultado.Data[1].Split("~");
                }

            }
        }

        public void RealizarCierreZ(int mantisa)
        {

            Globals.data = new List<string>();
            Globals.data.Add(mantisa.ToString());
            Globals.Servicio1 = new PipeClient();
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.CierreZ, Globals.data);
            Globals.Cierrez = new DatosCierreZ();
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.CierreZ);
            if (respuesta)
            {
                //Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.CierreZ, Globals.data);
                //Globals.Servicio1.SendMessage(ServicioPago.Comandos.CierreZ);

                //<< 0,Fecha 1,Hora 2,IdZeta 3,Cantidad 4,Monto
                string[] msg = Globals.Servicio1.Resultado.Data[0].Split('~');
                if (msg.Length < 3)
                {
                    Globals.Cierrez.MensajeaMostrar = "Error en cierre Z";
                }
                else if (msg[2] == "0")
                {
                    Globals.Cierrez.MensajeaMostrar = "No hay cierres Z que realizar actualmente";
                }
                else
                {
                    Globals.Cierrez.MensajeaMostrar = "El Cierre Z se ha realizado satisfactorimente";
                    Globals.Cierrez.Fecha = msg[0];
                    Globals.Cierrez.Hora = msg[1];
                    Globals.Cierrez.IDZCierre = msg[2];
                    Globals.Cierrez.Cantidad = msg[3];
                    Globals.Cierrez.MontoTotal = msg[4];
                }
            }
        }

        public void ObtenerReporteCierreZ()
        {

            Globals.data = new List<string>();
            Globals.Servicio1 = new PipeClient();
            Globals.fechasIDs = new IDReportesCierre();
            Globals.data.Add("1"); //0
            Globals.data.Add("L");
            Globals.data.Add("0");
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.ReporteCierreZ, Globals.data);
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.ReporteCierreZ);
            if (respuesta)
            {
                if (Globals.Servicio1.Resultado.Data.Count > 1)
                {
                    for (int i = 1; i < Globals.Servicio1.Resultado.Data.Count; i++)
                    {
                        //"Z~1030033~2019-10-08~0
                        if (i <= 4)
                        {
                            var datosCierres = Globals.Servicio1.Resultado.Data[i].Split("~"); ;
                            Globals.fechasIDs.Fechas.Add(datosCierres[2]);
                            Globals.fechasIDs.ID.Add(datosCierres[1]);
                        }

                    }
                }
                else
                {
                    //Global.mensajeRorte = "No hay cierres Z para mostrar actualmente";
                }
            }
            else
            {

            }

        }

        public void ObtenerReportebyID(string idz)
        {
            Globals.data = new List<string>();
            Globals.Servicio1 = new PipeClient();
            Globals.fechasIDs = new IDReportesCierre();
            Globals.DatosCierre = new ReportesCierre();
            Globals.data.Add("1"); //0
            Globals.data.Add("Z");
            Globals.data.Add(idz);
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.ReporteCierreZ, Globals.data);
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.ReporteCierreZ);
            if (respuesta)
            {
                if (Globals.Servicio1.Resultado.Data.Count > 1)
                {
                    var datosCierres = Globals.Servicio1.Resultado.Data[0].Split("~");
                    Globals.DatosCierre.Fechas = datosCierres[0];
                    Globals.DatosCierre.Hora = datosCierres[1];
                    Globals.DatosCierre.ID = datosCierres[2];

                    datosCierres = Globals.Servicio1.Resultado.Data[2].Split("~");
                    Globals.DatosCierre.Cantidad = datosCierres[datosCierres.Length - 2];
                    Globals.DatosCierre.MontoTotal = datosCierres[datosCierres.Length - 1];

                    datosCierres = Globals.Servicio1.Resultado.Data[Globals.Servicio1.Resultado.Data.Count - 1].Split("~");
                    if (datosCierres.Length > 2)
                    {
                        Globals.DatosCierre.MontoTotal = datosCierres[3];
                    }



                }
                else
                {
                    //Global.mensajeRorte = "No hay cierres Z para mostrar actualmente";
                }
            }
            else
            {

            }
        }

        public string VaciarGaveta(string gvo, string gvd, string tipo)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(gvo);
            data.Add(gvd);

            //return "OK";
            var resultado = false;
            if (tipo == "B")
            {
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_billete, data);
                resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_billete);
            }
            else {
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_moneda, data);
                resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_moneda);
            }
            

            if (resultado)
            {
                string msg = pipeClient._Resp;
                if (msg.Contains("OK"))
                {
                    return "OK";
                }
                else
                {
                    return "NOK";
                }
            }
            else
            {
                return "NOK";
            }

        }

        public string Estadovaciado(string GavID, string tipo)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();
            data.Add(GavID);
            //data.Add(Global.ba.nserialgv);
            var resultado = false;
            if (tipo == "B")
            {
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_billete_compr, data);
                resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_billete_compr);
            }
            else
            {
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_moneda_compr, data);
                resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_moneda_compr);
            }
            if (resultado)
            {
                //return "OK";
                string msg = pipeClient._Resp;


                if (msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {
                        return "OK";
                }
            }
            else
            {
                return "ERROR";
            }

        }

        public string Iniciocarga()
        {
            Globals.log.Debug("Inicio de Carga de Dinero");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2Inicio = new PipeClient2();
            Globals.Servicio2Inicio.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Ini_Agregar_dinero, Globals.data);
            var respuesta = Globals.Servicio2Inicio.SendMessage(ServicioPago.Comandos.Ini_Agregar_dinero);
            if (respuesta)
            {
                if (Globals.Servicio2Inicio.Resultado.Data[0].Contains("OK"))
                {
                    return "OK";
                }
                else
                {
                    return "NOK";
                }
            }
            else
            {
                Globals.log.Error("Eror al activar la maquina Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2Inicio.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2Inicio._Resp);
                return "NOK";
            }
        }

        public string FinalizarCarga()
        {
            Globals.log.Debug("Finalizar Carga");
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Fin_agregar_dinero, Globals.data);
            var vuelta = Globals.Servicio2.SendMessage(ServicioPago.Comandos.Fin_agregar_dinero);
            if (vuelta)
            {
                if (Globals.Servicio2.Resultado.Data[0].Contains("OK"))
                {
                    return "OK";
                }
                else
                {
                    Globals.log.Error("Ha ocurrido un error al Finalizar el Pago o Apagar la maquina Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2._Resp);
                    return "NOK";
                }

            }
            else
            {
                Globals.log.Error("Ha ocurrido un error al Finalizar el Pago o Apagar la maquina Transaccion: " + Globals.IDTransaccion + " Error:" + Globals.Servicio2.Resultado.CodigoError + " Respuesta completa " + Globals.Servicio2._Resp);
                return "NOK";
            }
        }

        public string EstadoCarga()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2Pago = new PipeClient2();
            Globals.Servicio2Pago.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.Cons_dinero_ingre, Globals.data);
            var respuesta = Globals.Servicio2Pago.SendMessage(ServicioPago.Comandos.Cons_dinero_ingre);
            // llamar al servicio pipe para traer los datos

            if (respuesta)
            {
                try
                {
                    var DineroIngresado = Globals.Servicio2Pago.Resultado.Data[0];
                    return DineroIngresado;
                }
                catch (Exception ex)
                {
                    return "Error";
                }
            }
            else
            {
                return "Error";
            }
        }

        public string CargarMoneda(GavMR MonIngresadas)
        {
            try
            {

                PipeClient2 pipeClient = new PipeClient2();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>();
                data.Add("M");
                data.Add(MonIngresadas.Idgav); // numero serie de gaveta
                if (int.Parse(MonIngresadas.M10) > 0)
                {
                    data.Add("10," + MonIngresadas.M10);
                }
                if (int.Parse(MonIngresadas.M50) > 0)
                {
                    data.Add("50," + MonIngresadas.M50);
                }
                if (int.Parse(MonIngresadas.M100) > 0)
                {
                    data.Add("100," + MonIngresadas.M100);
                }
                if (int.Parse(MonIngresadas.M500) > 0)
                {
                    data.Add("500," + MonIngresadas.M500);
                }
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.Agregar_diner, data);
                pipeClient.SendMessage(ServicioPago.Comandos.Agregar_diner);

                //<< 0,Fecha 1,Hora 2,IdZeta 3,Cantidad 4,Monto
                // string[] msg = pipeClient.Resultado.Data;
                if (pipeClient.Resultado.Data.Count > 0)
                {
                    return "OK";
                }

                else
                {
                    Globals.log.Error("error al enviar el comando " + pipeClient.Resultado.CodigoError);
                    return "NOK";
                }
            }
            catch (Exception ex)
            {
                Globals.log.Error("error al enviar el comando " + ex);
                return "NOK";
            }
        }

        public string InsertarDisp(string tipodisp)
        {
            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(tipodisp);

            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.agregdis, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.agregdis);
            if (respuesta)
            {
                string msg = pipeClient._Resp;

                if (msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {
                    //if (msg.Contains("OK"))

                    var resultado = pipeClient.Resultado.Data[0].Split("~");
                    string respuestaserv = resultado[resultado.Length - 1];
                    if (respuestaserv.Contains("diferente"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            else
            {
                return "NOK";
            }
        }


        public string RetirararDisp(string tipodisp)
        {
            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(tipodisp);

            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.retirdisp, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.retirdisp);
            if (respuesta == true)
            {
                string msg = pipeClient._Resp;

                if (msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {

                    var resultado = pipeClient.Resultado.Data[0].Split("~");
                    string respuestaserv = resultado[resultado.Length - 1];
                    if (respuestaserv.Contains("no existe"))
                    {
                        return "no existe";
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            else
            {
                return "NOK";
            }

        }

        public string AgregarGav(GavReq gav)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();
            data.Add(gav.Tipo);
            data.Add(gav.Idgav);

            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.agregav, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.agregav);
            if (respuesta == true)
            {
                string msg = pipeClient._Resp;

                if (msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {
                    //if (msg.Contains("OK"))

                    var resultado = pipeClient.Resultado.Data[0].Split("~");
                    string respuestaserv = resultado[resultado.Length - 1];
                    if (respuestaserv.Contains("diferente"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            else
            {
                return "NOK";
            }
        }

        public string RetirarGavB(string IDGAV)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(IDGAV);
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.retirargavBillete, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.retirargavBillete);
            string msg = pipeClient._Resp;
            if (respuesta == true)
            {
                if(msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {
                    //if (msg.Contains("OK"))

                    var resultado = pipeClient.Resultado.Data[0].Split("~");
                    string respuestaserv = resultado[resultado.Length - 1];
                    if (respuestaserv.Contains("diferente"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            else
            {
                return "NOK";
            }
        }

        public string RetirarGavM(string IDGAV)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(IDGAV);
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.retirargavMoneda, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.retirargavMoneda);
            if (respuesta == true)
            {
                string msg = pipeClient._Resp;
                if(msg.Contains("NOK"))
                {
                    return "NOK";
                }
                else
                {
                    //if (msg.Contains("OK"))

                    var resultado = pipeClient.Resultado.Data[0].Split("~");
                    string respuestaserv = resultado[resultado.Length - 1];
                    if (respuestaserv.Contains("diferente"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            else
            {
                return "NOK";
            }

        }

        public async Task ImprecionCierreZAsync(DatosCierreZ Cierre) {

            try
            {
                StreamReader objReader = new StreamReader("./Documentos/cierreZ.txt");
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

                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);
                comprobante = comprobante.Replace("DD-MM-AAAA", Cierre.Fecha);
                comprobante = comprobante.Replace("HH:MM:SS", Cierre.Hora);
                comprobante = comprobante.Replace("XXID", Cierre.IDZCierre);
                comprobante = comprobante.Replace("CNT", Cierre.Cantidad);
                comprobante = comprobante.Replace("MTD", Cierre.MontoTotal);

                var respuesta = await ImprimirComprobanteAsync(comprobante);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }
        }


        public void ObtenerBilletes(string saldo, string Tipo)
        {

            var denominacion = saldo.Split(";");
            foreach (var Billete in denominacion)
            {
                var cantidad = Billete.Split(",");
                if (cantidad[0] == "1000")
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B1000 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B1000 = cantidad[1];
                    }
                }

                if (cantidad[0] == "2000")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B2000 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B2000 = cantidad[1];
                    }
                }

                if (cantidad[0] == "5000")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B5000 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B5000 = cantidad[1];
                    }
                }

                if (cantidad[0] == "10000")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B10000 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B10000 = cantidad[1];
                    }
                }

                if (cantidad[0] == "20000")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B20000 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B20000 = cantidad[1];
                    }
                }

            }
        }

        public void ObtenerMonedsas(string saldo, string Tipo)
        {

            var denominacion = saldo.Split(";");
            foreach (var Moneda in denominacion)
            {
                var cantidad = Moneda.Split(",");
                if (cantidad[0] == "10")
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M10 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M10 = cantidad[1];
                    }
                }

                if (cantidad[0] == "50")
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M50 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M50 = cantidad[1];
                    }
                }

                if (cantidad[0] == "100")
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M100 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M100 = cantidad[1];
                    }
                }

                if (cantidad[0] == "500")
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M500 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M500 = cantidad[1];
                    }
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
    }
}
