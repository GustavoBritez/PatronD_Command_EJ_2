namespace Command_EJ3
{
    public class ComandoBrazoMuestreo : IComando
    {
        private readonly BrazoMuestreo _brazo;
        private readonly string _nuevaPosicion;
        private string _posicionPrevia = "";

        public string nombre => $"Maniobra de Brazo: {_nuevaPosicion}";

        ///PRE: Recibe brazo (BrazoMuestreo) como receptor y nuevaPosicion (string) del actuador.
        ///POST: Inicializa el comando guardando la referencia y la posición robótica objetivo.
        public ComandoBrazoMuestreo(BrazoMuestreo brazo, string nuevaPosicion)
        {
            _brazo = brazo;
            _nuevaPosicion = nuevaPosicion;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda el estado previo y despliega o ajusta la pinza robótica.
        public void ejecutar()
        {
            _posicionPrevia = _brazo.ObtenerPosicion();
            _brazo.SetPosicion(_nuevaPosicion);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura la posición robótica anterior del manipulador abisal.
        public void deshacer()
        {
            _brazo.SetPosicion(_posicionPrevia);
        }
    }
}
