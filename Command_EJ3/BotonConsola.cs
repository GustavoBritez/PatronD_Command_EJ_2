namespace Command_EJ3
{
    public class BotonConsola
    {
        public int Numero { get; }
        public string Descripcion { get; set; }
        public IComando? Comando { get; private set; }

        ///PRE: Recibe numero (int) de slot y descripcion (string) opcional.
        ///POST: Inicializa una nueva instancia de BotonConsola.
        public BotonConsola(int numero, string descripcion = "")
        {
            Numero = numero;
            Descripcion = descripcion;
        }

        ///PRE: Recibe comando (IComando) a vincular al actuador de la consola.
        ///POST: No retorna valor. Asigna la referencia del comando a la propiedad Comando.
        public void AsignarComando(IComando comando)
        {
            Comando = comando;
        }

        public bool TieneComando => Comando != null;
    }
}
