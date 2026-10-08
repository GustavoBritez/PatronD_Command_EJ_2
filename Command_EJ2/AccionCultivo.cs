using System;

namespace Command_EJ2
{
    public class AccionCultivo
    {
        public Guid Id { get; }
        public string NombreComando { get; }
        public DateTime FechaHora { get; }
        public bool Deshecho { get; }

        ///PRE: Recibe id (Guid), nombreComando (string), fechaHora (DateTime) y deshecho (bool).
        ///POST: Inicializa una nueva instancia de AccionCultivo con los datos del evento.
        public AccionCultivo(Guid id, string nombreComando, DateTime fechaHora, bool deshecho)
        {
            Id = id;
            NombreComando = nombreComando;
            FechaHora = fechaHora;
            Deshecho = deshecho;
        }
    }
}
