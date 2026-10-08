namespace Command
{
    internal static class Program
    {
        ///PRE: Ninguno. Punto de entrada principal de la aplicación.
        ///POST: No retorna valor. Inicializa la configuración de la aplicación e inicia Form1.
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}