using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class CargaDinero
    {
        public string M10 { get; set; }
        public string M50 { get; set; }
        public string M100 { get; set; }
        public string M500 { get; set; }
        public string B1000 { get; set; }
        public string B2000 { get; set; }
        public string B5000 { get; set; }
        public string B10000 { get; set; }
        public string B20000 { get; set; }
        public string IdTrans { get; set; }
        public string Rut { get; set; }

    }
}
