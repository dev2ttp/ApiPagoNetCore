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
        ControladorPerisfericos controPeri = new ControladorPerisfericos();

        public bool abrirpuerta()
        {
            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            var resultado = false;
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.AbrirPuerta, data);
            resultado = pipeClient.SendMessage(ServicioPago.Comandos.AbrirPuerta);
            return resultado;
        }

        public void SaldoTransaccion()
        {
            Globals.data = new List<string>();
            Globals.Servicio2 = new PipeClient2();
            Globals.Servicio2.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.estadosaldo, Globals.data);
            var respuesta = Globals.Servicio2.SendMessage(ServicioPago.Comandos.estadosaldo);
            if (respuesta)
            {
                Globals.Saldos = new SaldoGaveta();
                if (Globals.Servicio2.Resultado.Data.Count > 1)
                {

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

        public void SaldoMaquinaTrans()
        {
            Globals.billetes = new List<int>();
            Globals.monedas = new List<int>();
            PermitirVueltoService llenarSaldo = new PermitirVueltoService();
            Globals.SaldosMaquina = new SaldoGaveta();
            llenarSaldo.ObteneDineroMaquina();
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

        public void ObtenerReporteCierreZ(int mantisa)
        {

            Globals.data = new List<string>();
            Globals.Servicio1 = new PipeClient();
            Globals.fechasIDs = new IDReportesCierre();
            Globals.data.Add(mantisa.ToString()); //0
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

        public void ObtenerReportebyID(UserToken user)
        {
            Globals.data = new List<string>();
            Globals.Servicio1 = new PipeClient();
            Globals.fechasIDs = new IDReportesCierre();
            Globals.DatosCierre = new ReportesCierre();
            Globals.data.Add(user.IdUser); //0
            Globals.data.Add("Z");
            Globals.data.Add(user.TipoUser);
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.ReporteCierreZ, Globals.data);
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.ReporteCierreZ);
            if (respuesta)
            {
                var msg = Globals.Servicio1.Resultado.Data;
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

                    Globals.RParticularZMontos = Globals.RParticularZGavetas = Globals.RParticularZDiscrepancias = "";
                    Globals.RParticularZmain = new string[7];
                    var main = msg[0].Split('~');
                    Globals.RParticularZmain[0] = main[0];
                    Globals.RParticularZmain[1] = main[1];
                    Globals.RParticularZmain[2] = main[2];
                    Globals.RParticularZmain[3] = main[3];
                    Globals.RParticularZmain[4] = main[4];
                    Globals.RParticularZmain[5] = main[5];
                    Globals.RParticularZmain[6] = main[6];

                    int indice = 0;
                    Globals.RParticularZMontos = "";
                    for (int i = 1; i < msg.Count; i++)
                    {
                        var Montos = msg[i].Split('~');
                        if (Montos[1].Contains("Total"))
                        {
                            indice = i;
                            i = msg.Count;
                            Globals.RParticularZMontos += "Total ";
                            Globals.RParticularZMontos += Montos[4] + " ";
                            Globals.RParticularZMontos += Montos[5] + " \n";

                            //Globals.RParticularZMontos += "    Total      ";
                            //Globals.RParticularZMontos += formatear_bouche(2, "  ", Globals.RParticularZMontos.Length) + " ";
                            //Globals.RParticularZMontos += formatear_bouche(3, Montos[4], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZMontos += formatear_bouche(4, Montos[5], Globals.RParticularZMontos.Length) + " \n            ";
                        }
                        else
                        {
                            Globals.RParticularZMontos += Montos[1] + " ";
                            Globals.RParticularZMontos += Montos[2] + " ";
                            Globals.RParticularZMontos += Montos[4] + " ";
                            Globals.RParticularZMontos += Montos[5] + " \n";
                            //Globals.RParticularZMontos += formatear_bouche(1, Montos[1], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZMontos += formatear_bouche(2, Montos[2], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZMontos += formatear_bouche(3, Montos[4], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZMontos += formatear_bouche(4, Montos[5], Globals.RParticularZMontos.Length) + " \n             ";

                        }

                    }
                    Globals.RParticularZGavetas = "";
                    for (int i = indice + 1; i < msg.Count; i++)
                    {
                        var Gavetas = msg[i].Split('~');
                        if (Gavetas[1].Contains("Total"))
                        {
                            indice = i;
                            i = msg.Count;
                            Globals.RParticularZGavetas += "Total ";
                            Globals.RParticularZGavetas += Gavetas[4] + " ";


                            //Globals.RParticularZGavetas += "    Total      ";
                            //Globals.RParticularZGavetas += formatear_bouche(2, "  ", Globals.RParticularZMontos.Length) + " ";
                            //Globals.RParticularZGavetas += formatear_bouche(3, Gavetas[4], Globals.RParticularZMontos.Length) + "  ";
                            if (Gavetas.Length > 5)
                            {
                                Globals.RParticularZGavetas += Gavetas[5] + " \n";
                                // Globals.RParticularZGavetas += formatear_bouche(4, Gavetas[5], Globals.RParticularZMontos.Length) + " \n            ";
                            }
                            else
                            {
                                Globals.RParticularZGavetas += " \n             ";
                            }
                        }
                        else
                        {
                            Globals.RParticularZGavetas += Gavetas[1] + " ";
                            Globals.RParticularZGavetas += Gavetas[2] + " ";
                            Globals.RParticularZGavetas += Gavetas[4] + " ";
                            Globals.RParticularZGavetas += Gavetas[5] + " \n";

                            //Globals.RParticularZGavetas += formatear_bouche(1, Gavetas[1], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZGavetas += formatear_bouche(2, Gavetas[2], Globals.RParticularZMontos.Length) + "  ";
                            //Globals.RParticularZGavetas += formatear_bouche(3, Gavetas[4], Globals.RParticularZMontos.Length) + "  ";
                            if (Gavetas.Length > 5)
                            {
                                Globals.RParticularZGavetas += Gavetas[5] + " \n";
                                //Globals.RParticularZGavetas += formatear_bouche(4, Gavetas[5], Globals.RParticularZMontos.Length) + " \n             ";
                            }
                            else
                            {
                                Globals.RParticularZGavetas += " \n             ";
                            }

                        }

                    }

                    Globals.RParticularZDiscrepancias = "";
                    for (int i = indice + 1; i < msg.Count; i++)
                    {
                        var Discrepancia = msg[i].Split('~');
                        if (Discrepancia[1].Contains("Total"))
                        {
                            indice = i;
                            i = msg.Count;
                            Globals.RParticularZDiscrepancias += "Total       ";
                            Globals.RParticularZDiscrepancias += Discrepancia[3] + "  ";
                            if (Discrepancia.Length > 5)
                            {
                                Globals.RParticularZDiscrepancias += Discrepancia[5] + " \n";
                            }
                            else
                            {
                                Globals.RParticularZDiscrepancias += " \n";
                            }


                        }
                        else
                        {
                            Globals.RParticularZDiscrepancias += Discrepancia[0] + "  ";
                            Globals.RParticularZDiscrepancias += Discrepancia[1] + "  ";
                            Globals.RParticularZDiscrepancias += Discrepancia[2] + "  ";
                            Globals.RParticularZDiscrepancias += Discrepancia[3] + "  ";
                            Globals.RParticularZDiscrepancias += Discrepancia[4] + "  ";
                            if (Discrepancia.Length > 5)
                            {
                                Globals.RParticularZDiscrepancias += Discrepancia[5] + "  ";
                                Globals.RParticularZDiscrepancias += Discrepancia[6] + "  ";
                                Globals.RParticularZDiscrepancias += Discrepancia[7] + "  ";
                                Globals.RParticularZDiscrepancias += Discrepancia[8] + " \n";
                            }
                            else
                            {
                                Globals.RParticularZDiscrepancias += " \n";
                            }

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
            else
            {
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
            var inicio = controPeri.IniciarPago();
            var resPayout = controPeri.InicioPayout();
            var resHooper = controPeri.InicioHopper();
            if (inicio && resPayout && resHooper)
            {
                Globals.MaquinasActivadas = true;
                return "OK";
            }
            else
            {
                return "NOK";
            }
        }

        public string FinalizarCarga()
        {
            var ResfinPag = controPeri.FinalizarPago();
            var resPayout = controPeri.FinPayout();
            var resPayout2 = controPeri.FinHopper();
            if (ResfinPag && resPayout && resPayout2)
            {

                Globals.MaquinasActivadas = false;
                return "OK";
            }
            else
            {
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
                if (int.Parse(MonIngresadas.M1) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M1"]+"," + MonIngresadas.M1);
                }
                if (int.Parse(MonIngresadas.M2) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M2"] + "," + MonIngresadas.M2);
                }
                if (int.Parse(MonIngresadas.M3) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M3"] + "," + MonIngresadas.M3);
                }
                if (int.Parse(MonIngresadas.M4) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M4"] + "," + MonIngresadas.M4);
                }
                if (int.Parse(MonIngresadas.M5) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M5"] + "," + MonIngresadas.M5);
                }
                if (int.Parse(MonIngresadas.M6) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M6"] + "," + MonIngresadas.M6);
                }
                if (int.Parse(MonIngresadas.M7) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M7"] + "," + MonIngresadas.M7);
                }
                if (int.Parse(MonIngresadas.M8) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M8"] + "," + MonIngresadas.M8);
                }
                if (int.Parse(MonIngresadas.M9) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M9"] + "," + MonIngresadas.M9);
                }
                if (int.Parse(MonIngresadas.M10) > 0)
                {
                    data.Add(Globals._config["ValorMonedaReal:M10"] + "," + MonIngresadas.M10);
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
                    else if (respuestaserv.Contains("no existe"))
                    {
                        return "no existe";
                    }
                    else if (respuestaserv.Contains("no habilitada"))
                    {
                        return "no habilitada";
                    }
                    else if (respuestaserv.Contains("no cargado"))
                    {
                        return "no cargado";
                    }
                    else if (respuestaserv.Contains("previamente retirado"))
                    {
                        return "previamente retirado";
                    }
                    else if (respuestaserv.Contains("ya estaba inserta"))
                    {
                        return "ya estaba inserta";
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
                    else if (respuestaserv.Contains("no existe"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else if (respuestaserv.Contains("previamente retirada"))
                    {
                        return "previamente retirada";
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
                    else if (respuestaserv.Contains("no existe"))
                    {
                        return "Dispositivo Diferente";
                    }
                    else if (respuestaserv.Contains("previamente retirada"))
                    {
                        return "previamente retirada";
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

        public string DiscrepanciaB(Discrepancia Gavs)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(Gavs.GavOrigen);
            data.Add(Gavs.GavDestino);
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.AutoDiscrepanciaB, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.AutoDiscrepanciaB);
            if (respuesta == true)
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
            else {
                return "NOK";
            }
        }

        public string DiscrepanciaM(Discrepancia Gavs)
        {

            PipeClient2 pipeClient = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add(Gavs.GavOrigen);
            data.Add(Gavs.GavDestino);
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.AutoDiscrepanciaM, data);
            var respuesta = pipeClient.SendMessage(ServicioPago.Comandos.AutoDiscrepanciaM);
            if (respuesta == true)
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

        public async Task ImprecionCierreZAsync(DatosCierreZ Cierre)
        {

            try
            {
                StreamReader objReader = new StreamReader("./Documentos/cierreZ.txt");
                string sLine = "";
                string comprobante = "";
                Impresion comprobanteE = new Impresion();
                comprobanteE.document = "";

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
                comprobanteE.document = comprobante;
                var respuesta = await ImprimirComprobanteAsync(comprobanteE);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }
        }

        public async Task ImpresionReporteCierrez(UserToken user)
        {

            try
            {
                StreamReader objReader = new StreamReader("./Documentos/Impresion_reporteCierreZ.txt");
                string sLine = "";
                string comprobante = "";
                Impresion comprobanteE = new Impresion();
                comprobanteE.document = "";

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

                comprobante = comprobante.Replace("XXXIDU", user.IdUser.ToString());
                comprobante = comprobante.Replace("XXXUSR", user.Nombre);
                comprobante = comprobante.Replace("XXXFECHA", Globals.RParticularZmain[0]);
                comprobante = comprobante.Replace("XXXHORA", Globals.RParticularZmain[1]);
                comprobante = comprobante.Replace("XXXIDZETA", Globals.RParticularZmain[2]);
                comprobante = comprobante.Replace("XXXREP", Globals.RParticularZMontos + " ");
                comprobante = comprobante.Replace("XXXGAV", Globals.RParticularZGavetas + " ");
                comprobante = comprobante.Replace("XXXTOT", Globals.RParticularZDiscrepancias);
                comprobante = comprobante.Replace("XXXIDTRANS", Globals.IDTransaccion);

                comprobanteE.document = comprobante;
                var respuesta = await ImprimirComprobanteAsync(comprobanteE);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }
        }

        public async Task ImprecionComprobantePagoAsync(CargaDinero Carga)
        {

            try
            {
                StreamReader objReader = new StreamReader("./Documentos/comprobante_Ingre_Dinero.txt");
                string sLine = "";
                string comprobante = "";
                Impresion comprobanteE = new Impresion();
                comprobanteE.document = "";

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

                comprobante = comprobante.Replace("XXXRUT", Carga.Rut);
                comprobante = comprobante.Replace("dd/mm/aaaa hh:mm:ss PM", DateTime.Now.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Carga.IdTrans);
                comprobante = comprobante.Replace("M1XX", Carga.M1);
                comprobante = comprobante.Replace("M2XX", Carga.M2);
                comprobante = comprobante.Replace("M3XX", Carga.M3);
                comprobante = comprobante.Replace("M4XX", Carga.M4);
                comprobante = comprobante.Replace("M5XX", Carga.M5);
                comprobante = comprobante.Replace("M6XX", Carga.M6);
                comprobante = comprobante.Replace("M7XX", Carga.M7);
                comprobante = comprobante.Replace("M8XX", Carga.M8);
                comprobante = comprobante.Replace("M9XX", Carga.M9);
                comprobante = comprobante.Replace("M10XX", Carga.M10);
                comprobante = comprobante.Replace("B1XXX", Carga.B1);
                comprobante = comprobante.Replace("B2XXX", Carga.B2);
                comprobante = comprobante.Replace("B3XXX", Carga.B3);
                comprobante = comprobante.Replace("B4XXX", Carga.B4);
                comprobante = comprobante.Replace("B5XXX", Carga.B5);
                comprobante = comprobante.Replace("B6XXX", Carga.B6);
                comprobante = comprobante.Replace("B7XXX", Carga.B7);
                comprobante = comprobante.Replace("B8XXX", Carga.B8);
                comprobante = comprobante.Replace("B9XXX", Carga.B9);
                comprobante = comprobante.Replace("B10XXX", Carga.B10);


                comprobanteE.document = comprobante;
                var respuesta = await ImprimirComprobanteAsync(comprobanteE);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }
        }

        public async Task ImprimirGavAntesAhora(CargaDinero Carga, string Nombre)
        {
            SaldoMaquinaTrans();
            try
            {
                StreamReader objReader = new StreamReader("./Documentos/comprobante_AntesAhora.txt");
                string sLine = "";
                string comprobante = "";
                Impresion comprobanteE = new Impresion();
                comprobanteE.document = "";

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

                comprobante = comprobante.Replace("XXXTitulo", Nombre);
                comprobante = comprobante.Replace("XXXRUT", Carga.Rut);
                comprobante = comprobante.Replace("dd/mm/aaaa hh:mm:ss PM", DateTime.Now.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Carga.IdTrans);
                comprobante = comprobante.Replace("M1XX", Carga.M1 + "|" + Globals.SaldosMaquina.MR.M1);
                comprobante = comprobante.Replace("M1XX", Carga.M2 + "|" + Globals.SaldosMaquina.MR.M2);
                comprobante = comprobante.Replace("M1XX", Carga.M3 + "|" + Globals.SaldosMaquina.MR.M3);
                comprobante = comprobante.Replace("M1XX", Carga.M4 + "|" + Globals.SaldosMaquina.MR.M4);
                comprobante = comprobante.Replace("M1XX", Carga.M5 + "|" + Globals.SaldosMaquina.MR.M5);
                comprobante = comprobante.Replace("M1XX", Carga.M6 + "|" + Globals.SaldosMaquina.MR.M6);
                comprobante = comprobante.Replace("M1XX", Carga.M7 + "|" + Globals.SaldosMaquina.MR.M7);
                comprobante = comprobante.Replace("M1XX", Carga.M8 + "|" + Globals.SaldosMaquina.MR.M8);
                comprobante = comprobante.Replace("M1XX", Carga.M9 + "|" + Globals.SaldosMaquina.MR.M9);
                comprobante = comprobante.Replace("M1XX", Carga.M10 + "|" + Globals.SaldosMaquina.MR.M10);
                comprobante = comprobante.Replace("B1XXX", Carga.B1 + "|" + Globals.SaldosMaquina.BA.B1);
                comprobante = comprobante.Replace("B1XXX", Carga.B2 + "|" + Globals.SaldosMaquina.BA.B2);
                comprobante = comprobante.Replace("B1XXX", Carga.B3 + "|" + Globals.SaldosMaquina.BA.B3);
                comprobante = comprobante.Replace("B1XXX", Carga.B4 + "|" + Globals.SaldosMaquina.BA.B4);
                comprobante = comprobante.Replace("B1XXX", Carga.B5 + "|" + Globals.SaldosMaquina.BA.B5);
                comprobante = comprobante.Replace("B1XXX", Carga.B6 + "|" + Globals.SaldosMaquina.BA.B6);
                comprobante = comprobante.Replace("B1XXX", Carga.B7 + "|" + Globals.SaldosMaquina.BA.B7);
                comprobante = comprobante.Replace("B1XXX", Carga.B8 + "|" + Globals.SaldosMaquina.BA.B8);
                comprobante = comprobante.Replace("B1XXX", Carga.B9 + "|" + Globals.SaldosMaquina.BA.B9);
                comprobante = comprobante.Replace("B1XXX", Carga.B10 + "|" + Globals.SaldosMaquina.BA.B10);


                comprobanteE.document = comprobante;
                var respuesta = await ImprimirComprobanteAsync(comprobanteE);
            }
            catch (Exception ex)
            {
                Globals.log.Error("Ha ocurrido un error al leer el archivo de texto que contiene el ticket: Error " + ex.Message);
            }
        }

        public async Task ImprecionComprobantePagoAsyncR(CargaDinero Carga, string Nombre)
        {
            SaldoTransaccion();
            try
            {
                StreamReader objReader = new StreamReader("./Documentos/comprobante_AntesAhora.txt");
                string sLine = "";
                string comprobante = "";
                Impresion comprobanteE = new Impresion();
                comprobanteE.document = "";

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

                comprobante = comprobante.Replace("XXXTitulo", Nombre);
                comprobante = comprobante.Replace("XXXRUT", Carga.Rut);
                comprobante = comprobante.Replace("dd/mm/aaaa hh:mm:ss PM", DateTime.Now.ToString());
                comprobante = comprobante.Replace("XXXIDTRANS", Carga.IdTrans);
                comprobante = comprobante.Replace("M1XX", Carga.M1 + "|" + Globals.SaldosMaquina.MB.M1);
                comprobante = comprobante.Replace("M1XX", Carga.M2 + "|" + Globals.SaldosMaquina.MB.M2);
                comprobante = comprobante.Replace("M1XX", Carga.M3 + "|" + Globals.SaldosMaquina.MB.M3);
                comprobante = comprobante.Replace("M1XX", Carga.M4 + "|" + Globals.SaldosMaquina.MB.M4);
                comprobante = comprobante.Replace("M1XX", Carga.M5 + "|" + Globals.SaldosMaquina.MB.M5);
                comprobante = comprobante.Replace("M1XX", Carga.M6 + "|" + Globals.SaldosMaquina.MB.M6);
                comprobante = comprobante.Replace("M1XX", Carga.M7 + "|" + Globals.SaldosMaquina.MB.M7);
                comprobante = comprobante.Replace("M1XX", Carga.M8 + "|" + Globals.SaldosMaquina.MB.M8);
                comprobante = comprobante.Replace("M1XX", Carga.M9 + "|" + Globals.SaldosMaquina.MB.M9);
                comprobante = comprobante.Replace("M1XX", Carga.M10 + "|" + Globals.SaldosMaquina.MB.M10);
                comprobante = comprobante.Replace("B1XXX", Carga.B1 + "|" + Globals.SaldosMaquina.BR.B1);
                comprobante = comprobante.Replace("B1XXX", Carga.B2 + "|" + Globals.SaldosMaquina.BR.B2);
                comprobante = comprobante.Replace("B1XXX", Carga.B3 + "|" + Globals.SaldosMaquina.BR.B3);
                comprobante = comprobante.Replace("B1XXX", Carga.B4 + "|" + Globals.SaldosMaquina.BR.B4);
                comprobante = comprobante.Replace("B1XXX", Carga.B5 + "|" + Globals.SaldosMaquina.BR.B5);
                comprobante = comprobante.Replace("B1XXX", Carga.B6 + "|" + Globals.SaldosMaquina.BR.B6);
                comprobante = comprobante.Replace("B1XXX", Carga.B7 + "|" + Globals.SaldosMaquina.BR.B7);
                comprobante = comprobante.Replace("B1XXX", Carga.B8 + "|" + Globals.SaldosMaquina.BR.B8);
                comprobante = comprobante.Replace("B1XXX", Carga.B9 + "|" + Globals.SaldosMaquina.BR.B9);
                comprobante = comprobante.Replace("B1XXX", Carga.B10 + "|" + Globals.SaldosMaquina.BR.B10);

                comprobanteE.document = comprobante;
                var respuesta = await ImprimirComprobanteAsync(comprobanteE);
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
                if (cantidad[0] == Globals._config["lecturamaquina:B1"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B1 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B1 = cantidad[1];
                    }
                }

                if (cantidad[0] == Globals._config["lecturamaquina:B2"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B2 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B2 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B3"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B3 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B3 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B4"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B4 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B4 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B5"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B5 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B5 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B6"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B6 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B6 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B7"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B7 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B7 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B8"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B8 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B8 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B9"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B9 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B9 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["lecturamaquina:B10"])
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B10 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B10 = cantidad[1];
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
                if (cantidad[0] == Globals._config["ValorMonedaReal:M1"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M1 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M1 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M2"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M2 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M2 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M3"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M3 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M3 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M4"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M4= cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M4 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M5"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M5 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M5 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M6"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M6 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M1 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M7"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M7 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M7 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M8"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M8 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M8 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:M9"])
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M9 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M9 = cantidad[1];
                    }
                }
                if (cantidad[0] == Globals._config["ValorMonedaReal:10"])
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

            }
        }

        public async Task<bool> ImprimirComprobanteAsync(Impresion Documento)
        {
            var url = string.Format(Globals._config["Urls:Impresion"]);
            var json = JsonConvert.SerializeObject(Documento);
            var request = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, request))
            {
                var content = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
                return true;
            }


        }
    }
}
