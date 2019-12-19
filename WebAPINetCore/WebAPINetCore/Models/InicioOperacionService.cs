using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class InicioOperacionService
    {
        public InicioOperacionService()
        {
        }
        public bool Status { get; set; }
        public bool StatusMaquina { get; set; }
        public String MensajeAmostrar { get; set; }
        // public bool EntregandoVuelto { get; set; }
    }
}
