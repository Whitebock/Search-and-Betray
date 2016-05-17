using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using WhiteNet.Server;

namespace Server.NS_Model
{
    class CCC_Server
    {
        #region Attributes
        // Server 
        private WhiteNet.Server.Server server;
        private int port;
        private Timer synctimer;

        // Game
        private List<CCC_Player> players;
        private bool gameRunning;

        // Serverconfig
        public string Name { get; private set; }
        public int MaxPlayers { get; private set; }

        #endregion

        #region Delegates

        public delegate void PlayerEvent(CCC_Player player);
        public delegate void DateTimeEvent(DateTime datetime);

        #endregion

        #region Events

        public event PlayerEvent PlayerConnected = delegate { };
        public event PlayerEvent PlayerMoved = delegate { };
        public event PlayerEvent PlayerCrouch = delegate { };
        public event PlayerEvent PlayerDisconnected = delegate { };
        public event DateTimeEvent Sync = delegate { };

        #endregion

        #region Properties
        public List<CCC_Player> Players
        {
            get { return players; }
            set { players = value; }
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
            players = new List<CCC_Player>();

            synctimer = new Timer(10000);
            synctimer.Elapsed += SendSync;
            synctimer.Enabled = true;
            synctimer.AutoReset = true;
            
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Start listining for connections and, if set,
        /// start the sync timer.
        /// </summary>
        public void Start()
        {
            server.StartListener(port);
            synctimer.Start();
        }

        /// <summary>
        /// Stop Listener and Sync Timer.
        /// </summary>
        public void Stop()
        {
            server.StopListener();
            synctimer.Stop();
        }

        private void SendSync(object sender = null, ElapsedEventArgs e = null)
        {
            // Get sync table.
            CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.SYNC);
            List<byte> table = new List<byte>();
            for (int i = 0; i < players.Count; i++)
            {
                byte[] serialize = players[i].Serialize();
                UInt16 length = (UInt16)serialize.Length;

                table.AddRange(BitConverter.GetBytes(length));
                table.AddRange(serialize);
            }
            packet.Data = table.ToArray();

            // Send to all players.
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Client.Send(packet);
            }

