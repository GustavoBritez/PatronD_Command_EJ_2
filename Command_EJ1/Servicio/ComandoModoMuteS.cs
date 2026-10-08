using BLL;

namespace Servicio
{
    public class ComandoModoMuteS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private bool _estadoAnterior;

        public string nombre => "Mute Transmisión";

        ///PRE: Recibe transmisor (Transmisor_BLL) como receptor del comando.
        ///POST: Inicializa una nueva instancia de ComandoModoMuteS asociando el receptor.
        public ComandoModoMuteS(Transmisor_BLL transmisor)
        {
            _transmisor = transmisor;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda el estado previo e invierte el estado de silencio del transmisor.
        public void ejecutar()
        {
            _estadoAnterior = _transmisor.EstaMuteado();
            _transmisor.SetMute(!_estadoAnterior);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el estado previo de silencio en el transmisor.
        public void deshacer()
        {
            _transmisor.SetMute(_estadoAnterior);
        }
    }
}
