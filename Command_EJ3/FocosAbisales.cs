namespace Command_EJ3
{
    public class FocosAbisales
    {
        private string _modoLuz = "Luz Estroboscópica Guía (Bajo Consumo)";

        ///PRE: Recibe modo (string) con la intensidad lumínica submarina requerida.
        ///POST: No retorna valor. Conmuta los proyectores de alta presión.
        public void SetModoLuz(string modo)
        {
            _modoLuz = modo;
        }

        ///PRE: Ninguno.
        ///POST: Retorna una cadena (string) con el régimen actual de los focos de penetración abisal.
        public string ObtenerModoLuz()
        {
            return _modoLuz;
        }
    }
}
