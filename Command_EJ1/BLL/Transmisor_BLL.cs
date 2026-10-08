using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Transmisor_BLL
    {
        private int potenciaActual = 100;
        private bool isMuted = false;

        ///PRE: Recibe mutear (bool) indicando si se debe silenciar la transmisión.
        ///POST: No retorna valor. Actualiza el estado de isMuted.
        public void SetMute( bool mutear )
        {
            isMuted = mutear;
        }

        ///PRE: Recibe potencia (int) con el nivel de vatios para la antena.
        ///POST: No retorna valor. Actualiza el valor de potenciaActual.
        public void SetPotenciaAntena(int potencia)
        {
            potenciaActual = potencia;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un entero (int) con la potencia actual en vatios.
        public int ObtenerPotenciaActual()
        {
            return potenciaActual;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un booleano (bool) indicando si la transmisión se encuentra silenciada.
        public bool EstaMuteado()
        {
            return isMuted;
        }
    }
}
