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

        public void SetMute( bool mutear )
        {
            isMuted = mutear;
        }

        public void SetPotenciaAntena(int potencia)
        {
            potenciaActual = potencia;
        }

        public int ObtenerPotenciaActual()
        {
            return potenciaActual;
        }

        public bool EstaMuteado()
        {
            return isMuted;
        }
    }
}
