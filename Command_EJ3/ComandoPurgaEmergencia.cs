namespace Command_EJ3
{
    public class ComandoPurgaEmergencia : IComando
    {
        private readonly SistemaBalasto _balasto;
        private readonly BrazoMuestreo _brazo;
        private int _profundidadPrevia;
        private bool _flotabilidadPrevia;
        private string _posicionBrazoPrevia = "";

        public string nombre => "Purga de Lastre y Ascenso de Emergencia";

        ///PRE: Recibe balasto (SistemaBalasto) y brazo (BrazoMuestreo) como receptores de la maniobra crítica.
        ///POST: Inicializa el comando guardando las referencias de ambos sistemas.
        public ComandoPurgaEmergencia(SistemaBalasto balasto, BrazoMuestreo brazo)
        {
            _balasto = balasto;
            _brazo = brazo;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena el estado previo, retrae el brazo y expulsa el lastre para flotabilidad positiva.
        public void ejecutar()
        {
            _profundidadPrevia = _balasto.ObtenerProfundidad();
            _flotabilidadPrevia = _balasto.EstaEnFlotabilidadPositiva();
            _posicionBrazoPrevia = _brazo.ObtenerPosicion();

            _brazo.SetPosicion("Retraído y Asegurado por Protocolo de Ascenso");
            _balasto.PurgarLastreEmergencia();
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el lastre de profundidad y la posición de la pinza a sus valores previos.
        public void deshacer()
        {
            _balasto.RestaurarBalasto(_profundidadPrevia, _flotabilidadPrevia);
            _brazo.SetPosicion(_posicionBrazoPrevia);
        }
    }
}
