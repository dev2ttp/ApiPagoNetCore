using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class RespuestasTBK
    {
    }

    public class ResponsePagoTbk
    {
        public PagoTbkResp data { get; set; }
        public bool status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class PagoTbkResp
    {
        public int codErr { get; set; }
        public string voucher { get; set; }
        public string tipoTarjeta { get; set; }
        public int codAuth { get; set; }
        public int? numOperacion { get; set; }
        public int? montoTotal { get; set; }
        public string digitosTarjeta { get; set; }
        public string montoCuotas { get; set; }
        public string numCuotas { get; set; }
        public string tipoCuotas { get; set; }
        public string fechaTransaccion { get; set; }
        public string horaTransaccion { get; set; }
    }

    public class DetallePagoCliReq
    {
        public string RutCliente { get; set; }
        public string DvCliente { get; set; }
        public string TotalCancelado { get; set; }
        public string NroCliente { get; set; }
    }

    public class TbkModel
    {
        public int codErr { get; set; }
        public string glosa { get; set; }
    }
    public class ResponseTbk
    {
        public TbkModel data { get; set; }
        public bool status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class DetallePagoClienteDao
    {
        public DetallePagoClienteDao() { }
        public long NroCliente { get; set; }
        public string Empresa { get; set; }
        public string RutCliente { get; set; }
        public string NombreCliente { get; set; }
        public string DvCliente { get; set; }
        public string Direccion { get; set; }
        public string Comuna { get; set; }
        public string Tarifa { get; set; }
        public int TotalDoc { get; set; }
        public string TotalCancelado { get; set; }
        public int TipoDocumento { get; set; }
        public string ResumenPago { get; set; }
        public List<DetallePagoDao> DetallePagos { get; set; }
    }

    public class PrintVoucherTbkReq
    {
        public string Voucher { get; set; }
    }
    public class DetallePagoDao
    {
        public DetallePagoDao() { }
        public string TipoDoc { get; set; }
        public int NumDoc { get; set; }
        public string FechaEmision { get; set; }
        public string Monto { get; set; }
    }
}
