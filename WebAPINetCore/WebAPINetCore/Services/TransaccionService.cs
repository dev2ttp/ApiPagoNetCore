using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;
using WebAPINetCore.PipeServer;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Collections;

namespace WebAPINetCore.Services
{
    public class TransaccionService
    {
        public void InicioTransaccion()
        {

            Globals.data = new List<string>();
            Globals.data.Add("1");
            Globals.data.Add("9");
            Globals.Servicio1 = new PipeClient();
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.InicioTrans, Globals.data);
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.InicioTrans);
            if (respuesta)
            {
                Globals.IDTransaccion = "";
                Globals.RutUser = "";
                Globals.DVUser = "";
                if (Globals.Servicio1.Resultado.Data[0].Length > 0)
                {
                    Globals.IDTransaccion = Globals.Servicio1.Resultado.Data[0];
                    Globals.log.Info("Se acaba de iniciar una transaccion nueva ID: "+ Globals.IDTransaccion);
                }
                
            }
        }

        public void FinTransaccion()
        {
            Globals.data = new List<string>();
            Globals.data.Add("");
            Globals.Servicio1 = new PipeClient();
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.FinTrans, Globals.data);
            var respuesta = Globals.Servicio1.SendMessage(ServicioPago.Comandos.FinTrans);
            if (respuesta)
            {
                if (Globals.Servicio1.Resultado.Data[0].Length > 0)
                {
                    //Globals.IDTransaccion = "";
                    //Globals.RutUser = "";
                    //Globals.DVUser = "";
                }
                Globals.log.Info("Se acaba de Finalizar una transaccion de ID: " + Globals.IDTransaccion);
            }
        }
    }
}

