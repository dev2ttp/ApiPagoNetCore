using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace WebAPINetCore.Pago
{
    public class PipeClient
    {
        public struct Results
        {
            public int CodigoError;
            public List<string> Data;
        }

        public int Timeout { get; set; }
        public string Message { get; set; }
        public Results Resultado;

        private string _Resp;
        private readonly ServicioPago _servicioPago;
        private const string PipeClientName = "totalpago";

        public PipeClient()
        {
            this.Timeout = 10 * 1000;
            _servicioPago = new ServicioPago();
        }

        public void SendMessage(ServicioPago.Comandos command)
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
                    Resultado.Data = new List<string> { _servicioPago.GetError(Resultado.CodigoError) };
                    return;
                }

                int respCmd = Convert.ToInt32(_Resp.Substring((int)ServicioPago.Posicion.CMD_POS, (int)ServicioPago.Posicion.CMD_LEN));
                int len = Convert.ToInt32(_Resp.Substring((int)ServicioPago.Posicion.LEN_POS, (int)ServicioPago.Posicion.LEN_LEN));

                if (_Resp.Length != len)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGLEN;
                else if (respCmd != 999 && respCmd != (int)command)
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGCMD;

                if (Resultado.CodigoError != 0)
                {
                    Resultado.Data = new List<string> { _servicioPago.GetError(Resultado.CodigoError) };
                    return;
                }
                if (!ServicioPago.IsChecksumValid(_Resp))
                {
                    Resultado.CodigoError = (int)ServicioPago.ValidaMsg.MSGCKS;
                    Resultado.Data = new List<string> { _servicioPago.GetError(Resultado.CodigoError) };
                    return;
                }

                string msg = _Resp.Substring((int)ServicioPago.Posicion.DAT_POS, len - (int)ServicioPago.Posicion.FIX_LEN);
                if (respCmd == 999)
                {
                    Resultado.CodigoError = Convert.ToInt32(msg.Substring(0, 5));
                    if (Resultado.CodigoError == (int)ServicioPago.ValidaMsgServer.ERRTBK)
                        Resultado.Data = new List<string> { msg.Substring(5, msg.Length - 5) };
                    else
                        Resultado.Data = new List<string> { _servicioPago.GetError(Resultado.CodigoError) };
                    return;
                }

                Resultado.CodigoError = 0;
                Resultado.Data = msg.Split('|').ToList();
            }
            else
            {
                Resultado.CodigoError = -1;
                Resultado.Data = new List<string> { _Resp };
            }
        }

        private bool MessageSentSuccessfully()
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PipeClientName, PipeDirection.InOut, PipeOptions.None))
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