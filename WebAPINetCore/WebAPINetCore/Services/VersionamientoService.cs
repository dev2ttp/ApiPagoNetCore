using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Services
{
    public class VersionamientoService
    {
        public VersionamientoService() { }

        public bool EnviarInicioVersion(string Version, string APP)
        {
            PipeClient pipeClient = new PipeClient();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();
            var cambiover = false;

            if (APP == "APP_PAGO" && Version != Globals.VAppFPago)
            {
                Globals.VAppFPago = Version;
                cambiover = true;
            }

            if (APP == "APP_EFE" && Version != Globals.VAppFEfe)
            {
                Globals.VAppFEfe = Version;
                cambiover = true;
            }

            if (APP == "APP_TES" && Version != Globals.VAppFTes)
            {
                Globals.VAppFTes = Version;
                cambiover = true;
            }

            if (APP == "SRVLOC2" && Version != Globals.VAppSV2)
            {
                Globals.VAppSV2 = Version;
                cambiover = true;
            }
            if (APP == "APILOC" )
            {
                cambiover = true;
            }
            if (cambiover == true)
            {
                data.Add("0");
                data.Add(Version);
                data.Add(APP);
                var resultado = false;
                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.APP_FRTINI, data);
                resultado = pipeClient.SendMessage(ServicioPago.Comandos.APP_FRTINI);
                if (resultado == true)
                {
                    var respuesta = pipeClient.Resultado.Data;
                }
                return resultado;
            }
            else
            {
                return true;
            }
            
        }

        public bool EnviarFinVersion()
        {
            PipeClient pipeClient = new PipeClient();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add("0");
            var resultado = false;
            pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.APP_FRTFIN, data);
            resultado = pipeClient.SendMessage(ServicioPago.Comandos.APP_FRTFIN);
            if (resultado == true)
            {
                var respuesta = pipeClient.Resultado.Data;
            }
            return resultado;
        }

        public void VersionServicioPago2()
        {
            PipeClient2 pipeClient2 = new PipeClient2();
            ServicioPago servicio = new ServicioPago();
            List<string> data = new List<string>();

            data.Add("");
            var resultado = false;
            pipeClient2.Message = servicio.BuildMessage(ServicioPago.Comandos.APP_FRTINI, data);
            resultado = pipeClient2.SendMessage(ServicioPago.Comandos.APP_FRTINI);
            if (resultado == true)
            {
                EnviarInicioVersion(pipeClient2.Resultado.Data[0], "SRVLOC2");
            }
        }
    }
}
