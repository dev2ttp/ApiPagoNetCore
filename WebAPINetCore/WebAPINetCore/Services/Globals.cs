using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public static class Globals
    {

        //Var para realizar llamados de servicio de mensajeria PIPE
        public static PipeClient Servicio1 = new PipeClient();
        public static PipeClient2 Servicio2 = new PipeClient2();
        public static ServicioPago servicio = new ServicioPago();
        public static List<string> data = new List<string>();


        // VAriable para hacer Log de la API
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
