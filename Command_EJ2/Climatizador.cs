namespace Command_EJ2
{
    public class Climatizador
    {
        private int _temperaturaActual = 22;
        private bool _compuertasAbiertas = false;

        ///PRE: Recibe grados (int) con la temperatura objetivo a fijar en el domo.
        ///POST: No retorna valor. Actualiza la temperatura interna registrada.
        public void SetTemperatura(int grados)
        {
            _temperaturaActual = grados;
        }

        ///PRE: Recibe abiertas (bool) indicando si se abren o cierran las compuertas de ventilación.
        ///POST: No retorna valor. Actualiza el estado de apertura de las compuertas.
        public void SetCompuertas(bool abiertas)
        {
            _compuertasAbiertas = abiertas;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un entero (int) con la temperatura actual en grados Celsius.
        public int ObtenerTemperatura()
        {
            return _temperaturaActual;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un booleano (bool) indicando si las compuertas de ventilación están abiertas.
        public bool EstanCompuertasAbiertas()
        {
            return _compuertasAbiertas;
        }
    }
}
