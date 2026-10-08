namespace Command_EJ2
{
    public class ComandoTemperatura : IComando
    {
        private readonly Climatizador _climatizador;
        private readonly int _nuevaTemperatura;
        private int _tempPrevia;

        public string nombre => $"Fijar Temperatura a {_nuevaTemperatura} °C";

        ///PRE: Recibe climatizador (Climatizador) como receptor y nuevaTemperatura (int) objetivo.
        ///POST: Inicializa el comando guardando la referencia y el valor objetivo de temperatura.
        public ComandoTemperatura(Climatizador climatizador, int nuevaTemperatura)
        {
            _climatizador = climatizador;
            _nuevaTemperatura = nuevaTemperatura;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena la temperatura previa y asigna el nuevo valor al climatizador.
        public void ejecutar()
        {
            _tempPrevia = _climatizador.ObtenerTemperatura();
            _climatizador.SetTemperatura(_nuevaTemperatura);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura la temperatura del climatizador al valor anterior.
        public void deshacer()
        {
            _climatizador.SetTemperatura(_tempPrevia);
        }
    }
}
