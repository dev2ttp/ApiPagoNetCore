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
                foreach (var item in billetes)
                {
                    if (item.Contains("1000,"))
                    {
                        var b1000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b1000)*1000);
                    }
                    if (item.Contains("2000,"))
                    {
                        var b2000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b2000)*2000);
                    }
                    if (item.Contains("5000,"))
                    {
                        var b5000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b5000)*5000);
                    }
                    if (item.Contains("10000,"))
                    {
                        var b10000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b10000)*10000);
                    }
                    if (item.Contains("20000,"))
                    {
                        var b20000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b20000));
                    }
                }

                var Moneda = Globals.Servicio2ConsultarVuelto.Resultado.Data[1];
                var Monedas = Moneda.Split(';');
                foreach (var item in Monedas)
                {
                    if (item.Contains("0,10,"))
                    {
                        var moneda10 = item.Replace("0,10,", "10,");
                        var b10 = moneda10.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b10) * 10);
                    }
                    if (item.Contains("0,50,"))
                    {
                        var moneda50 = item.Replace("0,50,", "50,");
                        var b50 = moneda50.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b50) * 50);
                    }
                    if (item.Contains("1,00,"))
                    {
                        var moneda100 = item.Replace("1,00,", "100,");
                        var b100 = moneda100.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b100) * 100);
                    }
                    if (item.Contains("5,00,"))
                    {
                        var moneda500 = item.Replace("5,00,,", "500,");
                        var b500 = moneda500.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b500) * 500);
                    }
                }
            }
            else
            {

            }
        }

        public bool CalcularVueltoPosible(int MontoApagar)
        {
            int dineroTotal = 0;
            Globals.billetes = new List<int>();
            Globals.monedas = new List<int>();
            ObteneDineroMaquina();

            foreach (var item in Globals.billetes)
            {
                dineroTotal += item;
            }

            foreach (var item in Globals.monedas)
            {
                dineroTotal += item;
            }
            //monedas
            if (/*Globals.monedas[0] < 4 || Globals.monedas[1] < 4 || */Globals.monedas[2] < 400 /*|| Globals.monedas[3] < 2*/)
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
