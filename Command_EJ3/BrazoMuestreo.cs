namespace Command_EJ3
{
    public class BrazoMuestreo
    {
        private string _posicionActual = "Retraído y Bloqueado en Casco";

        ///PRE: Recibe posicion (string) con el nuevo estado del actuador robótico.
        ///POST: No retorna valor. Actualiza la posición de la pinza de muestreo.
        public void SetPosicion(string posicion)
        {
            _posicionActual = posicion;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una cadena (string) con el estado operativo de la pinza robótica.
        public string ObtenerPosicion()
        {
            return _posicionActual;
        }
    }
}
