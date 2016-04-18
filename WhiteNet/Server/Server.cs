using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WhiteNet.Server
{
    public class Server
    {
        #region Attributes
        private TcpListener listener;
        private List<ServerClient> clients;

        private bool listening;
        #endregion

        #region Delegates

        public delegate void ClientEvent(ServerClient client);

        #endregion

        #region Events

        public event ClientEvent ClientConnected = delegate { };

        #endregion

        #region Properties
        public List<ServerClient> Clients
        {
            get { return clients; }
        }

        public bool Listening
        {
            get { return listening; }
        }
        #endregion

        #region Constructors

        public Server()
        {
            listening = false;
            clients = new List<ServerClient>();
        }

        #endregion
        
        #region Listener Methodes

        /// <summary>
        /// Stopps Listener Thread
        /// </summary>
        public void StopListener()
        {
            if (!listening)
                throw new Exception("Listener not started");

            listener.Stop();
            listening = false;
        }

        /// <summary>
        /// Starts Listener Thread
        /// </summary>
        /// <param name="port">Port to listen</param>
        public void StartListener(int port)
        {
            if (listening)
                throw new Exception("Already listening for connections");

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), listener);

                listening = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start Listener", e);
            }
        }

        #endregion

        #region Thread Methodes

        private void OnConnect(IAsyncResult result)
        {
            try
            {
                TcpClient tcp = listener.EndAcceptTcpClient(result);
                
                //Add client
                ServerClient client = new ServerClient(tcp);
                ClientConnected(client);
                clients.Add(client);

                //Start accepting clients again
                listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), listener);
            }
            catch (ObjectDisposedException)
            {
                //The Server was stopped, so we can just return
                return;
            }


        }
        #endregion
    }
}
