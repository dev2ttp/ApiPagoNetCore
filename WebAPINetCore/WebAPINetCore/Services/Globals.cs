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

        //Var para realizar llamados de servicio de mensajeria PIPE
        public static PipeClient Servicio1 = new PipeClient();
        public static PipeClient2 Servicio2 = new PipeClient2();
        public static PipeClient2 Servicio2Inicio = new PipeClient2();
        public static PipeClient2 Servicio2Pago = new PipeClient2();
        public static PipeClient2 Servicio2Vuelto = new PipeClient2();
        public static PipeClient2 Servicio2Cancelar = new PipeClient2();
        public static ServicioPago servicio = new ServicioPago();
        public static List<string> data = new List<string>();

        public static CancelarPago EstadodeCancelacion = new CancelarPago();//variable para conocer estado de cancelacion


        // VAriable para hacer Log de la API
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
