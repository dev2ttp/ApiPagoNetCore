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
                if (int.Parse(MonIngresadas.M1) > 0)
                {
                    data.Add("1," + MonIngresadas.M1);
                }
                if (int.Parse(MonIngresadas.M5) > 0)
                {
                    data.Add("5," + MonIngresadas.M5);
                }
                if (int.Parse(MonIngresadas.M10) > 0)
                {
                    data.Add("10," + MonIngresadas.M10);
                }
                if (int.Parse(MonIngresadas.M25) > 0)
                {
                    data.Add("25," + MonIngresadas.M25);
                }

                if (int.Parse(MonIngresadas.M100) > 0)
                {
                    data.Add("100," + MonIngresadas.M100);
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
                comprobante = comprobante.Replace("M1XX", Carga.M10);
                comprobante = comprobante.Replace("M50X", Carga.M50);
                comprobante = comprobante.Replace("M10X", Carga.M100);
                comprobante = comprobante.Replace("M500", Carga.M500);
                comprobante = comprobante.Replace("B1XXX", Carga.B1000);
                comprobante = comprobante.Replace("B2XXX", Carga.B2000);
                comprobante = comprobante.Replace("B5000", Carga.B5000);
                comprobante = comprobante.Replace("B100XX", Carga.B10000);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000);

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
                comprobante = comprobante.Replace("M1XX", Carga.M10 + "|" + Globals.SaldosMaquina.MR.M1);
                comprobante = comprobante.Replace("M50X", Carga.M50 + "|" + Globals.SaldosMaquina.MR.M5);
                comprobante = comprobante.Replace("M10X", Carga.M100 + "|" + Globals.SaldosMaquina.MR.M10);
                comprobante = comprobante.Replace("M500", Carga.M500 + "|" + Globals.SaldosMaquina.MR.M25);
                comprobante = comprobante.Replace("M500", Carga.M500 + "|" + Globals.SaldosMaquina.MR.M100);
                comprobante = comprobante.Replace("B1XXX", Carga.B1000 + "|" + Globals.SaldosMaquina.BA.B1);
                comprobante = comprobante.Replace("B2XXX", Carga.B2000 + "|" + Globals.SaldosMaquina.BA.B2);
                comprobante = comprobante.Replace("B5000", Carga.B5000 + "|" + Globals.SaldosMaquina.BA.B5);
                comprobante = comprobante.Replace("B100XX", Carga.B10000 + "|" + Globals.SaldosMaquina.BA.B10);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.SaldosMaquina.BA.B20);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.SaldosMaquina.BA.B50);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.SaldosMaquina.BA.B100);

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
                comprobante = comprobante.Replace("M1XX", Carga.M10 + "|" + Globals.Saldos.MB.M1);
                comprobante = comprobante.Replace("M50X", Carga.M50 + "|" + Globals.Saldos.MB.M5);
                comprobante = comprobante.Replace("M10X", Carga.M100 + "|" + Globals.Saldos.MB.M10);
                comprobante = comprobante.Replace("M500", Carga.M500 + "|" + Globals.Saldos.MB.M25);
                comprobante = comprobante.Replace("M500", Carga.M500 + "|" + Globals.Saldos.MB.M100);
                comprobante = comprobante.Replace("B1XXX", Carga.B1000 + "|" + Globals.Saldos.BR.B1);
                comprobante = comprobante.Replace("B2XXX", Carga.B2000 + "|" + Globals.Saldos.BR.B2);
                comprobante = comprobante.Replace("B5000", Carga.B5000 + "|" + Globals.Saldos.BR.B5);
                comprobante = comprobante.Replace("B100XX", Carga.B10000 + "|" + Globals.Saldos.BR.B10);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.Saldos.BR.B20);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.Saldos.BR.B50);
                comprobante = comprobante.Replace("B20XXX", Carga.B20000 + "|" + Globals.Saldos.BR.B100);

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
                //| 100~BA~100~1000~1200~2,0; 20,0; 10,1; 1,3; 5,4
                //| 101~BR~100~1000~1200~20,4; 1,0; 2,0; 5,1; 10,2

                var cantidad = Billete.Split(",");
                if (cantidad[0] == "1,")
                {
                    if (Tipo == "BA")
                    {

                        Globals.Saldos.BA.B1 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B1= cantidad[1];
                    }
                }

                if (cantidad[0] == "2,")
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

                if (cantidad[0] == "5,")
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

                if (cantidad[0] == "10,")
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

                if (cantidad[0] == "20,")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B20 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B20 = cantidad[1];
                    }
                }

                if (cantidad[0] == "50,")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B50 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B50 = cantidad[1];
                    }
                }
                if (cantidad[0] == "100,")
                {
                    if (Tipo == "BA")
                    {
                        Globals.Saldos.BA.B100 = cantidad[1];
                    }
                    if (Tipo == "BR")
                    {
                        Globals.Saldos.BR.B100 = cantidad[1];
                    }
                }

            }
        }

        public void ObtenerMonedsas(string saldo, string Tipo)
        {
            //| 103~MB~100~1000~1200~10,0; 2,0; 5,1; 1,2; 100,2
            //| 102~MR~100~1000~1200~5,11; 10,0; 2,0; 1,3; 100,5
            var denominacion = saldo.Split(";");
            foreach (var Moneda in denominacion)
            {
                var cantidad = Moneda.Split(",");
                if (cantidad[0] == "1,")
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

                if (cantidad[0] == "5,")
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

                if (cantidad[0] == "10,")
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

                if (cantidad[0] == "25,")
                {
                    if (Tipo == "MB")
                    {
                        Globals.Saldos.MB.M25 = cantidad[1];
                    }
                    if (Tipo == "MR")
                    {
                        Globals.Saldos.MR.M25 = cantidad[1];
                    }
                }

                if (cantidad[0] == "100,")
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
