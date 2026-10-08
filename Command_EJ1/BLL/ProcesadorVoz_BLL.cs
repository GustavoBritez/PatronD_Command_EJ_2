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

        ///PRE: Ninguno.
        ///POST: No retorna valor. Establece el estado de efectoPitchActivo en true.
        public void ActivarEfectoPitch()
        {
            efectoPitchActivo = true;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Establece el estado de efectoPitchActivo en false.
        public void DesactivarEfectoPitch()
        {
            efectoPitchActivo = false;
        }

        ///PRE: Recibe db (int) con el nivel de decibelios a aplicar.
        ///POST: No retorna valor. Actualiza el valor de gananciaActual.
        public void AjustarGanancia(int db)
        {
            gananciaActual = db;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un entero (int) con la ganancia actual en decibelios.
        public int ObtenerGananciaActual()
        {
            return gananciaActual;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un booleano (bool) indicando si el efecto de tono (pitch) está activado.
        public bool EstaPitchActivo()
        {
            return efectoPitchActivo;
        }
    }
}
