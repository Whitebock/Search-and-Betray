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
        private Thread listenerThread;

        private bool listening;
        #endregion

        #region Properties
        public bool Listening
        {
            get { return listening; }
            set { listening = value; }
        }

        #endregion

        #region Events

        public delegate void TcpEvent(TcpClient tcp);

        private event TcpEvent tcpConnected = delegate { };

        public TcpEvent TcpConnected
        {
            get { return tcpConnected; }
            set { tcpConnected = value; }
        }

        #endregion

        #region Constructors

        public Server()
        {

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

            listener = null;
            listenerThread = null;
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

                listenerThread = new Thread(new ThreadStart(ListenerThread));
                listenerThread.Name = "Listener Thread";
                listenerThread.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Thread Methodes
        private void ListenerThread()
        {
            while (true)
            {
                try
                {
                    TcpClient tcp = listener.AcceptTcpClient();

                    TcpConnected(tcp);

                }
                catch (Exception) { }
            }
        }
        #endregion
    }
}
