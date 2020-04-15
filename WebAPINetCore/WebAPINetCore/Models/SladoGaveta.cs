﻿using System;
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
