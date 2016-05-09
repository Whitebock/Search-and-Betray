using Server.NS_Utils;
using Server.NS_View;
using Server.NS_ViewModel;
using System;
using System.IO;
using System.Windows;

namespace Server
{
    class Programm
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Check for required DLL.
            if (!File.Exists("WhiteNet.dll"))
            {
                MessageBox.Show("Missing WhiteNet.dll");
                return;
            }

            // Check Arguments.
            bool gui = false;
            foreach (string argument in args)
            {
                switch (argument)
                {
                    case "-gui":
                        gui = true;
                        break;
                    default:
                        break;
                }
            }

            // Create Viewmodel.
            ViewModel viewmodel = new ViewModel();

            // Create view.
            if (gui || true)
            {
                Application a = new Application();
                View view = new View();
                view.DataContext = viewmodel;
                a.Run(view);
            }
            else
            {
                // Check if the programm has been started inside a console.
                bool hasConsole = ConsoleHelper.AttachConsole(-1);
                if (!hasConsole)
                {
                    // If not, create a new one.
                    ConsoleHelper.AllocConsole();
                }

                ConsoleView view = new ConsoleView(viewmodel);

                // Free Console after the programm ends.
                ConsoleHelper.FreeConsole();
            }
            Environment.Exit(0);
        }
    }
}
