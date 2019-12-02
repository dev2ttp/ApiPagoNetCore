using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class EstadoPago
    {
        public int MontoAPagar { get; set; }
        public int DineroIngresado { get; set; }
        public int DineroFaltante { get; set; }
    }
}
