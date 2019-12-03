using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace WebAPINetCore.PipeServer
{
    public class PipeClient2
    {
        public struct Results
        {
            public int CodigoError;
            public List<string> Data;
            public string EstadoSalud;
        }

        public int Timeout { get; set; }
        public string Message { get; set; } //aca se guarda el mensaje ya listo para enviar al componente pipe
        public Results Resultado;

        public string _Resp;

        public PipeClient2()
        {
            this.Timeout = 30 * 1000;
        }

        public bool SendMessage(ServicioPago.Comandos command)
        {
            if (MessageSentSuccessfully())
            {
                ServicioPago trx = new ServicioPago();

                if (_Resp.Length < (int)ServicioPago.Posicion.MIN_MSG_LEN)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGLEN;
                else if (_Resp.Length > (int)ServicioPago.Posicion.MAX_MSG_LEN)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGLEN;
                else if (_Resp.Substring(0, (int)ServicioPago.Posicion.HEAD_LEN) != ServicioPago.Header || _Resp.Last() != '@')
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGFMT;

                if (Resultado.CodigoError != 0)
                {
                    // Resultado.Data = new List<string> { Estado.Servicio.GetError(Resultado.CodigoError) };
                    return false; 
                }

                int respCmd = Convert.ToInt32(_Resp.Substring((int)ServicioPago.Posicion.CMD_POS, (int)ServicioPago.Posicion.CMD_LEN));
                int len = Convert.ToInt32(_Resp.Substring((int)ServicioPago.Posicion.LEN_POS, (int)ServicioPago.Posicion.LEN_LEN));

                if (_Resp.Length != len)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGLEN;
                else if (respCmd != 999 && respCmd != (int)command)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGCMD;

                if (Resultado.CodigoError != 0)
                {
                    // Resultado.Data = new List<string> { Estado.Servicio.GetError(Resultado.CodigoError) };
                    return false; 
                }
                if (!ServicioPago.IsChecksumValid(_Resp))
                {
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGCKS;
                    // Resultado.Data = new List<string> { Estado.Servicio.GetError(Resultado.CodigoError) };
                    return false;
                }

                string msg = _Resp.Substring((int)ServicioPago.Posicion.DAT_POS, len - (int)ServicioPago.Posicion.FIX_LEN);
                if (respCmd == 999)
                {
                    Resultado.CodigoError = Convert.ToInt32(msg.Substring(0, 5));
                    if (Resultado.CodigoError == (int)ServicioPago.ValidaMsgServer.ERRTBK)
                        Resultado.Data = new List<string> { msg.Substring(5, msg.Length - 5) };
                    //else
                        //   Resultado.Data = new List<string> { Estado.Servicio.GetError(Resultado.CodigoError) };
                        return false;
                }

                Resultado.CodigoError = 0;
                Resultado.Data = msg.Split('|').ToList();
                if (Resultado.Data[0].Contains("&&EST"))
                {
                    Resultado.EstadoSalud = Resultado.Data[0].Substring(0,7);
                    Resultado.EstadoSalud = Resultado.EstadoSalud.Substring(2,5);
                    Resultado.Data[0]= Resultado.Data[0].Substring(7,Resultado.Data[0].Length-7);
                    //Global.EST = Resultado.EstadoSalud;
                }
                return true;

            }
            else
            {
                Resultado.CodigoError = -1;
                Resultado.Data = new List<string> { _Resp };
                return false;
            }
        }

        private bool MessageSentSuccessfully()
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "totalpago2", PipeDirection.InOut, PipeOptions.None))
                {
                    pipeClient.Connect(this.Timeout);
                    byte[] buffer = Encoding.ASCII.GetBytes(this.Message);
                    pipeClient.Write(buffer, 0, buffer.Length);
                    pipeClient.WaitForPipeDrain();

                    StreamReader stream = new StreamReader(pipeClient);
                    _Resp = stream.ReadToEnd();
                    //string aux = _Resp.Substring(1, _Resp.LastIndexOf("@") + 1);
                    //_Resp = aux;
                }

                return true;
            }
            catch (Exception ex)
            {
                _Resp = ex.ToString();
                return false;
            }
        }
    }
}

