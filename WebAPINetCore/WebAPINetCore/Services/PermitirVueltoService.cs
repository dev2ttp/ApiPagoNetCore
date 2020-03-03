using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class PermitirVueltoService
    {

        public PermitirVueltoService() { }

        public void ObteneDineroMaquina()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio2ConsultarVuelto = new PipeClient2();
            Globals.Servicio2ConsultarVuelto.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.DineroEnMaquina, Globals.data);
            var vuelta = Globals.Servicio2ConsultarVuelto.SendMessage(ServicioPago.Comandos.DineroEnMaquina);
            if (vuelta)
            {
                var billete = Globals.Servicio2ConsultarVuelto.Resultado.Data[0];
                var billetes = billete.Split(';');
                //0;1,3;2,0;5,4;10,1;20,0;50,0;100,0|;0,01,3;0,05,2;0,10,0;0,25,0;1,00,5EF0B@
                foreach (var item in billetes)
                {
                    if (item.Contains("1,"))
                    {
                        var b1 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b1));
                        Globals.SaldosMaquina.BA.B1 = b1;
                    }
                    if (item.Contains("2,"))
                    {
                        var b2 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b2)*2);
                        Globals.SaldosMaquina.BA.B2 = b2;
                    }
                    if (item.Contains("5,"))
                    {
                        var b5 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b5)*5);
                        Globals.SaldosMaquina.BA.B5 = b5;
                    }
                    if (item.Contains("10,"))
                    {
                        var b10 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b10)*10);
                        Globals.SaldosMaquina.BA.B10 = b10;
                    }
                    if (item.Contains("20,"))
                    {
                        var b20 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b20)*20);
                        Globals.SaldosMaquina.BA.B20 = b20;
                    }
                    if (item.Contains("50,"))
                    {
                        var b50 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b50)*50);
                        Globals.SaldosMaquina.BA.B50 = b50;
                    }
                    if (item.Contains("100,"))
                    {
                        var b100= item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b100)*100);
                        Globals.SaldosMaquina.BA.B100 = b100;
                    }
                }

                var Moneda = Globals.Servicio2ConsultarVuelto.Resultado.Data[1];
                var Monedas = Moneda.Split(';');
                foreach (var item in Monedas)
                {
                    if (item.Contains("0,01,"))
                    {
                        var moneda10 = item.Replace("0,01,", "1,");
                        var m1 = moneda10.Split(',')[1];
                        Globals.monedas.Add(int.Parse(m1));
                        Globals.SaldosMaquina.MR.M1 = m1;
                    }
                    if (item.Contains("0,05,"))
                    {
                        var moneda5 = item.Replace("0,05,", "5,");
                        var b5 = moneda5.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b5) * 5);
                        Globals.SaldosMaquina.MR.M5 = b5;
                    }
                    if (item.Contains("0,10,"))
                    {
                        var moneda10 = item.Replace("0,10,", "10,");
                        var b10 = moneda10.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b10) * 10);
                        Globals.SaldosMaquina.MR.M10 = b10;
                    }
                    if (item.Contains("0,25,"))
                    {
                        var moneda25 = item.Replace("0,25,", "25,");
                        var b25= moneda25.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b25) * 25);
                        Globals.SaldosMaquina.MR.M25 = b25;
                    }

                    if (item.Contains("1,00,"))
                    {
                        var moneda100 = item.Replace("1,00,,", "100,");
                        var b100 = moneda100.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b100) * 100);
                        Globals.SaldosMaquina.MR.M100 = b100;
                    }
                }
            }
            else
            {

            }
        }

        public bool CalcularVueltoPosible(int MontoApagar)
        {
            double dineroTotal = 0;
            Globals.billetes = new List<int>();
            Globals.monedas = new List<int>();
            ObteneDineroMaquina();

            foreach (var item in Globals.billetes)
            {
                dineroTotal += item;
            }
            var monedas = 0.0;
            foreach (var item in Globals.monedas)
            {
                monedas = item;
            }
            dineroTotal += (monedas/100);
            //monedas
            //if (/*Globals.monedas[0] < 4 || Globals.monedas[1] < 4 || */Globals.monedas[2] < 400 /*|| Globals.monedas[3] < 2*/)
            //{
            //    return false;
            //}
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
