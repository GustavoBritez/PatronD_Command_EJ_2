namespace Command_EJ3
{
    public class SistemaBalasto
    {
        private int _profundidadMetros = 500;
        private bool _flotabilidadPositiva = false;

        ///PRE: Recibe metros (int) con la cota de inmersión objetivo.
        ///POST: No retorna valor. Inunda o vacía tanques fijando la profundidad.
        public void FijarProfundidad(int metros)
        {
            _profundidadMetros = metros;
            _flotabilidadPositiva = false;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Expulsa el lastre sólido para iniciar ascenso forzado inmediato.
        public void PurgarLastreEmergencia()
        {
            _profundidadMetros = 0;
            _flotabilidadPositiva = true;
        }

        ///PRE: Recibe metros (int) y flotabilidad (bool) a restaurar.
        ///POST: No retorna valor. Reestablece los parámetros de lastre previos.
        public void RestaurarBalasto(int metros, bool flotabilidad)
        {
            _profundidadMetros = metros;
            _flotabilidadPositiva = flotabilidad;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un entero (int) con la profundidad submarina actual en metros.
        public int ObtenerProfundidad()
        {
            return _profundidadMetros;
        }

        ///PRE: Ninguno.
        ///POST: Retorna un booleano (bool) indicando si el batiscafo se encuentra en ascenso de emergencia.
        public bool EstaEnFlotabilidadPositiva()
        {
            return _flotabilidadPositiva;
        }
    }
}
