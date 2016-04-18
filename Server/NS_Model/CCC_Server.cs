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
        #region Attributes
        // Server 
        private WhiteNet.Server.Server server;
        private int port;

        // Game
        private List<CCC_Player> clients;
        private bool gameRunning;

        // Serverconfig
        public string Name { get; private set; }
        public int MaxPlayers { get; private set; }

        #endregion

        #region Properties
        public List<CCC_Player> Clients
        {
            get { return clients; }
            set { clients = value; }
        }
        
        public bool GameRunning
        {
            get { return gameRunning; }
        }
        #endregion

        #region Constructors
        public CCC_Server()
        {
            port = 63001;
            server = new WhiteNet.Server.Server();
            server.ClientConnected += OnClient;

            // TODO: Load config
            Name = "Test Server";
            MaxPlayers = 8;

            // Temp for testing
            gameRunning = false;
            clients = new List<CCC_Player>();
        }
        #endregion

        #region Methodes
        public void Start()
        {
            server.StartListener(port);
        }

        public void Stop()
        {
            server.StopListener();
        }
        #endregion

        #region Eventhandlers
        private void OnClient(ServerClient client)
        {
            Debug.WriteLine("--Client");
            CCC_Packet response = client.Read();
            if (response.Flag == CCC_Packet.Type.HANDSHAKE)
            {
                Debug.WriteLine("HANDSHAKE");
                if (response.Data.Length < 1 || response.Data[0] != CCC_Packet.Version)
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
                }
                else
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE_OK));
                }
            }
            else if(response.Flag == CCC_Packet.Type.INFO)
            {
                // THIS IS ONLY TEMP
                // Will be replaced by proper encoding later
                Debug.WriteLine("INFO");
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0};{1};{2};", Name, GameRunning, MaxPlayers);

                foreach (CCC_Player player in Clients)
                {
                    builder.AppendFormat("{0},", player.Username);
                }

                byte[] encodedInfo = Encoding.UTF8.GetBytes(builder.ToString());
                client.Send(new CCC_Packet(CCC_Packet.Type.INFO_RESPONSE, encodedInfo));
            }
            else
            {
                Debug.WriteLine("UNKNOWN");
                // Unknown Packet Flag.
                client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
            }
        }
        #endregion
    }
}
