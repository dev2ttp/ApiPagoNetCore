﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class DatosCierreZ
    {
        public string MensajeaMostrar { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string IDZCierre { get; set; }
        public string Cantidad { get; set; }
        public string MontoTotal { get; set; }

        public DatosCierreZ()
        {
            MensajeaMostrar = "";
            Fecha = "";
            Hora = "";
            IDZCierre = "";
            Cantidad = "";
            MontoTotal = "";
        }
    }

    public class IDReportesCierre
    {
        public List<string> Fechas { get; set; }
        public List<string> ID { get; set; }

        public IDReportesCierre()
        {
            Fechas = new List<string>();
            ID = new List<string>();
        }
    }

}
