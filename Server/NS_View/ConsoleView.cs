using Server.NS_ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.NS_View
{
    class ConsoleView
    {
        #region Attributes
        private List<string> options;
        #endregion

        #region Properties
        public ViewModel DataContext { get; set; }
        

        #endregion

        #region Constuctors
        public ConsoleView(ViewModel dataContext)
        {
            DataContext = dataContext;
            options = new List<string>() { "Overview", "Connected", "Stats" };
            
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
            
            PrintNavigation();
            
            ShowView("Overview");

            DataContext.PlayerConnected += OnPlayerConnected;
            DataContext.PlayerDisconnected += OnPlayerDisconnected;
            DataContext.PlayerMoved += OnPlayerMove;

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

        private void PrintNavigation()
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
        }
        #endregion

        #region Execute
        private void ExecuteOverview()
        {
            options.Add("StartServer");
            options.Add("StopServer");
            PrintNavigation();
            Console.WriteLine("Port: " + DataContext.Port);
            Console.WriteLine("LocalIP: " + DataContext.LocalAddress);
            Console.WriteLine("PublicIP: " + DataContext.PublicAddress);
            Console.WriteLine("Running: " + DataContext.Running);
        }

        private void ExecuteConnected()
        {
            Console.Write("Connected");
        }

        private void ExecuteStats()
        {
            Console.Write("Stats");
        }

        private void ExecuteStartServer()
        {
            DataContext.StartCommand.Execute(this);
        }

        private void ExecuteStopServer()
        {
            DataContext.StopCommand.Execute(this);
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
            Console.WriteLine(player.Username + " connected.");
        }

        private void OnPlayerDisconnected(PlayerData player)
        {
            Console.WriteLine(player.Username + " disconnected.");
        }

        private void OnPlayerMove(PlayerData player)
        {
            Console.WriteLine("[({0}, {1}, {2}), ({3}, {4}, {5}), ({6}, {7}, {8})]",
                player.Position.X, player.Position.Y, player.Position.Z,
                player.Rotation.X, player.Rotation.Y, player.Rotation.Z,
                player.Scale.X, player.Scale.Y, player.Scale.Z);
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
