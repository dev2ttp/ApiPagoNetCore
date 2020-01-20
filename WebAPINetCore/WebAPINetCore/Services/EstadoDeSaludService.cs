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
            Globals.BloqueoEfectivo = false;
            Globals.BloqueoTransbank = false;
            //Globals.EstadoDeSaludMaquina = "4,0,0,0,0";
            var Estados = Globals.EstadoDeSaludMaquina.Split(";");
            foreach (var item in Estados)
            {
                var flags = item.Split(",");
                if (flags[0] == "1")// puerta abierta
                {

                    SaludMaquina.StadoPuerta.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoPuerta.Mensaje = "No se puede continuar con las Operaciones La puerta se encuentra abierta. Por favor espere quw un ejecutivo se acerque para solucionar el problema";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[4] == "1")
                    {
                        SaludMaquina.StadoPuerta.BloqueoTotal = true;
                    }  
                }

                if (flags[0] == "2")// corriente 
                {
                    SaludMaquina.StadoCorreinte.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoCorreinte.Mensaje = "Se ha detectado un fallo en la corriente, Mientras el problema persista el sistema se Imhabilitaraa";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[4] == "1")
                    {
                        SaludMaquina.StadoCorreinte.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "4")//Min billete
                {
                    SaludMaquina.StadoMinBillete.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMinBillete.Mensaje = "Los Billetes se encuentran en el minimo de billete, se recomienda cargar la maquina";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[4] == "1")
                    {
                        SaludMaquina.StadoMinBillete.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "8")// max billete
                {
                    SaludMaquina.StadoMaxBillete.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.Mensaje = "Los Billetes se encuentran en el maximo de billete, se recomienda descargar la maquina";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxBillete.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "10")// min moneda
                {
                    SaludMaquina.StadoMinMoneda.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.Mensaje = "Las Monedas se encuentran en el minimo de monedas permitidas, se recomienda cargar la maquina";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMinMoneda.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "10")// max monedas
                {
                    SaludMaquina.StadoMaxMonedas.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.Mensaje = "Las Monedas se encuentran en el maximo de monedas permitido, se recomienda descargar la maquina";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoMaxMonedas.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "40")//dispositive diferente
                {
                    SaludMaquina.StadoDispDiferente.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.Mensaje = "Alguno de los Dispositivos Actuales no coinciden con los registrados, Por favor verifique, he intente nuevamente";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispDiferente.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "80")// atasco seguro 
                {
                    SaludMaquina.StadoDispAtascadoS.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.Mensaje = "Un billete se ha atascado en el dispositivo";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoS.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "100")// atasco inseguro
                {
                    SaludMaquina.StadoDispAtascadoInS.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.Mensaje = "Un billete se ha atascado bruscamente en el dispositivo";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispAtascadoInS.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "200")// intento fraude de Billete
                {
                    SaludMaquina.StadoDispIntentoFraude.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.Mensaje = "se ha intentado cometer fraude con un billete  en el dispositivo";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntentoFraude.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "400")// cCaja de billete full
                {
                    SaludMaquina.StadoDispCajaFull.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.Mensaje = "se ha detectado que la caja de billete  en el dispositivo se encuentra full";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispCajaFull.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "800")/// Unidad Atascada
                {
                    SaludMaquina.StadoDispUnidadAtascada.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.Mensaje = "se ha detectado una unidad atascada  en el dispositivo";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispUnidadAtascada.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "1000")// Moneda atascada 
                {
                    SaludMaquina.StadoDispMonedaAtascada.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.Mensaje = "se ha detectado una moneda atascada  en el dispositivo";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispMonedaAtascada.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "2000")// Busqueda de moneda fallida
                {
                    SaludMaquina.StadoDispBusquedaFallida.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.Mensaje = "no se ha encontasdo la moneda solicitada ";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispBusquedaFallida.BloqueoTotal = true;
                    }
                }

                if (flags[0] == "4000")// Intento Fraude Moneda
                {
                    SaludMaquina.StadoDispIntetoFraudeMoneda.Activo = true;
                    if (flags[1] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.Mensaje = "Se ha encontrado un intento de fraude en la moneda ";
                    }
                    if (flags[2] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoEfectivo = true;
                        Globals.BloqueoEfectivo = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoTransbank = true;
                        Globals.BloqueoTransbank = true;
                    }
                    if (flags[3] == "1")
                    {
                        SaludMaquina.StadoDispIntetoFraudeMoneda.BloqueoTotal = true;
                    }
                }
                if (flags[0] == "8000")// Flotando
                {
                    SaludMaquina.Floating = true;                 
                }
            }

            Globals.SaludMaquina = SaludMaquina;
        }

    }
}
