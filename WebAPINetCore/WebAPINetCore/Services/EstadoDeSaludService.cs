using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPINetCore.Models;

namespace WebAPINetCore.Services
{
    public class EstadoDeSaludService
    {


        public void DescifrarEstadoDeSalud() {

            //estadoSalud
            //public const int exc_puerta = 0x01;
            //public const int exc_corriente = 0x02;
            //public const int exc_minBillete = 0x04;
            //public const int exc_maxBillete = 0x08;
            //public const int exc_minMonedas = 0x10;
            //public const int exc_maxMonedas = 0x20;
            //public const int exc_disDiferente = 0x40;
            ////smartPayout
            //public const int exc_atascoSeguro = 0x80;
            //public const int exc_atascoInSeguro = 0x100;
            //public const int exc_intentoFraudeB = 0x200;
            //public const int exc_cajaFull = 0x400;
            //public const int exc_unidadAtascada = 0x800;
            ////smartHopper
            //public const int exc_monedaAtascada = 0x1000;
            //public const int exc_busquedaFallida = 0x2000;
            //public const int exc_intentoFraudeM = 0x4000;
            ////4,0,0,0,0; 8,0,0,0,0; 10,0,0,0,0
            ///
            EstadosDeSalud SaludMaquina = new EstadosDeSalud();
            var Estados = Globals.EstadoDeSaludMaquina.Split(";");
            foreach (var item in Estados)
            {
                var flags = item.Split(",");
                if (item.Contains("1,"))// puerta abierta
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoPuerta.Mensaje = "No se puede continuar con las Operaciones La puerta se encuentra abierta. Por favor espere quw un ejecutivo se acerque para solucionar el problema";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoEfectivo = true; 
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoTotal = true;
                    }  
                }

                if (item.Contains("2,"))// corriente 
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoCorreinte.Mensaje = "Se ha detectado un fallo en la corriente, Mientras el problema persista el sistema se Imhabilitaraa";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoTotal = true;
                    }
                }

                if (item.Contains("4,"))//Min billete
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoMinBillete.Mensaje = "Los Billetes se encuentran en el minimo de billete, se recomienda cargar la maquina";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoTotal = true;
                    }
                }

                if (item.Contains("8,"))// max billete
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.Mensaje = "Los Billetes se encuentran en el maximo de billete, se recomienda descargar la maquina";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoTotal = true;
                    }
                }

                if (item.Contains("10,"))// min moneda
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.Mensaje = "Las Monedas se encuentran en el minimo de monedas permitidas, se recomienda cargar la maquina";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoTotal = true;
                    }
                }

                if (item.Contains("20,"))// max monedas
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.Mensaje = "Las Monedas se encuentran en el maximo de monedas permitido, se recomienda descargar la maquina";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoTotal = true;
                    }
                }

                if (item.Contains("40,"))//dispositive diferente
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.Mensaje = "Alguno de los Dispositivos Actuales no coinciden con los registrados, Por favor verifique, he intente nuevamente";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoTotal = true;
                    }
                }

                if (item.Contains("80,"))// atasco seguro 
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.Mensaje = "Un billete se ha atascado en el dispositivo";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoTotal = true;
                    }
                }

                if (item.Contains("100,"))// atasco inseguro
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.Mensaje = "Un billete se ha atascado bruscamente en el dispositivo";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoTotal = true;
                    }
                }

                if (item.Contains("200,"))// intento fraude de Billete
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.Mensaje = "se ha intentado cometer fraude con un billete  en el dispositivo";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoTotal = true;
                    }
                }

                if (item.Contains("400,"))// cCaja de billete full
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.Mensaje = "se ha detectado que la caja de billete  en el dispositivo se encuentra full";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoTotal = true;
                    }
                }

                if (item.Contains("800,"))/// Unidad Atascada
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.Mensaje = "se ha detectado una unidad atascada  en el dispositivo";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoTotal = true;
                    }
                }

                if (item.Contains("1000,"))// Moneda atascada 
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.Mensaje = "se ha detectado una moneda atascada  en el dispositivo";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoTotal = true;
                    }
                }

                if (item.Contains("2000,"))// Busqueda de moneda fallida
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.Mensaje = "no se ha encontasdo la moneda solicitada ";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoTotal = true;
                    }
                }

                if (item.Contains("4000,"))// Intento Fraude Moneda
                {
                    if (flags[0] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.Mensaje = "Se ha encontrado un intento de fraude en la moneda ";
                    }
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoEfectivo = true;
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoTotal = true;
                    }
                }
            }
        }

    }
}
