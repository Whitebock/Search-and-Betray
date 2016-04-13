using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WhiteNet.Server;

namespace Server.NS_Model
{
    class CCC_Server
    {
        private int port;

        private List<TcpClient> clients;

        public List<TcpClient> Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        private WhiteNet.Server.Server server;

        public CCC_Server()
        {
            port = 63001;
            server = new WhiteNet.Server.Server();
            //server.ClientConnected += OnClient;
        }

        public void Start()
        {
            server.StartListener(port);
            //server.BeginRead();
        }

        public void Stop()
        {
            server.StopListener();
            /*
            if (server.Reading)
            {
                server.EndRead();

            }
            */
        }

        private void OnClient(TcpClient client)
        {
            CCC_Packet response = new CCC_Packet(CCC_Packet.Type.BLACKLIST); //server.Read();
            if (response.Flag == CCC_Packet.Type.HANDSHAKE)
            {
                if (response.Data[0] != CCC_Packet.Version)
                {
                    byte[] version = new byte[1];
                    version[0] = CCC_Packet.Version;
                    //server.Send(client, new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, version));
                    client.Close();
                }
                else
                {
                    //server.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE_OK));
                    client.Close();
                }
            }
        }
    }
}
