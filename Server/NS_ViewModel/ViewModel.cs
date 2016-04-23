using Server.NS_Model;
using Server.NS_Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WhiteNet;
using WhiteNet.Server;


namespace Server.NS_ViewModel
{
    public class ViewModel : ViewModelBase
    {
        #region Attributes
        private RelayCommand startCommand;
        private RelayCommand stopCommand;

        private ObservableCollection<CCC_Player> clients;

        private IPAddress localAddress;
        private IPAddress publicAddress;
        private int port;

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

        public ObservableCollection<CCC_Player> Clients
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

        #endregion

        #region Constructors
        public ViewModel()
        {
            server = new CCC_Server();
            Clients = new ObservableCollection<CCC_Player>();
            Port = 0;
            LocalAddress = IPUtils.GetLocalAddress(); ;
            PublicAddress = IPUtils.GetPublicAddress();
            startCommand = new RelayCommand(OnStartExecuted, OnStartCanExecute);
            stopCommand = new RelayCommand(OnStopExecuted, OnStopCanExecute);

            server.PlayerConnected += OnClientConnect;
        }

        private void OnClientConnect(CCC_Player player)
        {
            clients.Add(player);
        }

        #endregion

        #region Methodes



        #endregion

        #region Execute
        public void OnStopExecuted(object obj)
        {
            try
            {
                server.Stop();
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
                Port = 63001;
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
            return true;
        }

        public bool OnStartCanExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
