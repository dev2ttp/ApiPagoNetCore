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
        public EstadosDeSalud StatusMaquina { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoEfectivo { get; set; }
    }
}
