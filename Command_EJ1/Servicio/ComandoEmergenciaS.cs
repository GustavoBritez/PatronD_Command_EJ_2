using BLL;

namespace Servicio
{
    public class ComandoEmergenciaS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private int _potenciaAnterior;
        private bool _muteAnterior;

        public string nombre => "Corte de Emergencia";

        ///PRE: Recibe transmisor (Transmisor_BLL) como receptor del comando.
        ///POST: Inicializa una nueva instancia de ComandoEmergenciaS asociando el receptor.
        public ComandoEmergenciaS(Transmisor_BLL transmisor)
        {
            _transmisor = transmisor;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda el estado previo y establece potencia en 0 y silencio en true.
        public void ejecutar()
        {
            _potenciaAnterior = _transmisor.ObtenerPotenciaActual();
            _muteAnterior = _transmisor.EstaMuteado();

            _transmisor.SetPotenciaAntena(0);
            _transmisor.SetMute(true);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura la potencia y estado de silencio previos en el transmisor.
        public void deshacer()
        {
            _transmisor.SetPotenciaAntena(_potenciaAnterior);
            _transmisor.SetMute(_muteAnterior);
        }
    }
}
