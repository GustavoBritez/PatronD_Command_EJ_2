using BLL;

namespace Servicio
{
    public class ComandoModoMuteS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private bool _estadoAnterior;

        public string nombre => "Mute Transmisión";

        public ComandoModoMuteS(Transmisor_BLL transmisor)
        {
            _transmisor = transmisor;
        }

        public void ejecutar()
        {
            _estadoAnterior = _transmisor.EstaMuteado();
            _transmisor.SetMute(!_estadoAnterior);
        }

        public void deshacer()
        {
            _transmisor.SetMute(_estadoAnterior);
        }
    }
}
