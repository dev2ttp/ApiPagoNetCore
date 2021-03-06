﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public static class Globals
    {

        public static  IConfiguration _config;// configuraciones


        //Var para realizar llamados de servicio de mensajeria PIPE
        public static PipeClient Servicio1 = new PipeClient();
        public static PipeClient2 Servicio2 = new PipeClient2();
        public static PipeClient2 Servicio2Inicio = new PipeClient2();
        public static PipeClient2 Servicio2Pago = new PipeClient2();
        public static PipeClient2 Servicio2Vuelto = new PipeClient2();
        public static PipeClient2 Servicio2Cancelar = new PipeClient2();
        public static PipeClient2 Servicio2ConsultarVuelto = new PipeClient2();
        public static PipeClient2 Servicio2ConsultarDevolucionM = new PipeClient2();
        public static ServicioPago servicio = new ServicioPago();
        public static List<string> data = new List<string>();

        public static CancelarPago EstadodeCancelacion = new CancelarPago();//variable para conocer estado de cancelacion


        // VAriable para hacer Log de la API
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // variable impresa
        public static bool ComprobanteImpreso;
        public static int ComprobanteImpresoContador = 0;
        public static bool ComprobanteImpresoVuelto;

        // Variables de estado
        public static EstadoVuelto Vuelto;
        public static EstadoPago Pago;
        public static EstadoPago RespaldoParaVuelto;

        // datos usuario 
        public static string RutUser { get; set; }
        public static string DVUser { get; set; }
        public static string IDTransaccion { get; set; }

        //cancelacion de timers
        public static bool TimersVueltoCancel;
        public static bool VueltoUnaVEz;
        public static bool DandoVuelto;

        // estado de maquina y de servicio
        public static string EstadoDeSaludMaquina { get; set; }
        public static bool MaquinasActivadas { get; set; }
        public static bool HayVuelto { get; set; }
        public static bool NivelBloqueo { get; set; }
        public static bool PagoCompleto { get; set; }
        public static bool PagoFinalizado { get; set; }// Si el pago se ha completado se vuelve true para no enviar datos de mas  
        public static string dineroIngresado { get; set; }
        public static List<int> billetes { get; set; }
        public static List<int> monedas { get; set; }


        //Consulta de vuelto
        public static bool VueltoPermitido { get; set; }
    }
}
