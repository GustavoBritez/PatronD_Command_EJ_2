namespace BE
{
    public class Accion_BE
    {
        public Guid id;
        public string NombreComando;
        public DateTime FechaHora;
        public bool Desacido;

        ///PRE: Recibe id (Guid), nombreComando (string), fechaHora (DateTime) y desacido (bool).
        ///POST: Inicializa una nueva instancia de Accion_BE con los datos asignados.
        public Accion_BE(Guid id, string nombreComando, DateTime fechaHora, bool desacido)
        {
            this.id = id;
            NombreComando = nombreComando;
            FechaHora = fechaHora;
            Desacido = desacido;
        }
    }
}
