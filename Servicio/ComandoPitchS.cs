using BLL;

namespace Servicio
{
    public class ComandoPitchS : IComando
    {
        private readonly ProcesadorVoz_BLL _procesadorVoz;
        private bool _estadoAnterior;

        public string nombre => "Efecto Pitch Voz";

        public ComandoPitchS(ProcesadorVoz_BLL procesadorVoz)
        {
            _procesadorVoz = procesadorVoz;
        }

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
