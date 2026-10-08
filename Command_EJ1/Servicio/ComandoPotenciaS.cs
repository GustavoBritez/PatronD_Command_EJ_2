using BLL;

namespace Servicio
{
    public class ComandoPotenciaS : IComando
    {
        private readonly Transmisor_BLL _transmisor;
        private readonly int _nuevaPotencia;
        private int _potenciaAnterior;

        public string nombre => $"Ajustar Potencia a {_nuevaPotencia}W";

        ///PRE: Recibe transmisor (Transmisor_BLL) como receptor y nuevaPotencia (int) en vatios.
        ///POST: Inicializa una nueva instancia de ComandoPotenciaS con los parámetros indicados.
        public ComandoPotenciaS(Transmisor_BLL transmisor, int nuevaPotencia)
        {
            _transmisor = transmisor;
            _nuevaPotencia = nuevaPotencia;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda la potencia anterior y configura la nueva potencia en el transmisor.
        public void ejecutar()
        {
            _potenciaAnterior = _transmisor.ObtenerPotenciaActual();
            _transmisor.SetPotenciaAntena(_nuevaPotencia);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura la potencia anterior en el transmisor.
        public void deshacer()
        {
            _transmisor.SetPotenciaAntena(_potenciaAnterior);
        }
    }
}
