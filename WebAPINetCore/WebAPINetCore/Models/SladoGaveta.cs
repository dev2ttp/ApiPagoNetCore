using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class SaldoGaveta
    {
        public GavBA BA { get; set; }
        public GavBR BR { get; set; }
        public GavMB MB { get; set; }
        public GavMR MR { get; set; }

        public SaldoGaveta() {
            BA = new GavBA();
            BR = new GavBR();
            MB = new GavMB();
            MR = new GavMR();
        }
        
    }

    public class GavBA
    {

        public string B1000 { get; set; }
        public string B2000 { get; set; }
        public string B5000 { get; set; }
        public string B10000 { get; set; }
        public string B20000 { get; set; }

    }

    public class GavBR
    {

        public string B1000 { get; set; }
        public string B2000 { get; set; }
        public string B5000 { get; set; }
        public string B10000 { get; set; }
        public string B20000 { get; set; }

    }

    public class GavMB
    {

        public string M10 { get; set; }
        public string M50 { get; set; }
        public string M100 { get; set; }
        public string M500 { get; set; }
        
    }

    public class GavMR
    {

        public string M10 { get; set; }
        public string M50 { get; set; }
        public string M100 { get; set; }
        public string M500 { get; set; }

    }
}
