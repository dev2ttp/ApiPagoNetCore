﻿using System;
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
                    if (item.Contains("1000,"))
                    {
                        var b1000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b1000)*1000);
                        Globals.SaldosMaquina.BA.B1000 = b1000;
                    }
                    if (item.Contains("2000,"))
                    {
                        var b2000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b2000)*2000);
                        Globals.SaldosMaquina.BA.B2000 = b2000;
                    }
                    if (item.Contains("5000,"))
                    {
                        var b5000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b5000)*5000);
                        Globals.SaldosMaquina.BA.B5000 = b5000;
                    }
                    if (item.Contains("10000,"))
                    {
                        var b10000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b10000)*10000);
                        Globals.SaldosMaquina.BA.B10000 = b10000;
                    }
                    if (item.Contains("20000,"))
                    {
                        var b20000 = item.Split(',')[1];
                        Globals.billetes.Add(int.Parse(b20000)*20000);
                        Globals.SaldosMaquina.BA.B20000 = b20000;
                    }
                }

                var Moneda = controladorPerisfericos.GetAllLevelsCoin(); //Globals.Servicio2ConsultarVuelto.Resultado.Data[1];
                var Monedas = Moneda.Split(';');
                foreach (var item in Monedas)
                {
                    if (item.Contains("10,"))
                    {
                        var b10 = item.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b10)*10);
                        Globals.SaldosMaquina.MR.M10 = b10;
                    }
                    if (item.Contains("50,"))
                    {
                        var b50 = item.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b50)*50);
                        Globals.SaldosMaquina.MR.M50 = b50;
                    }
                    if (item.Contains("100,"))
                    {
                        var b100 = item.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b100)*100);
                        Globals.SaldosMaquina.MR.M100 = b100;
                    }
                    if (item.Contains("500,"))
                    {
                        var b500 = item.Split(',')[1];
                        Globals.monedas.Add(int.Parse(b500)*500);
                        Globals.SaldosMaquina.MR.M500 = b500;
                    }
                }
            //}
            //else
            //{

            //}
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
            //if (/*Globals.monedas[0] < 4 || Globals.monedas[1] < 4 || */Globals.monedas[2] < 400 /*|| Globals.monedas[3] < 2*/)
            //{
            //    return false;
            //}
            if (Globals.monedas[2] < 400 )
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
