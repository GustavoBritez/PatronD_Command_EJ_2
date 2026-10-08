namespace Command_EJ2
{
    public class BotonPanel
    {
        public int Numero { get; }
        public string Descripcion { get; set; }
        public IComando? Comando { get; private set; }

        ///PRE: Recibe numero (int) identificador del botón y opcionalmente descripcion (string).
        ///POST: Inicializa una nueva instancia de BotonPanel.
        public BotonPanel(int numero, string descripcion = "")
        {
            Numero = numero;
            Descripcion = descripcion;
        }

        ///PRE: Recibe comando (IComando) a vincular al botón.
        ///POST: No retorna valor. Asigna la referencia del comando a la propiedad Comando.
        public void AsignarComando(IComando comando)
        {
            Comando = comando;
        }

        public bool TieneComando => Comando != null;
    }
}
