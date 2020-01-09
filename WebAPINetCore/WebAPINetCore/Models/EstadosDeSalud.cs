using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class EstadoPuerta 
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo  { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoCorriente
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMinimoBillete
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMaxBillete
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoMinMonedas
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoMaxMonedas
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispDiferente
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispAtascoSeguro
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispAtascoInSeguro
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispIntentoFraudeB
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispCajaFull
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispUnidadAtascada    
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }

    public class EstadoDispMonedaAtascada
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispBusquedaFallida
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadoDispIntentoFraudeMoneda
    {
        public bool Activo { get; set; }
        public string Mensaje { get; set; }
        public bool BloqueoEfectivo { get; set; }
        public bool BloqueoTransbank { get; set; }
        public bool BloqueoTotal { get; set; }
    }
    public class EstadosDeSalud
    {
        public EstadoPuerta StadoPuerta { get; set; }
        public EstadoCorriente StadoCorreinte { get; set; }
        public EstadoMinimoBillete StadoMinBillete { get; set; }
        public EstadoMaxBillete StadoMaxBillete { get; set; }
        public EstadoMinMonedas StadoMinMoneda { get; set; }
        public EstadoMaxMonedas StadoMaxMonedas { get; set; }
        public EstadoDispDiferente StadoDispDiferente { get; set; }
        public EstadoDispAtascoSeguro StadoDispAtascadoS { get; set; }
        public EstadoDispAtascoInSeguro StadoDispAtascadoInS { get; set; }
        public EstadoDispIntentoFraudeB StadoDispIntentoFraude { get; set; }
        public EstadoDispCajaFull StadoDispCajaFull { get; set; }
        public EstadoDispUnidadAtascada StadoDispUnidadAtascada { get; set; }
        public EstadoDispMonedaAtascada StadoDispMonedaAtascada { get; set; }
        public EstadoDispBusquedaFallida StadoDispBusquedaFallida { get; set; }
        public EstadoDispIntentoFraudeMoneda StadoDispIntetoFraudeMoneda { get; set; }

        public EstadosDeSalud() {
            StadoCorreinte = new EstadoCorriente();
            StadoDispAtascadoInS = new EstadoDispAtascoInSeguro();
            StadoDispAtascadoS = new EstadoDispAtascoSeguro();
            StadoDispBusquedaFallida = new EstadoDispBusquedaFallida();
            StadoDispCajaFull = new EstadoDispCajaFull();
            StadoDispDiferente = new EstadoDispDiferente();
            StadoDispIntentoFraude = new EstadoDispIntentoFraudeB();
            StadoDispIntetoFraudeMoneda = new EstadoDispIntentoFraudeMoneda();
            StadoDispMonedaAtascada = new EstadoDispMonedaAtascada();
            StadoDispUnidadAtascada = new EstadoDispUnidadAtascada();
            StadoMaxBillete = new EstadoMaxBillete();
            StadoMaxMonedas = new EstadoMaxMonedas();
            StadoMinBillete = new EstadoMinimoBillete();
            StadoMinMoneda = new EstadoMinMonedas();
            StadoPuerta = new EstadoPuerta();
        }
    }
}
