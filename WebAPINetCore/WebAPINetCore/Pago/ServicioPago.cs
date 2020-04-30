using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace WebAPINetCore.Pago
{
    public class ServicioPago
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public const string Header = "TP10PP";

        public enum Comandos
        {
            EstadoImpresora = 104,  // 0,IdUsr 1,PrtEdo


            IniTbk = 190,           // 0,IdUsr
            CargaLlavesTbk = 191,   // 0,IdUsr


            PagoTbk = 192,          // 0,IdUsr 1,SrvTipo 2,BolNum 3,Monto 4,Financiador 5,RutM 6,DV
            AnulaPago = 193,        // 0,IdUsr 1,BolNum 2,Monto 3,Motivo
            UltimaVenta = 194,      // 0,IdUsr


            CierreTbk = 195,        // 0,IdUsr



            Pooling = 196,          // 0,IdUsr
            PagoOk = 200,           // 0,IdUsr 1,SrvTipo 2,BolNum 3,Monto 4,Financiador 5,RutM 6,DV 7,MedioPago 8,MedioTipo 9,MedioSubTipo 10,CodAut 11,FolBono 12,$Total 13,$Bonif            
            PagoFallido = 201,      // 0,IdUsr 1,SrvTipo 2,BolNum 3,Monto 4,Financiador 5,RutM 6,DV 7,MedioPago 8,MedioTipo 9,Error
            PagoUpdate = 202,       // 0,IdUsr 1,IdPago  2,NumDoc 3,$Total 4,$Bonif [5,$Medio 6,MedioPago 7,MedioTipo 8,Tarjeta 9,CodAut]
            LogOpe = 204,           // 0,IdUsr 1,acc 2,idToUpd 3,FHini 4,FHfin 5,tipo 6,detalle 7,monto 8,fCierre 9,subTipo 10,rutCli 11,codAseg 12,codLugar 13,rutConv 14,rutMedico 15,codEsp 16,FHagenda 17,rutCaj 18,tipoPago 19,idCita 20,codPres 21,nomAseg
            PagoOk2 = 205           // 0,IdUsr 1,SrvTipo 2,NumDoc 3,$Total 4,Financiador 5,RutM 6,DV 7,$Bonif 8,DatOpe(0,$Medio 1,MedioPago 2,MedioTipo 3,MedioSubTipo 4,CodAut)
        }

        public enum Posicion
        {
            HEAD_LEN = 6,
            LEN_LEN = 5,
            CMD_LEN = 3,
            CKS_LEN = 4,
            FOOT_LEN = 5,                                                                // 4(Cks) + 1(@)
            MAX_DAT_LEN = 8192,                                                          // Largo maximo Data
            FIX_LEN = (HEAD_LEN + LEN_LEN + CMD_LEN + FOOT_LEN),
            LEN_POS = (HEAD_LEN),                                                        // Posición Largo en Mensaje
            CMD_POS = (HEAD_LEN + LEN_LEN),                                              // Posición Comando en Mensaje
            DAT_POS = (HEAD_LEN + LEN_LEN + CMD_LEN),                                    // Posición Data en Mensaje
            MIN_MSG_LEN = (HEAD_LEN + LEN_LEN + CMD_LEN + FOOT_LEN),                     // Largo Min Mensaje
            MAX_MSG_LEN = (HEAD_LEN + LEN_LEN + CMD_LEN + MAX_DAT_LEN + FOOT_LEN)        // Largo Max Mensaje
        }

        public enum ValidaMsg
        {
            MSGMIN = 12001,   // MSG: largo < permitido
            MSGMAX = 12002,   // MSG: largo > permitido
            MSGFMT = 12003,   // MSG: formato
            MSGLEN = 12004,   // MSG: tamaño string recv
            MSGCMD = 12005,   // MSG: comando no válido
            MSGCKS = 12006,   // MSG: checksum no válido
            TIMOUT = 11190,   // Timeout
            GENSCS = 12099    // Error general
        }

        public enum ValidaMsgServer
        {
            MSGMIN = 11501,   // MSG: largo < permitido
            MSGMAX = 11502,   // MSG: largo > permitido
            MSGFMT = 11503,   // MSG: formato
            MSGLEN = 11504,   // MSG: tamaño string recv
            MSGCMD = 11505,   // MSG: comando no válido
            MSGCKS = 11506,   // MSG: checksum no válido
            MSGDAT = 11507,   // MSG: número de data no válida
            ERRTBK = 11541,   // MSG: error en transbank
            GENSCS = 11599    // Error general
        }

        public ServicioPago()
        { }

        public string GetMessageTransbank(string codigo)
        {
            switch (codigo)
            {
                case "00": return "Aprobado";
                case "01": return "Rechazado";
                case "02": return "Host no Responde";
                case "03": return "Conexión Fallo";
                case "04": return "Transacción ya Fue Anulada";
                case "05": return "No existe Transacción para Anular";
                case "06": return "Tarjeta no Soportada";
                case "07": return "Transacción Cancelada";
                case "08": return "Debe Anular pago por mesón";
                case "09": return "Error Lectura Tarjeta";
                case "10": return "Monto menor al mínimo permitido";
                case "11": return "No existe venta";
                case "12": return "Transacción No Soportada";
                case "13": return "Debe ejecutar cierre";
                case "79": return "Inserte y retire la tarjeta";
                case "80": return "Confirme Monto con botón verde";
                case "81": return "Ingrese clave y presione botón verde";
                case "82": return "Solicitando confirmación de pago";
                case "90": return "Inicialización Exitosa";
                case "91": return "Inicialización Fallida";
                case "92": return "Lector no Conectado";
                default: return "Mensaje Desconocido";
            }
        }

        private bool IsMessageValid(string message, ref int codError)
        {
            codError = 0;
            if (message.Length < (int)Posicion.MIN_MSG_LEN)
                codError = (int)ValidaMsg.MSGLEN;
            else if (message.Length > (int)Posicion.MAX_MSG_LEN)
                codError = (int)ValidaMsg.MSGLEN;
            else if (message.Substring(0, (int)Posicion.HEAD_LEN) != Header || message.Last() != '@')
                codError = (int)ValidaMsg.MSGFMT;

            return codError == 0;
        }

        public string BuildMessage(Comandos command, List<string> data)
        {
            string datas = string.Join("|", data);
            string cmd = ((int)command).ToString();
            datas = NormaliseString(datas);
            int length = Header.Length + 5 + cmd.Length + datas.Length + 4 + 1;

            return Header + length.ToString().PadLeft(5, '0') + cmd + datas + Checksum(Header + length.ToString().PadLeft(5, '0') + cmd + datas) + "@";
        }

        private string Checksum(string data)
        {
            long cks = 0;
            for (int i = 1; i <= data.Length; i++)
            {
                cks -= Strings.Asc(Strings.Mid(data, i, 1));
            }
            return Strings.Right(Conversion.Hex(cks), 4).PadLeft(4, '0');
        }

        public static bool IsChecksumValid(string receiveText)
        {
            return !(Strings.Mid(receiveText, Convert.ToInt32(Strings.Mid(receiveText, 7, 5)) - 5 + 1, 4) != new ServicioPago().Checksum(Strings.Mid(receiveText, 1, receiveText.Length - 5)));
        }

        private string NormaliseString(string text)
        {
            string resp;
            resp = text.Replace("á", "a");
            resp = resp.Replace("é", "e");
            resp = resp.Replace("í", "i");
            resp = resp.Replace("ó", "o");
            resp = resp.Replace("ú", "u");
            resp = resp.Replace("ñ", "n");
            resp = resp.Replace("Á", "A");
            resp = resp.Replace("É", "E");
            resp = resp.Replace("Í", "I");
            resp = resp.Replace("Ó", "O");
            resp = resp.Replace("Ú", "U");
            resp = resp.Replace("Ñ", "N");
            return resp;
        }

        public string GetError(int command)
        {
            switch (command)
            {
                case (int)ValidaMsg.MSGMIN:
                case (int)ValidaMsgServer.MSGMIN:
                    return "Largo de mensaje < permitido";
                case (int)ValidaMsg.MSGMAX:
                case (int)ValidaMsgServer.MSGMAX:
                    return "Largo de mensaje > permitido";
                case (int)ValidaMsg.MSGFMT:
                case (int)ValidaMsgServer.MSGFMT:
                    return "Error de formato en mensaje";
                case (int)ValidaMsg.MSGLEN:
                case (int)ValidaMsgServer.MSGLEN:
                    return "Error de tamaño de mensaje";
                case (int)ValidaMsg.MSGCMD:
                case (int)ValidaMsgServer.MSGCMD:
                    return "Comando no válido";
                case (int)ValidaMsg.MSGCKS:
                case (int)ValidaMsgServer.MSGCKS:
                    return "Checksum no válido";
                case (int)ValidaMsgServer.MSGDAT:
                    return "Parámetros no válidos";
                case (int)ValidaMsgServer.GENSCS:
                    return "Error general";
                case (int)ValidaMsg.TIMOUT:
                    return "Timeout Servidor";
                default:
                    return "Error General";
            }
        }

        private bool IsPipeServerReady()
        {
            PipeClient pipeClient = new PipeClient();

            try
            {
                pipeClient.Message = BuildMessage(Comandos.EstadoImpresora, new List<string>() { "1", "0" });
                log.Info("Q: IniSrvPago: "+ pipeClient.Message + " -> " + Comandos.EstadoImpresora);
                pipeClient.SendMessage(Comandos.EstadoImpresora);

                if (pipeClient.Resultado.CodigoError == 0)
                {
                    string msg = string.Join("|", pipeClient.Resultado.Data);
                    log.Info("R: IniSrvPago: "+ msg + " -> " + Comandos.EstadoImpresora);
                    return true;
                }
                else
                {
                    log.Info("E: IniSrvPago: "+ pipeClient.Resultado.CodigoError + ": " + pipeClient.Resultado.Data[0] + " -> " + Comandos.EstadoImpresora);
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("IniSrvPago: "+ ex.ToString());
                return false;
            }
        }
    }
}