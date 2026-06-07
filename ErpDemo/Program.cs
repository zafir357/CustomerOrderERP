using ErpDemo.Forms;

namespace ErpDemo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // initialise les paramètres Windows (DPI, thème...)
            Application.Run(new MainForm());       // lance la fenêtre principale
        }
    }
}