            Sync(DateTime.Now);
        }
        #endregion

        
        private void OnClient(ServerClient client)
        {
            CCC_Packet response = client.Read();
            
            if (response.Flag == CCC_Packet.Type.HANDSHAKE)
            {
                /// Will check if client and server are using
                /// the same protocol version.
                if (response.Data.Length < 1 || response.Data[0] != CCC_Packet.Version)
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
                }
                else
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE_OK));
                }
            }
            else if (response.Flag == CCC_Packet.Type.INFO)
            {
                /// Will return information about the server.
                /// Clients can request this to prevent unnecessary requests.
                /// 

                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0};{1};{2};", Name, GameRunning, MaxPlayers);

                foreach (CCC_Player player in players)
                {
                    builder.AppendFormat("{0},", player.Username);
                }

                byte[] encodedInfo = Encoding.Unicode.GetBytes(builder.ToString());
                client.Send(new CCC_Packet(CCC_Packet.Type.INFO_RESPONSE, encodedInfo));
            }
            else if (response.Flag == CCC_Packet.Type.LOGIN)
            {
                /// Will check for:
                ///  - Game is full
                ///  - Username is taken by another player
                ///  - Username is valid
                ///    (Filter out inappropriate keywords/symbols/...)
                ///  - Whitelist
                ///  - Blacklist
                /// 
                /// If successfull, will add user to the player list.
                /// 

                #region Check if game is full.
                if (players.Count == MaxPlayers)
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.GAME_FULL));
                    return;
                }
                #endregion

                string username = Encoding.Unicode.GetString(response.Data);

                #region Check if username is taken.
                foreach (CCC_Player p in players)
                {
                    if (p.Username == username)
                    {
                        client.Send(new CCC_Packet(CCC_Packet.Type.USERNAME_TAKEN));
                        return;
                    }
                }
                #endregion

                #region Check if username is valid.
                // TODO
                // Maybe some file or smth
                if (String.IsNullOrWhiteSpace(username) || username.Contains("hacker"))
                {
                    client.Send(new CCC_Packet(CCC_Packet.Type.USERNAME_INVALID));
                    return;
                }
                #endregion

                #region Check Whitelist.

                #endregion

                #region Check Blacklist.

                #endregion

                #region Send Login Response
                CCC_Player player = new CCC_Player(client, players.Count + 1, username);

                List<byte> loginresponse = new List<byte>();
                loginresponse.Add(player.ID);
                loginresponse.AddRange(Encoding.Unicode.GetBytes(player.Username));
                client.Send(new CCC_Packet(CCC_Packet.Type.LOGIN_OK, loginresponse.ToArray()));
                #endregion

                #region Notify other Players
                CCC_Packet joinPacket = new CCC_Packet(CCC_Packet.Type.PLAYER_JOIN);
                List<byte> temp = new List<byte>();
                temp.Add((byte)player.ID);
                temp.AddRange(Encoding.Unicode.GetBytes(player.Username));
                joinPacket.Data = temp.ToArray();

                // Notify other players.
                foreach (CCC_Player p in players)
                {
                    p.Client.Send(joinPacket);
                }
                #endregion

                // Add player.
                players.Add(player);
                PlayerConnected(player);
                
                // Add all eventhandlers
                player.Logout += Player_Logout;
                player.TransformChanged += Player_TransformChanged;
                player.Crouch += Player_Crouch;
                player.Timeout += Player_Timeout;
                player.Shoot += Player_Shoot;

                // Sync
                SendSync();
            }
            else
            {
                /// Will return PROTOCOL_NOT_SUPPORTED error.
                /// 

                client.Send(new CCC_Packet(CCC_Packet.Type.PROTOCOL_NOT_SUPPORTED, CCC_Packet.Version));
            }
        }

        #region Player Eventhandlers
        private void Player_Timeout(CCC_Player player)
        {
            Debug.WriteLine("TIMEOUT");
            CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.TIMEOUT);
            packet.Data = new byte[1];
            packet.Data[0] = player.ID;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player)
                {
                    players.RemoveAt(i);
                }
                else
                {
                    players[i].Client.Send(packet);
                }
            }
            PlayerDisconnected(player);
        }

        private void Player_Crouch(CCC_Player player)
        {
            SendPlayerUpdate(player);

            PlayerCrouch(player);
        }

        private void Player_TransformChanged(CCC_Player player)
        {
                SendPlayerUpdate(player);

             PlayerMoved(player);
        }

        private void Player_Logout(CCC_Player player)
        {
            CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.LOGOUT);
            packet.Data = new byte[1];
            packet.Data[0] = player.ID;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player)
                {
                    players.RemoveAt(i);
                }
                else
                {
                    players[i].Client.Send(packet);
                }
            }
            PlayerDisconnected(player);
        }

        private void Player_Shoot(CCC_Player player, CCC_Player.Vector3 position, int? playerid = null, int? amount = null)
        {
            List<byte> shootpacket = new List<byte>();
            shootpacket.AddRange(BitConverter.GetBytes(playerid.HasValue));
            byte[] pos = position;
            shootpacket.AddRange(pos);

            // Check if a player was hit.
            if (playerid.HasValue)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    // Find player that was hit.
                    if (players[i].ID == playerid)
                    {
                        // Get damage amount.
                        int damage = 0;
                        if (amount.HasValue)
                            damage = amount.Value;

                        // Set health and send update
                        players[i].TakeDamage((byte)damage);
                        shootpacket.Add((byte)damage);
                        shootpacket.Add(players[i].ID);
                        SendPlayerUpdate(players[i]);
                    }
                }
            }

            // Send shoot packet
            CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_SHOOT);
            packet.Data = shootpacket.ToArray();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != player)
                    try { players[i].Client.Send(packet); }
                    catch (Exception) { players.RemoveAt(i); }
            }
        }

        private void SendPlayerUpdate(CCC_Player player)
        {
            CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_UPDATE);
            packet.Data = player.Serialize();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player)
                    players[i] = player;
                else
                    try { players[i].Client.Send(packet); }
                    catch (Exception) { players.RemoveAt(i); }
            }
        }
        #endregion
    }
}
