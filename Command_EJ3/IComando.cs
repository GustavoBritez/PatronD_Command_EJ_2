namespace Command_EJ3
{
    public interface IComando
    {
        string nombre { get; }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Ejecuta la maniobra encapsulada sobre los sistemas del batiscafo.
        void ejecutar();

        ///PRE: Ninguno.
        ///POST: No retorna valor. Revierte la maniobra restaurando el estado previo del submarino.
        void deshacer();
    }
}
