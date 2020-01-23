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
                            ObtenerBilletes(saldo[5], "BA");
                        }
                        if (saldo[1] == "BR")
                        {
                            ObtenerBilletes(saldo[5], "BR");
                        }

                        if (saldo[1] == "MB")
                        {
                            ObtenerMonedsas(saldo[5], "MB");
                        }

                        if (saldo[1] == "MR")
                        {
                            ObtenerMonedsas(saldo[5], "MR");
                        }

                    }
                    Globals.Servicio2.Resultado.Data[1].Split("~");
                }

            }
        }

        public void RealizarCierreZ(int mantisa) {

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

        public void ObtenerReporteCierreZ() {

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
            else { 
            
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
