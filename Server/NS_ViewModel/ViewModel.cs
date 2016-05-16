﻿using Server.NS_Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using WhiteNet;


namespace Server.NS_ViewModel
{
    public class ViewModel : ViewModelBase
    {
        #region Attributes
        private RelayCommand startCommand;
        private RelayCommand stopCommand;

        private ObservableCollection<PlayerData> clients;

        private IPAddress localAddress;
        private IPAddress publicAddress;
        private int port;
        private bool running;
        private DateTime lastSync;

        private CCC_Server server;
        #endregion


        #region Properties
        public RelayCommand StartCommand
        {
            get { return startCommand; }
        }
        public RelayCommand StopCommand
        {
            get { return stopCommand; }
        }

        public ObservableCollection<PlayerData> Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(); }
        }
        public IPAddress LocalAddress
        {
            get { return localAddress; }
            set { localAddress = value; OnPropertyChanged(); }
        }
        public IPAddress PublicAddress
        {
            get { return publicAddress; }
            set { publicAddress = value; OnPropertyChanged(); }
        }
        public bool Running
        {
            get { return running; }
            set { running = value; OnPropertyChanged(); }
        }
        public DateTime LastSync
        {
            get { return lastSync; }
            set { lastSync = value; OnPropertyChanged(); }
        }
        #endregion

        #region Events
        public delegate void PlayerEvent(PlayerData player);

        public event PlayerEvent PlayerConnected = delegate { };
        public event PlayerEvent PlayerMoved = delegate { };
        public event PlayerEvent PlayerDisconnected = delegate { };
        #endregion

        #region Constructors
        public ViewModel()
        {
            server = new CCC_Server();
            Clients = new ObservableCollection<PlayerData>();
            Port = 63000;
            LocalAddress = IPUtils.GetLocalAddress();
            PublicAddress = IPUtils.GetPublicAddress();
            Running = false;
            LastSync = DateTime.MinValue;
            startCommand = new RelayCommand(OnStartExecuted, OnStartCanExecute);
            stopCommand = new RelayCommand(OnStopExecuted, OnStopCanExecute);
            
            server.PlayerConnected += OnClientConnect;
            server.PlayerDisconnected += OnClientDisconnect;
            server.PlayerMoved += OnPlayerMove;
            server.PlayerCrouch += OnPlayerCrouch;
            server.Sync += Server_Sync;
            
        }

        private void Server_Sync(DateTime datetime)
        {
            LastSync = datetime;
        }

        #endregion

        #region Eventhandlers
        private void OnClientDisconnect(CCC_Player player)
        {
            PlayerData p = ConvertPlayerData(player);
            
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].ID == p.ID)
                {
                    // This code is required because it is not possible to edit 
                    // an ObservableCollection outside the UI thread.
                    if (Application.Current != null)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
                        { Clients.RemoveAt(i); });
                    else
                        Clients.RemoveAt(i);

                    break;
                }
            }

            PlayerDisconnected(p);
        }

        private void OnClientConnect(CCC_Player player)
        {
            PlayerData p = ConvertPlayerData(player);

            // This code is required because it is not possible to edit 
            // an ObservableCollection outside the UI thread.
            if (Application.Current != null)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
                { Clients.Add(p); });
            else
                Clients.Add(p);

            PlayerConnected(p);
        }

        private void OnPlayerMove(CCC_Player player)
        {
            PlayerData p = ConvertPlayerData(player);
            
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].ID == p.ID)
                {
                    // This code is required because it is not possible to edit 
                    // an ObservableCollection outside the UI thread.
                    if (Application.Current != null)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
                        { Clients[i] = p; });
                    else
                        Clients[i] = p;

                    break;
                }
            }
            PlayerMoved(p);
        }
        private void OnPlayerCrouch(CCC_Player player)
        {
            PlayerData p = ConvertPlayerData(player);

            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].ID == p.ID)
                {
                    // This code is required because it is not possible to edit 
                    // an ObservableCollection outside the UI thread.
                    if (Application.Current != null)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
                        { Clients[i] = p; });
                    else
                        Clients[i] = p;

                    break;
                }
            }
        }
        #endregion

        #region Methodes

        private PlayerData ConvertPlayerData(CCC_Player player)
        {
            PlayerData p = new PlayerData();
            p.ID = player.ID;
            p.TeamID = player.TeamID;
            p.Username = player.Username;
            p.Health = player.Health;
            p.Armour = player.Armour;
            p.Crouching = player.Crouching;

            PlayerData.Vector3 pos = new PlayerData.Vector3();
            pos.X = player.Position.X;
            pos.Y = player.Position.Y;
            pos.Z = player.Position.Z;
            p.Position = pos;

            PlayerData.Vector3 rot = new PlayerData.Vector3();
            rot.X = player.Rotation.X;
            rot.Y = player.Rotation.Y;
            rot.Z = player.Rotation.Z;
            p.Rotation = rot;

            PlayerData.Vector3 scl = new PlayerData.Vector3();
            scl.X = player.Scale.X;
            scl.Y = player.Scale.Y;
            scl.Z = player.Scale.Z;
            p.Scale = scl;

            PlayerData.Vector3 vel = new PlayerData.Vector3();
            vel.X = player.Velocity.X;
            vel.Y = player.Velocity.Y;
            vel.Z = player.Velocity.Z;
            p.Velocity = vel;

            return p;
        }

        #endregion

        #region Execute
        public void OnStopExecuted(object obj)
        {
            try
            {
                server.Stop();
                Clients.Clear();
                Running = false;
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception");
            }
        }

        public void OnStartExecuted(object obj)
        {
            try
            {
                server.Start();
                Running = true;
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception");
            }
        }
        #endregion

        #region CanExecute
        public bool OnStopCanExecute(object obj)
        {
            return Running;
        }

        public bool OnStartCanExecute(object obj)
        {
            return !Running;
        }
        #endregion
    }
}
