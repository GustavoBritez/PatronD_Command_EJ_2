namespace Command_EJ2
{
    public class ComandoEspectroLuz : IComando
    {
        private readonly IluminacionCultivo _iluminacion;
        private readonly string _nuevoEspectro;
        private string _espectroPrevio = "";

        public string nombre => $"Espectro LED: {_nuevoEspectro}";

        ///PRE: Recibe iluminacion (IluminacionCultivo) como receptor y nuevoEspectro (string) objetivo.
        ///POST: Inicializa el comando guardando la referencia y el espectro lumínico a activar.
        public ComandoEspectroLuz(IluminacionCultivo iluminacion, string nuevoEspectro)
        {
            _iluminacion = iluminacion;
            _nuevoEspectro = nuevoEspectro;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Guarda el espectro previo y conmuta al nuevo espectro lumínico.
        public void ejecutar()
        {
            _espectroPrevio = _iluminacion.ObtenerEspectro();
            _iluminacion.SetEspectro(_nuevoEspectro);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el espectro lumínico previo en el sistema de iluminación.
        public void deshacer()
        {
            _iluminacion.SetEspectro(_espectroPrevio);
        }
    }
}
