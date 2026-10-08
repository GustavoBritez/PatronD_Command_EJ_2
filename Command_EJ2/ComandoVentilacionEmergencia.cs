namespace Command_EJ2
{
    public class ComandoVentilacionEmergencia : IComando
    {
        private readonly Climatizador _climatizador;
        private readonly SistemaRiego _sistemaRiego;
        private bool _compuertasPrevias;
        private bool _riegoPrevio;
        private int _mlPrevio;

        public string nombre => "Purga y Ventilación de Emergencia";

        ///PRE: Recibe climatizador (Climatizador) y sistemaRiego (SistemaRiego) como receptores.
        ///POST: Inicializa el comando guardando las referencias de ambos receptores.
        public ComandoVentilacionEmergencia(Climatizador climatizador, SistemaRiego sistemaRiego)
        {
            _climatizador = climatizador;
            _sistemaRiego = sistemaRiego;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena el estado previo, abre compuertas y corta bombas por seguridad.
        public void ejecutar()
        {
            _compuertasPrevias = _climatizador.EstanCompuertasAbiertas();
            _riegoPrevio = _sistemaRiego.EstaRiegoActivo();
            _mlPrevio = _sistemaRiego.ObtenerDosificacion();

            _climatizador.SetCompuertas(true);
            _sistemaRiego.DetenerRiego();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el estado previo de las compuertas y de la bomba de riego.
        public void deshacer()
        {
            _climatizador.SetCompuertas(_compuertasPrevias);
            if (_riegoPrevio)
            {
                _sistemaRiego.ActivarRiego(_mlPrevio);
            }
            else
            {
                _sistemaRiego.DetenerRiego();
            }
        }
    }
}
