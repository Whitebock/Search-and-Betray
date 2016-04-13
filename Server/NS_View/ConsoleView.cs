using Server.NS_Utils;
using Server.NS_ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NS_View
{
    class ConsoleView
    {
        #region Attributes
        private ViewModel dataContext;

        public ViewModel DataContext
        {
            get { return dataContext; }
            set { dataContext = value; }
        }

        #endregion

        #region Properties

        #endregion

        #region Constuctors
        public ConsoleView(ViewModel dataContext)
        {
            DataContext = dataContext;
            //Check if the programm has been started inside a console
            bool hasConsole = ConsoleHelper.AttachConsole(-1);
            if (!hasConsole)
            {
                //If not, create a new one
                ConsoleHelper.AllocConsole();
            }

            Console.WriteLine();
            PrintLine();
            Console.WriteLine("CCC Dedicated Server");
            PrintLine();

            dataContext.StartCommand.Execute(this);
            Console.WriteLine("Server started at: ");
            Console.WriteLine("Port: " + dataContext.Port);
            Console.WriteLine("LocalIP: " + dataContext.LocalAddress);
            Console.WriteLine("PublicIP: " + dataContext.PublicAddress);
            PrintLine();

            dataContext.Clients.CollectionChanged += OnNewClient;
            //dataContext.StopCommand.Execute(this);
            //Console.WriteLine("Server stopped");
            //Console.ReadKey(true);
            //while (true){ }
            Debug.WriteLine("Started");
            Console.ReadLine();
        }

        #endregion

        private void OnNewClient(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CHANGE!!");
            foreach (var s in e.NewItems)
            {
                Console.WriteLine(s);
            }
        }

        #region Formatting Methodes
        private void PrintLine(char c = '-')
        {
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write(c);
            }
        }
        #endregion
    }
}
