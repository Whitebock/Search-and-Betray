using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WhiteNet.Server
{
    public class ServerClient
    {
        #region Attributes
        private TcpClient tcp;
        private IPEndPoint ip;

        private BinaryWriter writer;
        #endregion

        #region Properties
        public TcpClient Tcp
        {
            get { return tcp; }
        }
        public IPEndPoint IP
        {
            get { return ip; }
        }
        #endregion

        #region Events

        public delegate void Message(ServerClient sender, string message);

        private event Message recieveEvent = delegate { };

        public Message RecieveEvent
        {
            get { return recieveEvent; }
            set { recieveEvent = value; }
        }

        #endregion

        #region Constructors
        public ServerClient(TcpClient tcp)
        {
            this.tcp = tcp;
            ip = (IPEndPoint)tcp.Client.LocalEndPoint;
            writer = new BinaryWriter(tcp.GetStream());
            tcp.GetStream().BeginRead(new byte[] { 0 }, 0, 0, Read, tcp);
        }
        #endregion

        #region Network Methodes
        private void Read(IAsyncResult result)
        {
            try
            {

                if (tcp.Connected)
                    tcp.GetStream().BeginRead(new byte[] { 0 }, 0, 0, Read, null);
            }
            catch (Exception e)
            {
                RecieveEvent(this, e.Message);
            }
        }

        public void Send(string s)
        {
            //writer.WriteLine(s);
            //writer.Flush();
        }
        #endregion

        public override string ToString()
        {
            return String.Format("ServerClient[{0}:{1}]", ip.Address, ip.Port);
        }
    }
}
