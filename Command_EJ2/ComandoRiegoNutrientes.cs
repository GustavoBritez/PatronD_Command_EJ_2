namespace Command_EJ2
{
    public class ComandoRiegoNutrientes : IComando
    {
        private readonly SistemaRiego _sistemaRiego;
        private readonly int _mililitros;
        private bool _bombaPrevia;
        private int _mlPrevio;

        public string nombre => $"Inyección Riego Nutritivo ({_mililitros} ml)";

        ///PRE: Recibe sistemaRiego (SistemaRiego) como receptor y mililitros (int) a suministrar.
        ///POST: Inicializa el comando guardando la referencia y la dosis requerida.
        public ComandoRiegoNutrientes(SistemaRiego sistemaRiego, int mililitros)
        {
            _sistemaRiego = sistemaRiego;
            _mililitros = mililitros;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena el estado previo y activa el pulso de riego con la dosis indicada.
        public void ejecutar()
        {
            _bombaPrevia = _sistemaRiego.EstaRiegoActivo();
            _mlPrevio = _sistemaRiego.ObtenerDosificacion();
            _sistemaRiego.ActivarRiego(_mililitros);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el estado previo de la bomba y la dosificación en el receptor.
        public void deshacer()
        {
            if (_bombaPrevia)
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
