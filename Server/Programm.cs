using Server.NS_Utils;
using Server.NS_View;
using Server.NS_ViewModel;
using System;
using System.Windows;

namespace Server
{
    class Programm
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Check Arguments
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

            //Create Viewmodel
            ViewModel viewmodel = new ViewModel();

            //Create view
            if (gui)
            {
                Application a = new Application();
                View view = new View();
                view.DataContext = viewmodel;
                a.Run(view);
            }
            else
            {
                ConsoleView view = new ConsoleView(viewmodel);

                ConsoleHelper.FreeConsole();
            }
            Environment.Exit(0);
        }
    }
}
