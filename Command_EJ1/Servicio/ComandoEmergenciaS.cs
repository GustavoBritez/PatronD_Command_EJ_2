using BLL;

namespace Servicio
{
    public class ComandoEmergenciaS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private int _potenciaAnterior;
        private bool _muteAnterior;

        public string nombre => "Corte de Emergencia";

        public ComandoEmergenciaS(Transmisor_BLL transmisor)
        {
            _transmisor = transmisor;
        }

        public void ejecutar()
        {
            _potenciaAnterior = _transmisor.ObtenerPotenciaActual();
            _muteAnterior = _transmisor.EstaMuteado();

            _transmisor.SetPotenciaAntena(0);
            _transmisor.SetMute(true);
        }

        public void deshacer()
        {
            _transmisor.SetPotenciaAntena(_potenciaAnterior);
            _transmisor.SetMute(_muteAnterior);
        }
    }
}
