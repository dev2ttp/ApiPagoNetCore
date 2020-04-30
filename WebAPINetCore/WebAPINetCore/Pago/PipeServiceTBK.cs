using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;

namespace WebAPINetCore.Pago
{
    public class PipeServiceTBK
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        public string Message { get; set; }
        private string PipeName = string.Format(Globals._config["Pipeserve:Pipe1"]);
        private string PipeName2 = string.Format(Globals._config["Pipeserve:Pipe2"]);
        public bool pipeTransaction(string transaccion, string argumento)
        {
            string msg = string.Format("{0}~{1}", transaccion, argumento);
            string resp = string.Empty;
            try
            {
                log.Info("Q: Pipe Validar Huella ->  Transaccion: " + transaccion + "; Data: " + argumento);
                using (var pipeClient = new NamedPipeClientStream(".", string.Format(Globals._config["Pipeserve:Pipe1"]), PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
                {
                    byte[] arreglo = Encoding.ASCII.GetBytes(msg);
                    pipeClient.Connect(10000);
                    pipeClient.Write(arreglo, 0, arreglo.Length);
                    pipeClient.WaitForPipeDrain();
                    var stream = new StreamReader(pipeClient);
                    resp = stream.ReadToEnd();
                    pipeClient.Close();
                }

                if (resp.Equals("OK~OK"))
                {
                    log.Info("R: Pipe Validar Huella: " + resp);
                    return true;
                }
                else
                {
                    log.Error("E: Pipe Validar Huella: " + resp);
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Error("E: Pipe Validar Huella: " + e.ToString());
                return false;
            }

        }

        public string ComponentPipePrintVoucher(string voucher)
        {
            string resp = string.Empty;
            string[] result;
            try
            {
                log.Info("Q: Pipe Print Voucher TBK ->  Pipe name: " + PipeName2 + "; Data: " + voucher);
                using (var pipeClient = new NamedPipeClientStream(".", PipeName2, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
                {
                    byte[] arreglo = Encoding.ASCII.GetBytes("112~" + voucher);
                    byte[] respuesta = new byte[512];

                    Console.WriteLine("Connecting to server...\n");
                    pipeClient.Connect();
                    pipeClient.Write(arreglo, 0, arreglo.Length);
                    pipeClient.WaitForPipeDrain();
                    var stream = new StreamReader(pipeClient);
                    resp = stream.ReadToEnd();

                    result = resp.Split('~');
                    Console.Write(pipeClient);
                    log.Info("R: Pipe Print Voucher TBK: " + resp);
                    pipeClient.Close();
                }
                return resp;
            }
            catch (Exception e)
            {
                log.Error("E: Pipe Print Voucher TBK: " + e.ToString());
                return resp;
            }
        }

        public string ComponentPipe(string argumento)
        {
            string resp = string.Empty;

            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
            {
                byte[] arreglo = Encoding.ASCII.GetBytes("101~pdf~" + argumento);

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();
                pipeClient.Write(arreglo, 0, arreglo.Length);
                pipeClient.WaitForPipeDrain();
                var stream = new StreamReader(pipeClient);
                resp = stream.ReadToEnd();

                Console.Write(pipeClient);
                pipeClient.Close();
            }
            return resp;
        }

        public string Print(string argumento)
        {
            string resp = string.Empty;

            using (var pipeClient = new NamedPipeClientStream(".", PipeName2, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
            {
                byte[] arreglo = Encoding.ASCII.GetBytes("108~pdf~" + argumento);

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();
                pipeClient.Write(arreglo, 0, arreglo.Length);
                pipeClient.WaitForPipeDrain();
                var stream = new StreamReader(pipeClient);
                resp = stream.ReadToEnd();

                Console.Write(pipeClient);
                pipeClient.Close();
            }
            return resp;
        }

        /*public string ComponentPipeDownLoadFile(string argumento)
        {
            string resp = string.Empty;

            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
            {
                byte[] arreglo = Encoding.ASCII.GetBytes("107~pdf~" + argumento);

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();
                pipeClient.Write(arreglo, 0, arreglo.Length);
                pipeClient.WaitForPipeDrain();
                var stream = new StreamReader(pipeClient);
                resp = stream.ReadToEnd();

                Console.Write(pipeClient);
                pipeClient.Close();
            }
            return resp;
        }*/


        public string ComponentPipe(string argumento, string accion)
        {
            string resp = string.Empty;
            string[] result;

            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
            {
                byte[] arreglo = Encoding.ASCII.GetBytes("601~" + accion + "~" + argumento);
                byte[] respuesta = new byte[512];

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();
                pipeClient.Write(arreglo, 0, arreglo.Length);
                pipeClient.WaitForPipeDrain();
                var stream = new StreamReader(pipeClient);
                resp = stream.ReadToEnd();

                result = resp.Split('~');
                Console.Write(pipeClient);
                pipeClient.Close();
            }
            return resp;
        }

        public string ComponentPipe(DetallePagoClienteDao cuponPago)
        {
            string resp = string.Empty;
            string[] result;

            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation))
            {
                string arg = string.Empty;

                var argumento = cuponPago.NroCliente + ";" +
                    cuponPago.Empresa + ";" +
                    cuponPago.NombreCliente + ";" +
                    cuponPago.RutCliente + ";" +
                    cuponPago.DvCliente + ";" +
                    cuponPago.Direccion + ";" +
                    cuponPago.Comuna + ";" +
                    cuponPago.Tarifa + ";" +
                    cuponPago.TotalDoc;

                if (cuponPago.DetallePagos.Count > 0)
                {
                    for (var i = 0; i < cuponPago.DetallePagos.Count; i++)
                    {
                        arg = ";" + cuponPago.DetallePagos[i].Monto.Replace(".", "") + ";" + cuponPago.DetallePagos[i].TipoDoc + ";" + cuponPago.DetallePagos[i].NumDoc;
                    }
                }
                var detalle = argumento + "@" + arg;


                byte[] arreglo = Encoding.ASCII.GetBytes("601~cupon~" + detalle);
                byte[] respuesta = new byte[512];

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();
                pipeClient.Write(arreglo, 0, arreglo.Length);
                pipeClient.WaitForPipeDrain();
                var stream = new StreamReader(pipeClient);
                resp = stream.ReadToEnd();

                result = resp.Split('~');
                Console.Write(pipeClient);
                pipeClient.Close();
            }
            return resp;
        }


    }
}
