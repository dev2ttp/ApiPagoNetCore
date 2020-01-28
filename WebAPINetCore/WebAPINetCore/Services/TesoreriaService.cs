using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class TesoreriaService
    {
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
                Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.CierreZ, Globals.data);
                Globals.Servicio1.SendMessage(ServicioPago.Comandos.CierreZ);

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

        public string VaciarGaveta(string gvo, string gvd)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(gvo);
            data.Add(gvd);

            return "OK";
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_billete, data);
            var resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_billete);

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

        public string Estadovaciado(string GavID)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();
            data.Add(GavID);
            //data.Add(Global.ba.nserialgv);
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.vacio_billete_compr, data);
            var resultado = pipeClient.SendMessage(ServicioPago.Comandos.vacio_billete_compr);

            if (resultado)
            {
                return "OK";
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

        public string CargarMoneda(GavMR MonIngresadas) {
            try
            {

                PipeClient2 pipeClient = new PipeClient2();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>();
                data.Add("M");
                data.Add(MonIngresadas.Idgav); // numero serie de gaveta
                if (int.Parse(MonIngresadas.M10) > 0)
                {
                    data.Add(MonIngresadas.M10);
                }
                if (int.Parse(MonIngresadas.M50) > 0)
                {
                    data.Add(MonIngresadas.M50);
                }
                if (int.Parse(MonIngresadas.M100) > 0)
                {
                    data.Add(MonIngresadas.M100);
                }
                if (int.Parse(MonIngresadas.M500) > 0)
                {
                    data.Add(MonIngresadas.M500);
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

    }
}
