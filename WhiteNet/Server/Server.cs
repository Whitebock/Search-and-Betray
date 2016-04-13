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
        private Thread thread;
        private List<ServerClient> clients;
        private IPEndPoint endPoint;
        
        #endregion

        #region Properties

        public List<ServerClient> Clients
        {
            get { return clients; }
        }

        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        public bool Listening
        {
            get { return thread != null; }
        }

        #endregion

        #region Events

        public delegate void ClientEvent(ServerClient client);

        private event ClientEvent clientConnected = delegate { };

        public ClientEvent ClientConnected
        {
            get { return clientConnected; }
            set { clientConnected = value; }
        }

        #endregion

        #region Constructor

        public Server()
        {
            clients = new List<ServerClient>();
            endPoint = new IPEndPoint(IPUtils.GetLocalAddress(), 0);
        }

        #endregion

        #region Network Methodes

        private void ListenerThread()
        {
            while (true)
            {
                try
                {
                    TcpClient tcp = listener.AcceptTcpClient();

                    //StreamReader reader = new StreamReader(tcp.GetStream());
                    //Read logon infos

                    ServerClient client = new ServerClient(tcp);

                    //Check if client is already connected

                    clients.Add(client);
                    //client.RecieveEvent += Recieved;
                    //client.DisconnectEvent += Disconnected;

                    ClientConnected(client);

                }
                catch (Exception) { }
            }
        }

        private void Recieved(ServerClient client, string message)
        {
            //Send to other clients
        }

        private void Disconnected(ServerClient client)
        {
            clients.Remove(client);
            client.Tcp.Close();

            //Notify the other Clients

        }
        #endregion

        #region Methodes

        /// <summary>
        /// Stopps Listener Thread
        /// </summary>
        public void StopListener()
        {
            foreach (ServerClient c in clients)
            {
                //Notify all other clients
                c.Tcp.Close();
            }
            listener = null;
            thread = null;
            clients.Clear();
        }

        /// <summary>
        /// Starts Listener Thread
        /// </summary>
        /// <param name="port">Port to listen</param>
        public void StartListener(int port)
        {
            endPoint.Port = port;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                thread = new Thread(new ThreadStart(ListenerThread));
                thread.Name = "Listener Thread";
                thread.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}
