using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class EstadoPuerta 
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo  { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoCorriente
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMinimoBillete
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMaxBillete
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoMinMonedas
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMaxMonedas
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispDiferente
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispAtascoSeguro
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispAtascoInSeguro
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispIntentoFraudeB
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispCajaFull
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispUnidadAtascada    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispMonedaAtascada
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispBusquedaFallida
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispIntentoFraudeMoneda
    {
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadosDeSalud
    {
        public EstadoPuerta StadoPuerta = new EstadoPuerta();
        public EstadoCorriente StadoCorreinte = new EstadoCorriente();
        public EstadoMinimoBillete StadoMinBillete = new EstadoMinimoBillete();
        public EstadoMaxBillete StadoMaxBillete = new EstadoMaxBillete();
        public EstadoMinMonedas StadoMinMoneda = new EstadoMinMonedas();
        public EstadoMaxMonedas StadoMaxMonedas = new EstadoMaxMonedas();
        public EstadoDispDiferente StadoDispDiferente = new EstadoDispDiferente();
        public EstadoDispAtascoSeguro StadoDispAtascadoS = new EstadoDispAtascoSeguro();
        public EstadoDispAtascoInSeguro StadoDispAtascadoInS = new EstadoDispAtascoInSeguro();
        public EstadoDispIntentoFraudeB StadoDispIntentoFraude = new EstadoDispIntentoFraudeB();
        public EstadoDispCajaFull StadoDispCajaFull = new EstadoDispCajaFull();
        public EstadoDispUnidadAtascada StadoDispUnidadAtascada = new EstadoDispUnidadAtascada();
        public EstadoDispMonedaAtascada StadoDispMonedaAtascada = new EstadoDispMonedaAtascada();
        public EstadoDispBusquedaFallida StadoDispBusquedaFallida = new EstadoDispBusquedaFallida();
        public EstadoDispIntentoFraudeMoneda StadoDispIntetoFraudeMoneda = new EstadoDispIntentoFraudeMoneda();

        public EstadosDeSalud() { 
        
        }
    }
}
