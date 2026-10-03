using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProcesadorVoz_BLL
    {
        private int gananciaActual = 0;
        private bool efectoPitchActivo = false;

        public void ActivarEfectoPitch()
        {
            efectoPitchActivo = true;
        }

        public void DesactivarEfectoPitch()
        {
            efectoPitchActivo = false;
        }

        public void AjustarGanancia(int db)
        {
            gananciaActual = db;
        }

        public int ObtenerGananciaActual()
        {
            return gananciaActual;
        }

        public bool EstaPitchActivo()
        {
            return efectoPitchActivo;
        }
    }
}
