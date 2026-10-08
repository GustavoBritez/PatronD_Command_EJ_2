namespace Command_EJ3
{
    public class ComandoFocosAbisales : IComando
    {
        private readonly FocosAbisales _focos;
        private readonly string _nuevoModo;
        private string _modoPrevio = "";

        public string nombre => $"Focos: {_nuevoModo}";

        ///PRE: Recibe focos (FocosAbisales) como receptor y nuevoModo (string) de iluminación.
        ///POST: Inicializa el comando guardando la referencia y el régimen lumínico.
        public ComandoFocosAbisales(FocosAbisales focos, string nuevoModo)
        {
            _focos = focos;
            _nuevoModo = nuevoModo;
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Almacena la iluminación previa y conmuta al nuevo modo.
        public void ejecutar()
        {
            _modoPrevio = _focos.ObtenerModoLuz();
            _focos.SetModoLuz(_nuevoModo);
        }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Restaura el régimen lumínico submarino anterior.
        public void deshacer()
        {
            _focos.SetModoLuz(_modoPrevio);
        }
    }
}
