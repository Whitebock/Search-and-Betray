using Server.NS_ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;

namespace Server.NS_View
{
    class ConsoleView
    {
        #region Attributes
        private ObservableCollection<string> options;
        private string lastView;
        private ObservableCollection<string> standardOptions;
        #endregion

        #region Properties
        public ViewModel DataContext { get; set; }
        
        #endregion

        #region Constuctors
        public ConsoleView(ViewModel dataContext)
        {
            // Initalize
            DataContext = dataContext;
            options = new ObservableCollection<string>();
            standardOptions = new ObservableCollection<string>() { "Overview", "Connected", "Stats", "StartServer", "StopServer" };
            options.CollectionChanged += PrintNavigation;

            // Clear console, so formatting is correct.
            Console.Clear();

            // Disable cursor
            Console.CursorVisible = false;

            // Fix console size.
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;

            // Header
            Console.Title = "CCC Dedicated Server";
            PrintLine();
            Console.WriteLine("CCC Dedicated Server");
            PrintLine();

            // Add start menu
            foreach (string s in standardOptions)
            {
                options.Add(s);
            }

            // Add event handlers
            DataContext.PlayerConnected += OnPlayerConnected;
            DataContext.PlayerDisconnected += OnPlayerDisconnected;
            DataContext.PlayerMoved += OnPlayerMove;
            
            ShowView("Overview");

            while (true)
            {
                int menu = ChooseMenu();
                if (menu > 0 && menu < options.Count + 1)
                {
                    string viewname = options[menu - 1];
                    ShowView(viewname);
                }
            }
        }

        #endregion

        #region Menu

        private int ChooseMenu()
        {
            int charcode = 0;
            do
            {
                charcode = (int)Console.ReadKey(true).KeyChar;
            } while (charcode < 47 || charcode > 59);

            return (charcode % 47) - 1;
        }

        private void PrintNavigation(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Clear Menu
            Console.SetCursorPosition(0, 3);
            for (int i = 0; i < options.Count; i++)
            {
                PrintLine(' ');
            }

            // Write Menu
            Console.SetCursorPosition(0, 3);
            for (int i = 0; i < options.Count; i++)
            {
                if (CanExecute(options[i]))
                    Console.ForegroundColor = ConsoleColor.Gray;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.CursorLeft = 2;
                Console.WriteLine((i + 1) + " " + options[i]);
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            PrintLine();
        }

        private void ShowView(string viewname)
        {
            options.Clear();
            foreach (string s in standardOptions)
            {
                options.Add(s);
            }
            lastView = viewname;
            if (!CanExecute(viewname))
                return;

            int cur = 4 + options.Count;

            // Clear view screen.
            Console.SetCursorPosition(0, cur);
            for (int i = 0; i < Console.BufferHeight - cur - 1; i++)
            {
                PrintLine(' ');
            }
            // Reset position.
            Console.SetCursorPosition(0, cur);

            // Show view.
            Execute(viewname);

            // Update Navigation
            PrintNavigation(this, null);
        }
        #endregion

        #region Execute
        private void ExecuteOverview()
        {
            Console.WriteLine("Port: " + DataContext.Port);
            Console.WriteLine("LocalIP: " + DataContext.LocalAddress);
            Console.WriteLine("PublicIP: " + DataContext.PublicAddress);
            Console.WriteLine("Running: " + DataContext.Running);
        }

        private void ExecuteConnected()
        {
            foreach (PlayerData player in DataContext.Clients)
            {
                Console.WriteLine("[{0}] {1}", player.ID, player.Username);
            }
        }

        private void ExecuteStats()
        {
            foreach (PlayerData player in DataContext.Clients)
            {
                Console.WriteLine("[{0}] {1}HP, {2}AR", player.ID, player.Health, player.Armour);
            }
        }

        private void ExecuteStartServer()
        {
            DataContext.StartCommand.Execute(this);
            ShowView(options[0]);
        }

        private void ExecuteStopServer()
        {
            DataContext.StopCommand.Execute(this);
            ShowView(options[0]);
        }

        #endregion

        #region CanExecute
        private bool CanExecuteConnected()
        {
            return DataContext.Clients.Count > 0;
        }

        private bool CanExecuteStats()
        {
            return DataContext.Clients.Count > 0;
        }

        private bool CanExecuteStartServer()
        {
            return !DataContext.Running;
        }

        private bool CanExecuteStopServer()
        {
            return DataContext.Running;
        }

        #endregion

        private void Execute(string methodname)
        {
            MethodInfo method = this.GetType().GetMethod("Execute" + methodname, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
                method.Invoke(this, null);
            else
                throw new Exception("Method unknown");
        }

        private bool CanExecute(string methodname)
        {
            MethodInfo method = this.GetType().GetMethod("CanExecute" + methodname, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
            {
                bool canexecute = (bool)method.Invoke(this, null);
                return canexecute;
            }
            return true;
        }

        #region EventHandlers
        private void OnPlayerConnected(PlayerData player)
        {
            PrintNavigation(this, null);
            ShowView(lastView);
        }

        private void OnPlayerDisconnected(PlayerData player)
        {
            PrintNavigation(this, null);
            ShowView(lastView);
        }

        private void OnPlayerMove(PlayerData player)
        {

        }

        #endregion

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
