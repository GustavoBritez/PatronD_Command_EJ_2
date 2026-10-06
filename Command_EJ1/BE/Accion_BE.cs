namespace BE
{
    public class Accion_BE
    {
        public Guid id;
        public string NombreComando;
        public DateTime FechaHora;
        public bool Desacido;

        public Accion_BE(Guid id, string nombreComando, DateTime fechaHora, bool desacido)
        {
            this.id = id;
            NombreComando = nombreComando;
            FechaHora = fechaHora;
            Desacido = desacido;
        }
    }
}
