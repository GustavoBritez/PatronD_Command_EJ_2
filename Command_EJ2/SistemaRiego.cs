namespace Command_EJ2
{
    public class SistemaRiego
    {
        private bool _bombaActiva = false;
        private int _dosificacionMl = 0;

        ///PRE: Recibe ml (int) con la cantidad de solución hidropónica a dosificar.
        ///POST: No retorna valor. Activa la bomba dosificadora y registra los mililitros configurados.
        public void ActivarRiego(int ml)
        {
            _bombaActiva = true;
            _dosificacionMl = ml;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Detiene la bomba y coloca la dosificación en 0.
        public void DetenerRiego()
        {
            _bombaActiva = false;
            _dosificacionMl = 0;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un booleano (bool) indicando si la bomba de riego está encendida.
        public bool EstaRiegoActivo()
        {
            return _bombaActiva;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un entero (int) con los mililitros de solución nutritiva suministrados.
        public int ObtenerDosificacion()
        {
            return _dosificacionMl;
        }
    }
}
