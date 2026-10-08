namespace Command_EJ2
{
    public interface IComando
    {
        string nombre { get; }

        ///PRE: Ninguno.
        ///POST: No retorna valor. Ejecuta la operación encapsulada sobre los receptores del invernadero.
        void ejecutar();

        ///PRE: Ninguno.
        ///POST: No retorna valor. Revierte la operación restaurando las condiciones previas del cultivo.
        void deshacer();
    }
}
