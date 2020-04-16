using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;
using WebAPINetCore.Models;

namespace WebAPINetCore.Services
{
    public class PermitirVueltoService
    {

        public PermitirVueltoService() { }
        ControladorPerisfericos controladorPerisfericos = new ControladorPerisfericos();
        public void ObteneDineroMaquina()
        {
            //var vuelta = false ;
            //do
            //{
            //    Globals.data = new List<string>();
            //    Globals.data.Add("");
            //    Globals.Servicio2ConsultarVuelto = new PipeClient2();
            //    Globals.Servicio2ConsultarVuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DineroEnMaquina, Globals.data);
            //    vuelta = Globals.Servicio2ConsultarVuelto.SendMessage(ServicioPago.Comandos.DineroEnMaquina);
            //} while (Globals.Servicio2ConsultarVuelto.Resultado.Data[0].Contains("NOK"));

            //if (vuelta)
            //{
            Globals.SaldosMaquina = new SaldoGaveta();
            var billete = controladorPerisfericos.GetAllLevelsNota(); //Globals.Servicio2ConsultarVuelto.Resultado.Data[0];
            var billetes = billete.Split(';');
            foreach (var item in billetes)
            {
                if (item.Contains(Globals._config["lecturamaquina:B1"] + ","))
                {
                    var b1 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b1) * int.Parse(Globals._config["lecturamaquina:B1"]));
                    Globals.SaldosMaquina.BA.B1 = b1;
                }
                if (item.Contains(Globals._config["lecturamaquina:B2"] + ","))
                {
                    var b2 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b2) * int.Parse(Globals._config["lecturamaquina:B2"]));
                    Globals.SaldosMaquina.BA.B2 = b2;
                }
                if (item.Contains(Globals._config["lecturamaquina:B3"] + ","))
                {
                    var b3 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b3) * int.Parse(Globals._config["lecturamaquina:B3"]));
                    Globals.SaldosMaquina.BA.B3 = b3;
                }
                if (item.Contains(Globals._config["lecturamaquina:B4"] + ","))
                {
                    var b4 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b4) * int.Parse(Globals._config["lecturamaquina:B4"]));
                    Globals.SaldosMaquina.BA.B4 = b4;
                }
                if (item.Contains(Globals._config["lecturamaquina:B5"] + ","))
                {
                    var b5 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b5) * int.Parse(Globals._config["lecturamaquina:B5"]));
                    Globals.SaldosMaquina.BA.B5 = b5;
                }
                if (item.Contains(Globals._config["lecturamaquina:B6"] + ","))
                {
                    var b6 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b6) * int.Parse(Globals._config["lecturamaquina:B6"]));
                    Globals.SaldosMaquina.BA.B6 = b6;
                }
                if (item.Contains(Globals._config["lecturamaquina:B7"] + ","))
                {
                    var b7 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b7) * int.Parse(Globals._config["lecturamaquina:B7"]));
                    Globals.SaldosMaquina.BA.B7 = b7;
                }
                if (item.Contains(Globals._config["lecturamaquina:B8"] + ","))
                {
                    var b8 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b8) * int.Parse(Globals._config["lecturamaquina:B8"]));
                    Globals.SaldosMaquina.BA.B8 = b8;
                }
                if (item.Contains(Globals._config["lecturamaquina:B9"] + ","))
                {
                    var b9 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b9) * int.Parse(Globals._config["lecturamaquina:B9"]));
                    Globals.SaldosMaquina.BA.B9 = b9;
                }
                if (item.Contains(Globals._config["lecturamaquina:B10"] + ","))
                {
                    var b10 = item.Split(',')[1];
                    Globals.billetes.Add(int.Parse(b10) * int.Parse(Globals._config["lecturamaquina:B10"]));
                    Globals.SaldosMaquina.BA.B10 = b10;
                }

            }

            var Moneda = controladorPerisfericos.GetAllLevelsCoin(); //Globals.Servicio2ConsultarVuelto.Resultado.Data[1];
            var Monedas = Moneda.Split(';');
            foreach (var item in Monedas)
            {
                if (item.Contains(Globals._config["lecturamaquina:M1"]+","))
                {
                    var moneda1 = item.Replace(Globals._config["ValorMonedaReal:M1"] +",", Globals._config["lecturamaquina:M1"]+",");
                    var  m1= moneda1.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m1) * int.Parse(Globals._config["ValorMonedaReal:M1"]));
                    Globals.SaldosMaquina.MR.M1 = m1;
                }
                if (item.Contains(Globals._config["lecturamaquina:M2"] + ","))
                {
                    var moneda2 = item.Replace(Globals._config["ValorMonedaReal:M2"] + ",", Globals._config["lecturamaquina:M2"] + ",");
                    var m2 = moneda2.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m2) * int.Parse(Globals._config["ValorMonedaReal:M2"]));
                    Globals.SaldosMaquina.MR.M2 = m2;
                }
                if (item.Contains(Globals._config["lecturamaquina:M3"] + ","))
                {
                    var moneda3 = item.Replace(Globals._config["ValorMonedaReal:M3"] + ",", Globals._config["lecturamaquina:M3"] + ",");
                    var m3 = moneda3.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m3) * int.Parse(Globals._config["ValorMonedaReal:M3"]));
                    Globals.SaldosMaquina.MR.M3 = m3;
                }
                if (item.Contains(Globals._config["lecturamaquina:M4"] + ","))
                {
                    var moneda4 = item.Replace(Globals._config["ValorMonedaReal:M4"] + ",", Globals._config["lecturamaquina:M4"] + ",");
                    var m4 = moneda4.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m4) * int.Parse(Globals._config["ValorMonedaReal:M4"]));
                    Globals.SaldosMaquina.MR.M4 = m4;
                }
                if (item.Contains(Globals._config["lecturamaquina:M5"] + ","))
                {
                    var moneda5 = item.Replace(Globals._config["ValorMonedaReal:M5"] + ",", Globals._config["lecturamaquina:M5"] + ",");
                    var m5 = moneda5.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m5) * int.Parse(Globals._config["ValorMonedaReal:M5"]));
                    Globals.SaldosMaquina.MR.M5 = m5;
                }
                if (item.Contains(Globals._config["lecturamaquina:M6"] + ","))
                {
                    var moneda6 = item.Replace(Globals._config["ValorMonedaReal:M6"] + ",", Globals._config["lecturamaquina:M6"] + ",");
                    var m6 = moneda6.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m6) * int.Parse(Globals._config["ValorMonedaReal:M6"]));
                    Globals.SaldosMaquina.MR.M6 = m6;
                }
                if (item.Contains(Globals._config["lecturamaquina:M7"] + ","))
                {
                    var moneda7 = item.Replace(Globals._config["ValorMonedaReal:M7"] + ",", Globals._config["lecturamaquina:M7"] + ",");
                    var m7 = moneda7.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m7) * int.Parse(Globals._config["ValorMonedaReal:M7"]));
                    Globals.SaldosMaquina.MR.M7 = m7;
                }
                if (item.Contains(Globals._config["lecturamaquina:M8"] + ","))
                {
                    var moneda8 = item.Replace(Globals._config["ValorMonedaReal:M8"] + ",", Globals._config["lecturamaquina:M8"] + ",");
                    var m8 = moneda8.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m8) * int.Parse(Globals._config["ValorMonedaReal:M8"]));
                    Globals.SaldosMaquina.MR.M8 = m8;
                }
                if (item.Contains(Globals._config["lecturamaquina:M9"] + ","))
                {
                    var moneda9 = item.Replace(Globals._config["ValorMonedaReal:M9"] + ",", Globals._config["lecturamaquina:M9"] + ",");
                    var m9 = moneda9.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m9) * int.Parse(Globals._config["ValorMonedaReal:M9"]));
                    Globals.SaldosMaquina.MR.M9 = m9;
                }
                if (item.Contains(Globals._config["lecturamaquina:M10"] + ","))
                {
                    var moneda10 = item.Replace(Globals._config["ValorMonedaReal:M10"] + ",", Globals._config["lecturamaquina:M10"] + ",");
                    var m10 = moneda10.Split(',')[1];
                    Globals.monedas.Add(int.Parse(m10) * int.Parse(Globals._config["ValorMonedaReal:M10"]));
                    Globals.SaldosMaquina.MR.M10 = m10;
                }

            }
        }

        public bool CalcularVueltoPosible(float MontoApagar)
        {
            int dineroTotal = 0;
            Globals.billetes = new List<int>();
            Globals.monedas = new List<int>();
            ObteneDineroMaquina();


            var moneda = 0;
            var billete = 0;
            foreach (var item in Globals.billetes)
            {
                billete += item;
            }

            if (Globals._config["Decimal:billete"] == "true")
            {
                billete = billete / int.Parse(Globals._config["Decimal:divbillete"]);
            }
            dineroTotal += billete;


            foreach (var item in Globals.monedas)
            {
                moneda += item;
            }

            if (Globals._config["Decimal:moneda"] == "true")
            {
                moneda = moneda / int.Parse(Globals._config["Decimal:divmoneda"]);
            }
            dineroTotal += moneda;
            //monedas
            //if (/*Globals.monedas[0] < 4 || Globals.monedas[1] < 4 || */Globals.monedas[2] < 400 /*|| Globals.monedas[3] < 2*/)
            //{
            //    return false;
            //}
            if (Globals.monedas[2] < int.Parse(Globals._config["PermitirVuelto:Monedas"]))
            {
                return false;
            }
            if (MontoApagar > dineroTotal)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
