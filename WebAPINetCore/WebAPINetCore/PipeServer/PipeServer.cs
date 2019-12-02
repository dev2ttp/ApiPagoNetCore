
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

namespace WebAPINetCore.PipeServer
{
    public class PipeServer
    {
        public struct Results
        {
            public readonly int ErrorCode;
            public readonly List<string> Result;

            public Results(int errorCode, List<string> result)
            {
                this.ErrorCode = errorCode;
                this.Result = result;
            }
        }

        private NamedPipeServerStream pipeServer;

        private Results _Resultado;
        public Results Resultado
        {
            get { return _Resultado; }
            set { _Resultado = value; }
        }

        public PipeServer()
        { }

        public void StartListening()
        {
            this.Resultado = new Results(0, null);
            pipeServer = new NamedPipeServerStream("totalpago", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            pipeServer.BeginWaitForConnection(WaitForConnectionCallback, pipeServer);
        }

        public void StopListening()
        {
            pipeServer.Close();
            pipeServer.Dispose();
            pipeServer = null;
        }

        public void Clear()
        {
            this.Resultado = new Results(0, null);
        }

        private void WaitForConnectionCallback(IAsyncResult iar)
        {
            try
            {
                pipeServer = (NamedPipeServerStream)iar.AsyncState;
                pipeServer.EndWaitForConnection(iar);

                byte[] buffer = new byte[1024];
                pipeServer.Read(buffer, 0, buffer.Length);

                buffer = buffer.Where(x => x != 0x00).ToArray();
                string stringData = Encoding.UTF8.GetString(buffer);

                this.Resultado = new Results(0, new List<string> { stringData });

                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream("totalpago", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                pipeServer.BeginWaitForConnection(WaitForConnectionCallback, pipeServer);
                //Log.Write("TbkMessage", stringData, Evento.Message);
            }
            catch (ObjectDisposedException)
            {
                // Cuando se llama al método Close() para cerrar el server, se ejecuta WaitForConnectionCallback() aunque el pipe ya está cerrado.
                // Esto produce una excepción, que se controla aquí.
                return;
            }
            catch (Exception ex)
            {
                this.Resultado = new Results(-1, new List<string> { ex.Message });
                //Log.Write("TbkMessage", ex.ToString(), Evento.Error);
            }
        }
    }
}
