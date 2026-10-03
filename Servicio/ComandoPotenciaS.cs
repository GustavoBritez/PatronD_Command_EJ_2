using BLL;

namespace Servicio
{
    public class ComandoPotenciaS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private readonly int _nuevaPotencia;
        private int _potenciaAnterior;

        public string nombre => $"Ajustar Potencia a {_nuevaPotencia}W";

        public ComandoPotenciaS(Transmisor_BLL transmisor, int nuevaPotencia)
        {
            _transmisor = transmisor;
            _nuevaPotencia = nuevaPotencia;
        }

        public void ejecutar()
        {
            _potenciaAnterior = _transmisor.ObtenerPotenciaActual();
            _transmisor.SetPotenciaAntena(_nuevaPotencia);
        }

        public void deshacer()
        {
            _transmisor.SetPotenciaAntena(_potenciaAnterior);
        }
    }
}
