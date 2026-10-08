namespace Command_EJ3
{
    public class ComandoInmersion : IComando
    {
        private readonly SistemaBalasto _balasto;
        private readonly int _nuevaProfundidad;
        private int _profundidadPrevia;
        private bool _flotabilidadPrevia;

        public string nombre => $"Inmersión a {_nuevaProfundidad} metros";

        ///PRE: Recibe balasto (SistemaBalasto) como receptor y nuevaProfundidad (int) en metros.
        ///POST: Inicializa el comando guardando la referencia y la cota de inmersión.
        public ComandoInmersion(SistemaBalasto balasto, int nuevaProfundidad)
        {
            _balasto = balasto;
            _nuevaProfundidad = nuevaProfundidad;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena la cota previa e inunda tanques hacia la nueva profundidad.
        public void ejecutar()
        {
            _profundidadPrevia = _balasto.ObtenerProfundidad();
            _flotabilidadPrevia = _balasto.EstaEnFlotabilidadPositiva();
            _balasto.FijarProfundidad(_nuevaProfundidad);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura la cota de profundidad previa en el sistema de balasto.
        public void deshacer()
        {
            _balasto.RestaurarBalasto(_profundidadPrevia, _flotabilidadPrevia);
        }
    }
}
