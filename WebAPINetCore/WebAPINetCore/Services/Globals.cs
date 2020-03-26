using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public static PipeClient2 EstadoSalud = new PipeClient2();
        public static ServicioPago servicio = new ServicioPago();
        public static List<string> data = new List<string>();

        public static CancelarPago EstadodeCancelacion = new CancelarPago();//variable para conocer estado de cancelacion

        public static PipeClient2 ServInicioPayou = new PipeClient2();
        public static PipeClient2 ServInicioHopper = new PipeClient2();
        public static PipeClient2 ServFinPayou = new PipeClient2();
        public static PipeClient2 ServFinHopper = new PipeClient2();
        public static PipeClient2 ServNivelNota = new PipeClient2();
        public static PipeClient2 ServNivelMoneda = new PipeClient2();


        // VAriable para hacer Log de la API
        //public static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        // variable impresa
        public static bool ComprobanteImpreso;
        public static int ComprobanteImpresoContador = 0;
        public static bool ComprobanteImpresoVuelto;
        public static int ImpresoraMontoPagar = 0;
        public static int ImpresoraMontoIngresado = 0;
        public static int ImpresoraMontoVueltoEntregar = 0;
        public static int ImpresoraMontoEntregado = 0;

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
        public static string EstadoDeSaludMaquina { get; set; }// status sin descifrar de la maquina
        public static bool MaquinasActivadas { get; set; }
        public static bool HayVuelto { get; set; }
        public static bool NivelBloqueo { get; set; }
        public static bool PagoCompleto { get; set; }
        public static bool PagoFinalizado { get; set; }// Si el pago se ha completado se vuelve true para no enviar datos de mas  
        public static string dineroIngresado { get; set; }
        public static List<int> billetes { get; set; }
        public static List<int> monedas { get; set; }

        public static EstadosDeSalud SaludMaquina  { get; set; }//Status descifrados de salud de la maquina
        public static bool BloqueoTransbank { get; set; }// status saber si se bloquea transbank
        public static bool BloqueoEfectivo { get; set; }// status saber si se bloquea efectivo



        //Consulta de vuelto
        public static bool VueltoPermitido { get; set; }
        public static bool DineroIngresadoSolicitado { get; set; }// se utiliza para saber si ya se pidio el dinero final ingresado ( para hacerlo una sola vez)
        public static int VueltosSinIniciar { get; set; }//utilizado para contar las veces que se envia un vuelto pero la maquina no da respuesta



        // Tesoreria
        //Saldos
        public static SaldoGaveta Saldos { get; set; } // utilizado para llevar los saldos de toda la maquina
        public static SaldoGaveta SaldosMaquina { get; set; } // utilizado para llevar los saldos de toda la maquina

        //Cieere Z
        public static DatosCierreZ Cierrez = new DatosCierreZ(); // Datos Devueltos Al realizar un cierre Z

        // IDs Fechas Reporte Cieere Z
        public static IDReportesCierre fechasIDs = new IDReportesCierre();//IDs y fechas de los reportes de cierre Z

        // Datos Cierre Cieere Z
        public static ReportesCierre DatosCierre = new ReportesCierre();//IDs y fechas de los reportes de cierre Z

        // variables reporte cierre Z
        public static string RParticularZMontos { get; set; }
        public static string RParticularZGavetas { get; set; }
        public static string RParticularZDiscrepancias { get; set; }
        public static string[] RParticularZmain { get; set; }
        


    }
}
