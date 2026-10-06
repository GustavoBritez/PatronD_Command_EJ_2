namespace Servicio
{
    public class BotonConsola
    {
        public int Numero { get; }
        public string Descripcion { get; set; }
        public IComando? Comando { get; private set; }

        public BotonConsola(int numero, string descripcion = "")
        {
            Numero = numero;
            Descripcion = descripcion;
        }

        public void AsignarComando(IComando comando)
        {
            Comando = comando;
        }

        public bool TieneComando => Comando != null;
    }
}
