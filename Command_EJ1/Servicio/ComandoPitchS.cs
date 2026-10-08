using BLL;

namespace Servicio
{
    public class ComandoPitchS : IComando
    {
        private readonly ProcesadorVoz_BLL _procesadorVoz;
        private bool _estadoAnterior;

        public string nombre => "Efecto Pitch Voz";

        ///PRE: Recibe procesadorVoz (ProcesadorVoz_BLL) como receptor del comando.
        ///POST: Inicializa una nueva instancia de ComandoPitchS asociando el receptor.
        public ComandoPitchS(ProcesadorVoz_BLL procesadorVoz)
        {
            _procesadorVoz = procesadorVoz;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda el estado previo y conmuta la activación del efecto pitch en el procesador.
        public void ejecutar()
        {
            _estadoAnterior = _procesadorVoz.EstaPitchActivo();
            if (_estadoAnterior)
            {
                _procesadorVoz.DesactivarEfectoPitch();
            }
            else
            {
                _procesadorVoz.ActivarEfectoPitch();
            }
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el estado previo del efecto pitch en el procesador.
        public void deshacer()
        {
            if (_estadoAnterior)
            {
                _procesadorVoz.ActivarEfectoPitch();
            }
            else
            {
                _procesadorVoz.DesactivarEfectoPitch();
            }
        }
    }
}
