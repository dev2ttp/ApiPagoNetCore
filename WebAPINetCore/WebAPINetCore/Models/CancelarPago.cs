using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class CancelarPago
    {
        public CancelarPago() { }

        public bool CancelacionCompleta { get; set; }
        public bool EntregandoVuelto { get; set; }
    }
}
