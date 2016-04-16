using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<ServerClient> clients;

        public List<ServerClient> Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        private WhiteNet.Server.Server server;

        public CCC_Server()
        {
            port = 63001;
            server = new WhiteNet.Server.Server();
            server.ClientConnected += OnClient;
        }

        public void Start()
        {
            server.StartListener(port);
        }

        public void Stop()
        {
            server.StopListener();
        }

        private void OnClient(ServerClient client)
        {
            CCC_Packet response = client.Read();
            if (response.Flag == CCC_Packet.Type.HANDSHAKE)
            {
                if (response.Data.Length < 1 || response.Data[0] != CCC_Packet.Version)
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
                }
                else
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE_OK));
                }
            }
            else
            {
                //Unknown Packet Flag
                client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
            }
        }
    }
}
