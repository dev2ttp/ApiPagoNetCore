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

            BA.B1 = "0";
            BA.B2 = "0";
            BA.B3 = "0";
            BA.B4 = "0";
            BA.B5 = "0";
            BA.B6 = "0";
            BA.B7 = "0";
            BA.B8 = "0";
            BA.B9 = "0";
            BA.B10 = "0";


            BR.B1 = "0";
            BR.B2 = "0";
            BR.B3 = "0";
            BR.B4 = "0";
            BR.B5 = "0";
            BR.B6 = "0";
            BR.B7 = "0";
            BR.B8 = "0";
            BR.B9 = "0";
            BR.B10 = "0";


            MB.M1 = "0";
            MB.M2 = "0";
            MB.M3 = "0";
            MB.M4 = "0";
            MB.M5 = "0";
            MB.M6 = "0";
            MB.M7 = "0";
            MB.M8 = "0";
            MB.M9 = "0";
            MB.M10 = "0";

            MR.M1 = "0";
            MR.M2 = "0";
            MR.M3 = "0";
            MR.M4 = "0";
            MR.M5 = "0";
            MR.M6 = "0";
            MR.M7 = "0";
            MR.M8 = "0";
            MR.M9 = "0";
            MR.M10 = "0";

        }
        
    }

    public class GavBA
    {
        public string Idgav { get; set; }
        public string B1 { get; set; }
        public string B2 { get; set; }
        public string B3 { get; set; }
        public string B4 { get; set; }
        public string B5 { get; set; }
        public string B6 { get; set; }
        public string B7 { get; set; }
        public string B8 { get; set; }
        public string B9 { get; set; }
        public string B10 { get; set; }

    }

    public class GavBR
    {
        public string Idgav { get; set; }
        public string B1 { get; set; }
        public string B2 { get; set; }
        public string B3 { get; set; }
        public string B4 { get; set; }
        public string B5 { get; set; }
        public string B6 { get; set; }
        public string B7 { get; set; }
        public string B8 { get; set; }
        public string B9 { get; set; }
        public string B10 { get; set; }

    }

    public class GavMB
    {
        public string Idgav { get; set; }
        public string M1 { get; set; }
        public string M2 { get; set; }
        public string M3 { get; set; }
        public string M4 { get; set; }
        public string M5 { get; set; }
        public string M6 { get; set; }
        public string M7 { get; set; }
        public string M8 { get; set; }
        public string M9 { get; set; }
        public string M10 { get; set; }

    }

    public class GavMR
    {
        public string Idgav { get; set; }
        public string M1 { get; set; }
        public string M2 { get; set; }
        public string M3 { get; set; }
        public string M4 { get; set; }
        public string M5 { get; set; }
        public string M6 { get; set; }
        public string M7 { get; set; }
        public string M8 { get; set; }
        public string M9 { get; set; }
        public string M10 { get; set; }

    }
}
