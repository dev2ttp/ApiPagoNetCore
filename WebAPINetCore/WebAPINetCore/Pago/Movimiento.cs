using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebAPINetCore.Services;
using WebAPINetCore.Models;

namespace WebAPINetCore.Pago
{
    public class Movimiento
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public string idKiosko = Globals.idmaquina;

        public string printerName = string.Format(Globals._config["Impresora:Nombre"]);

        public string IdTrx { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RutCliente { get; set; }
        public string Patente { get; set; }
        public int Monto { get; set; }
        public int Folio { get; set; }
        public string Detalle { get; set; }
        public bool OperacionExitosa { get; set; }
        public string TipoMovimientoM { get; set; }
        public string SubTipoM { get; set; }
        public string TipoAccion { get; set; }

        public Movimiento()
        { }


        public Movimiento(string idTrx, DateTime fechaFin, DateTime fechaTermino, string tipoMovimiento, string subTipoMovimiento, string detalle, string rutCliente, string patente, bool operacionExitosa)
        {
            IdTrx = idTrx;
            FechaInicio = fechaFin;
            FechaFin = fechaTermino;
            TipoMovimientoM = tipoMovimiento;
            SubTipoM = subTipoMovimiento;
            Detalle = detalle;
            RutCliente = rutCliente;
            Patente = patente;
            OperacionExitosa = operacionExitosa;
        }

        public Movimiento(int valor, string idTrx, DateTime fechaFin, DateTime fechaTermino, string tipoMovimiento, string subTipoMovimiento, string detalle, string rutCliente, string patente,
            bool operacionExitosa, string tipo)
        {
            IdTrx = idTrx;
            FechaInicio = fechaFin;
            FechaFin = fechaTermino;
            TipoMovimientoM = tipoMovimiento;
            SubTipoM = subTipoMovimiento;
            Detalle = detalle;
            RutCliente = rutCliente;
            Patente = patente;
            OperacionExitosa = operacionExitosa;
            TipoAccion = tipo;
        }

        public ResponsePagoTbk RealizarPagoTbk(DetallePagoCliReq req)
        {
            string msg = string.Empty;
            ResponsePagoTbk response = new ResponsePagoTbk();
            PagoTbkResp respPagoTbk = new PagoTbkResp();
            try
            {
                var idKiosko = Globals.idmaquina;
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>
                {
                    idKiosko,
                    "REG", //SRVTIpo
                    req.NroCliente,
                    req.TotalCancelado,
                    "",
                    req.RutCliente,
                    req.DvCliente
                };

                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.PagoTbk, data);
                log.Info("Q: Transbank: " + pipeClient.Message + " -> " + ServicioPago.Comandos.PagoTbk);
                pipeClient.SendMessage(ServicioPago.Comandos.PagoTbk);
                if (pipeClient.Resultado.CodigoError == 0)
                {
                    log.Info("R: Transbank: " + pipeClient.Resultado.Data[0] + " | Status Code: " + pipeClient.Resultado.CodigoError + " -> " + ServicioPago.Comandos.PagoTbk);
                    msg = string.Join("|", pipeClient.Resultado.Data[0]);
                    var resultData = pipeClient.Resultado.Data[0].Split('~');
                    respPagoTbk.codErr = pipeClient.Resultado.CodigoError;
                    respPagoTbk.voucher = resultData[0];
                    respPagoTbk.tipoTarjeta = resultData[2].Substring(0,1);
                    respPagoTbk.codAuth = Int32.Parse(resultData[4]);
                    respPagoTbk.numOperacion = resultData[5] != "" ? int.Parse(resultData[5]) : 0;
                    respPagoTbk.montoTotal = int.Parse(resultData[7]);
                    respPagoTbk.digitosTarjeta = resultData[12].PadLeft(6, '*');
                    respPagoTbk.tipoCuotas = resultData[15] ?? "";
                    respPagoTbk.numCuotas = resultData[16] ?? "";
                    respPagoTbk.montoCuotas = resultData[17] ?? "";                   
                    respPagoTbk.fechaTransaccion = DateTime.Now.Date.ToShortDateString();
                    respPagoTbk.horaTransaccion = DateTime.Now.ToShortTimeString();             
                    response.code = 200;
                    response.status = true;
                    response.data = respPagoTbk;
                    response.message = null;
                    return response;
                }
                else
                {
                    log.Info("E: Transbank: " + pipeClient.Resultado.Data[0] + " | Status Code: " + pipeClient.Resultado.CodigoError + " -> " + ServicioPago.Comandos.PagoTbk);
                    respPagoTbk.codErr = pipeClient.Resultado.CodigoError;
                    respPagoTbk.voucher = null;
                    response.code = 424;
                    response.status = false;
                    response.data = respPagoTbk;
                    response.message = pipeClient.Resultado.Data[0];
                    return response;
                }

            }
            catch (Exception ex)
            {
                log.Error("E: Transbank: " + ex.ToString() + " -> " + ServicioPago.Comandos.PagoTbk);
                response.code = 804;
                response.status = false;
                response.data = null;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }
        public void InsertarRegistro()
        {
            try
            {
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>();

                // 0,IdUsr 1,"REG" 2,IdUpd 3,FHini 4,FHfin 5,Tipo 6,Detalle 7,Monto 8,fCierre 9,SubTipo 10,RutCli 11,Folio 12,CodLugar 13,RutConvenio 14,RutMedico 15,CodEspecialidad
                // 16,FHagenda 17,RutCajero 18,TipoPago 19,IdCita 20,CodPrestacion 21,NomAsegurador 22,Turno
                data.Add("1");
                data.Add("REG");
                data.Add("0");
                data.Add(FechaInicio.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                data.Add(FechaFin.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                //data.Add(TiposToString[Tipo]);
                data.Add(TipoMovimientoM);
                data.Add(SubTipoM);
                data.Add(Monto.ToString());
                data.Add(OperacionExitosa ? "V" : "E");
                //data.Add(SubTiposToString[SubTipo]);
                data.Add(SubTipoM);
                data.Add(RutCliente.Replace(".", ""));
                data.Add(Folio.ToString()); //codigo Asegurador
                data.Add("0");
                data.Add("");
                data.Add(Patente.ToString());
                data.Add(TipoAccion.ToString());
                data.Add("");
                data.Add("");
                data.Add("0");
                data.Add("0");
                data.Add("");
                data.Add("");
                data.Add("0");
                data.Add(IdTrx.ToString());
                data.Add("");

                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.LogOpe, data);
                log.Info("Q: Transbank: "+ pipeClient.Message +" -> "+ ServicioPago.Comandos.LogOpe);
                pipeClient.SendMessage(ServicioPago.Comandos.LogOpe);

                if (pipeClient.Resultado.CodigoError == 0)
                {
                    string msg = string.Join("|", pipeClient.Resultado.Data[0]);
                    log.Info("R: Transbank: "+msg+" -> " + ServicioPago.Comandos.LogOpe);
                }
                else
                {
                    log.Info("E: Transbank: "+ pipeClient.Resultado.Data[0] +" -> " + ServicioPago.Comandos.LogOpe);
                }
            }
            catch (Exception ex)
            {
                log.Error("E: Transbank: "+ ex.ToString() + " -> " + ServicioPago.Comandos.LogOpe);
            }
        }

        
        /*public string InsertarPago(DetallePagoCliReq req)
        {
            string msg = string.Empty;
            try
            {
                var idKiosko = System.Configuration.ConfigurationManager.AppSettings["idKiosko"];
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>();

                data.Add(idKiosko);
                data.Add("REG"); //SRVTIpo
                data.Add(req.NroCliente);
                data.Add(req.TotalCancelado);
                data.Add("");
                data.Add(req.RutCliente);
                data.Add(req.DvCliente);

                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.PagoTbk, data);
                log.Info("Q: Transbank: " + pipeClient.Message + " -> " + ServicioPago.Comandos.PagoTbk);
                pipeClient.SendMessage(ServicioPago.Comandos.PagoTbk);

                if (pipeClient.Resultado.CodigoError == 0)
                {
                    msg = string.Join("|", pipeClient.Resultado.Data[0]);
                    var ticket = pipeClient.Resultado.Data[0].Split('~')[0];
                    msg = ticket;
                    log.Info("R: Transbank: " + pipeClient.Resultado.Data[0] + " Cod: " + pipeClient.Resultado.CodigoError + " -> " + ServicioPago.Comandos.PagoTbk);
                }
                else
                {
                    msg = "NOK";
                    log.Info("E: Transbank: " + pipeClient.Resultado.Data[0] + " Cod: "+ pipeClient.Resultado.CodigoError + " -> " + ServicioPago.Comandos.PagoTbk);
                }
            }
            catch (Exception ex)
            {
                msg = "NOK";
                log.Error("E: Transbank: " + ex.ToString() + " -> " + ServicioPago.Comandos.PagoTbk);
            }
            return msg;
        }*/


        public string ValidarTbk(ServicioPago.Comandos comandos, string idKiosko)
        {
            string msg = string.Empty;
            try
            {
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>
                {
                    idKiosko
                };

                pipeClient.Message = servicio.BuildMessage(comandos, data);
                pipeClient.SendMessage(comandos);
                log.Info("Q: Validar Transbank: "+ pipeClient.Message +" | Codigo Error: "+ pipeClient.Resultado.CodigoError + " -> " + comandos);
                if (pipeClient.Resultado.CodigoError == -1)
                {
                    log.Info("E: AdmTbk: "+ pipeClient.Resultado.Data[0] + " -> " + comandos);
                    return msg;
                }
                else if (pipeClient.Resultado.CodigoError == 0)
                {
                    msg = string.Join("|", pipeClient.Resultado.Data);
                    log.Info("R: AdmTbk: "+ msg + " -> " + comandos);
                    if (comandos == ServicioPago.Comandos.CierreTbk || comandos == ServicioPago.Comandos.UltimaVenta)
                    {
                        string[] var = pipeClient.Resultado.Data[0].Split('~');
                        //string fullTicket = string.Empty;
                        //NIIClassLib nii = new NIIClassLib();

                        //fullTicket = "\x1B\x32\n";      // Set pitch default   
                        //fullTicket += "\x1B\x74\x00";   // No japan code

                        log.Info("Q: EmiPrinter: "+ string.Format("valor var 2: {1}", printerName, var[2]));
                        msg = var[2];
                        log.Info("R: EmiPrinter Ticket: " + msg + " -> " + comandos);
                        log.Info("R: Validar OK");
                        return msg;
                    }
                    else
                    {
                        log.Info("E: Validar Transbank: " + pipeClient.Resultado.Data[0] + " -> " + ServicioPago.Comandos.PagoTbk);
                        return msg;
                    }
                }
                else
                {
                    log.Info("E: Validar Transbank: " + pipeClient.Resultado.Data[0] + " -> " + ServicioPago.Comandos.PagoTbk);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                log.Error("E: Validar Transbank: " + ex.ToString() + " -> " + comandos);
                return msg;
            }           
        }

        /// <summary>
        /// método para consultar el estado de tbk, sin booleano alex
        /// </summary>
        private static void EstadoTransbank()
        {
            try
            {
                var idKiosko = Globals.idmaquina;
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>
                {
                    idKiosko
                };

                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.Pooling, data);
                log.Info("Q: Transbank: "+ pipeClient.Message + " -> " + ServicioPago.Comandos.Pooling);
                pipeClient.SendMessage(ServicioPago.Comandos.Pooling);

                if (pipeClient.Resultado.CodigoError == 0)
                {
                    string msg = string.Join("|", pipeClient.Resultado.Data[0]);
                    log.Info("R: Transbank: "+ msg + " -> " + ServicioPago.Comandos.Pooling);
                }
                else
                {
                    log.Info("E: Transbank: "+ pipeClient.Resultado.Data[0] + " -> " + ServicioPago.Comandos.Pooling);
                }
            }
            catch (Exception ex)
            {
                log.Error("E: Transbank: "+ ex.ToString() + " -> " + ServicioPago.Comandos.Pooling);
            }
        }

        /// <summary>
        /// método para consultar el estado de tbk
        /// </summary>
        public bool EstadoTransbank(ServicioPago.Comandos comandos)
        {
            try
            {
                var idKiosko = Globals.idmaquina;
                PipeClient pipeClient = new PipeClient();
                ServicioPago servicio = new ServicioPago();
                List<string> data = new List<string>
                {
                    idKiosko
                };

                pipeClient.Message = servicio.BuildMessage(ServicioPago.Comandos.Pooling, data);
                log.Info("Q: Transbank: "+ pipeClient.Message + " -> " + ServicioPago.Comandos.Pooling);
                pipeClient.SendMessage(ServicioPago.Comandos.Pooling);

                if (pipeClient.Resultado.CodigoError == 0)
                {
                    string msg = string.Join("|", pipeClient.Resultado.Data[0]);
                    log.Info("R: Transbank: "+ msg + " -> " + ServicioPago.Comandos.Pooling);
                    return true;
                }
                else
                {
                    log.Info("E: Transbank: "+ pipeClient.Resultado.Data[0] + " -> " + ServicioPago.Comandos.Pooling);
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("E: Transbank: "+ ex.ToString() + " -> " + ServicioPago.Comandos.Pooling);
                return false;
            }
        }

        public ResponseTbk cierreTbk()
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                string msg = ValidarTbk(ServicioPago.Comandos.CierreTbk, idKiosko);
                log.Info("Q: Transbank: -> " + ServicioPago.Comandos.CierreTbk);
                if (msg != "")
                {
                    PipeServiceTBK pipe = new PipeServiceTBK();
                    var resp = pipe.ComponentPipePrintVoucher(msg);

                    respTbk.codErr = 0;
                    respTbk.glosa = resp;            
                    response.data = respTbk;
                    response.code = 200;
                    response.status = true;
                    response.message = null;
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.CierreTbk + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                else
                {
                    respTbk.codErr = -1;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 424;
                    response.status = false;
                    response.message = "Error al Validar Tbk";
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.CierreTbk + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
            }
            catch (Exception e)
            {
                log.Error("E: Transbank: " + e.ToString() + " -> " + ServicioPago.Comandos.CierreTbk);
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            } 
        }

        public ResponseTbk inicializacionTbk()
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                string msg = ValidarTbk(ServicioPago.Comandos.IniTbk, idKiosko);
                log.Info("Q: Transbank: -> " + ServicioPago.Comandos.IniTbk);
                if (msg != "")
                {
                    respTbk.codErr = 0;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 200;
                    response.status = true;
                    response.message = null;
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.IniTbk + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                else
                {
                    respTbk.codErr = -1;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 424;
                    response.status = false;
                    response.message = "Error al Validar Tbk";
                    log.Info("E: Transbank: -> " + ServicioPago.Comandos.IniTbk + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }   
            }
            catch (Exception e)
            {
                log.Error("E: Transbank: " + e.ToString() + " -> " + ServicioPago.Comandos.IniTbk);
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }

        public ResponseTbk cargaLlavesTbk()
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                string msg = ValidarTbk(ServicioPago.Comandos.CargaLlavesTbk, idKiosko);
                log.Info("Q: Transbank: -> " + ServicioPago.Comandos.CargaLlavesTbk);
                if (msg != "")
                {
                    respTbk.codErr = 0;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 200;
                    response.status = true;
                    response.message = null;
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.CargaLlavesTbk+ " || Data: "+ JsonConvert.SerializeObject(response));
                    return response;
                }
                else
                {
                    respTbk.codErr = -1;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 424;
                    response.status = false;
                    response.message = "Error al Validar Tbk";
                    log.Info("E: Transbank: -> " + ServicioPago.Comandos.CargaLlavesTbk + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
            }
            catch (Exception e)
            {
                log.Error("E: Transbank: " + e.ToString() + " -> " + ServicioPago.Comandos.CargaLlavesTbk);
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }
        public ResponseTbk anularPagoTbk()
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                string msg = ValidarTbk(ServicioPago.Comandos.AnulaPago, idKiosko);
                log.Info("Q: Transbank: -> " + ServicioPago.Comandos.AnulaPago);
                if (msg != "")
                {
                    respTbk.codErr = 0;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 200;
                    response.status = true;
                    response.message = null;
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.AnulaPago + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                else
                {
                    respTbk.codErr = -1;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 424;
                    response.status = false;
                    response.message = "Error al Validar Tbk";
                    log.Info("E: Transbank: -> " + ServicioPago.Comandos.AnulaPago + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
            }
            catch (Exception e)
            {
                log.Error("E: Transbank: " + e.ToString() + " -> " + ServicioPago.Comandos.AnulaPago);
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }

        public ResponseTbk ultimaVentaTbk()
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                log.Info("Q: Transbank: -> " + ServicioPago.Comandos.UltimaVenta);
                string msg = ValidarTbk(ServicioPago.Comandos.UltimaVenta, idKiosko);
                
                if (msg != "")
                {
                    PipeServiceTBK pipe = new PipeServiceTBK();
                    var resp = pipe.ComponentPipePrintVoucher(msg);

                    respTbk.codErr = 0;
                    respTbk.glosa = resp;
                    response.data = respTbk;
                    response.code = 200;
                    response.status = true;
                    response.message = null;
                    log.Info("R: Transbank: -> " + ServicioPago.Comandos.UltimaVenta + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                else
                {
                    respTbk.codErr = -1;
                    respTbk.glosa = msg;
                    response.data = respTbk;
                    response.code = 424;
                    response.status = false;
                    response.message = "Error al Validar Tbk";
                    log.Info("E: Transbank: -> " + ServicioPago.Comandos.UltimaVenta + " || Data: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                
            }
            catch (Exception e)
            {
                log.Error("E: Transbank: " + e.ToString() + " -> " + ServicioPago.Comandos.UltimaVenta);
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }

        public ResponseTbk printVoucherTbk(PrintVoucherTbkReq req)
        {
            ResponseTbk response = new ResponseTbk();
            TbkModel respTbk = new TbkModel();
            try
            {
                log.Info("Q: Transbank Print Voucher");
                PipeServiceTBK pipe = new PipeServiceTBK();
                var resp = pipe.ComponentPipePrintVoucher(req.Voucher);
                respTbk.codErr = 0;
                respTbk.glosa = resp;
                response.data = respTbk;
                response.code = 200;
                response.status = true;
                response.message = null;
                log.Info("R: Transbank Print Voucher: -> Data: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (Exception e)
            {
                log.Error("E: Transbank Print Voucher: " + e.ToString());
                response.data = null;
                response.code = 804;
                response.status = false;
                response.message = "Error al consultar ApiRestful MasVida";
                return response;
            }
        }

        /*
         * Método para consultar el estado de impresora NII
         */
        public bool EstadoPrinter()
        {
            log.Info("Q: Printer Status: "+ "Revisión impresora: " + Globals._config["Impresora:Nombre"]);

            //ImpresoraTermica.GetEstado();
            if (false) {
                log.Info("E: Printer Status: FALSE");
                return false;
            }
            else
            {
                log.Info("R: Printer Status: TRUE");
                return true;
            } 
        }
        /*
         * Método para consultar el estado de impresora TUP
         */
        public bool EstadoPrinterTup()
        {
            log.Info("Q: Printer Status: " + "Revisión impresora: " + Globals._config["Impresora:Nombre"]);

            if (false)
            {
                log.Info("E: Printer Status: FALSE");
                return false;
            }
            else
            {
                log.Info("R: Printer Status: TRUE");
                return true;
            }
        }
        public void InsertarRegistroAsync()
        {
            Task.Factory.StartNew(InsertarRegistro);
        }


        public void ConsultarEstadoTbkAsync()
        {
            Task.Factory.StartNew(EstadoTransbank);
        }


    }
}