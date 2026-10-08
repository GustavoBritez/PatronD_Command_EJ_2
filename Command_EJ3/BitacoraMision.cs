using System;

namespace Command_EJ3
{
    public class BitacoraMision
    {
        public Guid Id { get; }
        public string Maniobra { get; }
        public DateTime Timestamp { get; }
        public bool Revertida { get; }

        ///PRE: Recibe id (Guid), maniobra (string), timestamp (DateTime) y revertida (bool).
        ///POST: Inicializa un registro de bitácora para la travesía oceanográfica.
        public BitacoraMision(Guid id, string maniobra, DateTime timestamp, bool revertida)
        {
            Id = id;
            Maniobra = maniobra;
            Timestamp = timestamp;
            Revertida = revertida;
        }
    }
}
