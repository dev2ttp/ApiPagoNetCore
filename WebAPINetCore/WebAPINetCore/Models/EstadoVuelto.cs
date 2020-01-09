using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class EstadoVuelto
    {
        public EstadoVuelto()
        {
        }
        public int VueltoTotal { get; set; }
        public int DineroRegresado { get; set; }
        public int DineroFaltante { get; set; }
        public bool VueltoFinalizado { get; set; }
    }

    public class EstadoVueltoResp
    {
        public EstadoVueltoResp()
        {
        }
        public EstadoVuelto data { get; set; }
        public bool Status { get; set; }
        public bool PagoStatus { get; set; }
        public EstadosDeSalud StatusMaquina { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoEfectivo { get; set; }
    }
}
