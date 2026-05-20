using CustomGameDB.Models;
using HandyControl.Tools;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace CustomGameDB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Usuario usuarioLogeado {  get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();

            bool? dialogResult = loginWindow.ShowDialog();
            if (dialogResult == true)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                this.Shutdown();


            }

            


        }

    }

}
