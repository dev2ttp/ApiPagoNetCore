using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class EstadoPago
    {
        public EstadoPago() { 
        }
        public int MontoAPagar { get; set; }
        public int DineroIngresado { get; set; }
        public int DineroFaltante { get; set; }
    }

    public class EstadoPagoResp
    {
        public EstadoPagoResp() { 
        }
        public EstadoPago data { get; set; }
        public bool Status { get; set; }
    }
}
