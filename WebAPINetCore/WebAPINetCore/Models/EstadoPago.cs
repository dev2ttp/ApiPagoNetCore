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
        public float MontoAPagar { get; set; }
        public float DineroIngresado { get; set; }
        public float DineroFaltante { get; set; }
       // public bool EntregandoVuelto { get; set; }
    }

    public class EstadoPagoResp
    {
        public EstadoPagoResp() { 
        }
        public EstadoPago data { get; set; }
        public bool Status { get; set; }
        public bool PagoStatus { get; set; }
        public EstadosDeSalud StatusMaquina { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoEfectivo { get; set; }
    }
}
