namespace Command_EJ2
{
    public class IluminacionCultivo
    {
        private string _modoEspectro = "Luz Natural (Standby)";

        ///PRE: Recibe espectro (string) indicando el régimen fotosintético deseado.
        ///POST: No retorna valor. Actualiza el modo del sistema LED de iluminación.
        public void SetEspectro(string espectro)
        {
            _modoEspectro = espectro;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una cadena (string) con la descripción del espectro lumínico activo.
        public string ObtenerEspectro()
        {
            return _modoEspectro;
        }
    }
}
