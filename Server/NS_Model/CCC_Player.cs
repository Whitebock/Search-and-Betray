using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteNet.Server;

namespace Server.NS_Model
{
    public class CCC_Player
    {
        #region Attibutes

        public struct Vector3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public static implicit operator byte[] (Vector3 vector)
            {
                List<byte> temp = new List<byte>();
                temp.AddRange(BitConverter.GetBytes(vector.X));
                temp.AddRange(BitConverter.GetBytes(vector.Y));
                temp.AddRange(BitConverter.GetBytes(vector.Z));
                return temp.ToArray();
            }

            public static implicit operator Vector3(byte[] packet)
            {
                Vector3 vector = new Vector3();
                vector.X = BitConverter.ToSingle(packet, 0);
                vector.Y = BitConverter.ToSingle(packet, 4);
                vector.Z = BitConverter.ToSingle(packet, 8);
                return vector;
            }
        }

        //Player
        public byte ID { get; private set; }
        public byte Health { get; private set; }
        public byte Armour { get; private set; }
        public byte TeamID { get; private set; }
        public string Username { get; private set; }
        public bool Crouching { get; private set; }

        // Gameobject 
        public Vector3 Scale { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }

        //Network
        public ServerClient Client { get; private set; }

        #endregion

        #region Delegates

        public delegate void PlayerEvent(CCC_Player player);

        #endregion

        #region Events
        /// <summary>
        /// Will be fired when the player moves, rotates, 
        /// changes velocity or changes scale
        /// </summary>
        public event PlayerEvent TransformChanged = delegate { };

        /// <summary>
        /// Will be fired when the player starts crouching,
        /// or stops crouching.
        /// </summary>
        public event PlayerEvent Crouch = delegate { };

        /// <summary>
        /// Will be fired when the player disconnects
        /// </summary>
        public event PlayerEvent Logout = delegate { };

        /// <summary>
        /// Will be fired on timeout
        /// </summary>
        public event PlayerEvent Timeout = delegate { };

        #endregion

        #region Constructors
        public CCC_Player(ServerClient client, int id, string username)
        {
            Client = client;
            ID = (byte)id;
            Username = username;
            Crouching = false;
            Health = 100;
            Armour = 50;
            
            Client.DataReceived += OnDataReceived;
            Client.Timeout += OnTimeout;
            Client.BeginRead();
        }

        #endregion

        private void OnDataReceived(byte[] data)
        {
            CCC_Packet response = data;
            if (response.Flag == CCC_Packet.Type.PLAYER_MOVE)
            {

                byte[] posbytes = response.Data.Skip(0).Take(12).ToArray();
                byte[] rotbytes = response.Data.Skip(12).Take(12).ToArray();
                byte[] velbytes = response.Data.Skip(24).Take(12).ToArray();
                byte[] sclbytes = response.Data.Skip(36).Take(12).ToArray();

                Position = posbytes;
                Rotation = rotbytes;
                Velocity = velbytes;
                Scale = sclbytes;

                TransformChanged(this);
            }
            else if (response.Flag == CCC_Packet.Type.PLAYER_CROUCH)
            {
                Crouching = BitConverter.ToBoolean(response.Data, 0);
                Crouch(this);
            }
            else if (response.Flag == CCC_Packet.Type.LOGOUT)
            {
                Logout(this);
            }
        }

        private void OnTimeout(byte[] data)
        {
            Timeout(this);
        }

        /// <summary>
        /// Serialize Player object, including all stats.
        /// (Health, Position, etc.)
        /// </summary>
        /// <returns>Serialized bytes</returns>
        public byte[] Serialize()
        {
            List<byte> temp = new List<byte>();

            // Player stats.
            temp.Add(ID);
            temp.Add(TeamID);
            temp.Add(Health);
            temp.Add(Armour);

            // A bit overkill to use an entire byte,
            // might use the empty space for something else
            // in the future.
            temp.Add(BitConverter.GetBytes(Crouching)[0]);

            // Player transform.
            byte[] pos = Position;
            byte[] rot = Rotation;
            byte[] vel = Velocity;
            byte[] scl = Scale;

            temp.AddRange(pos);
            temp.AddRange(rot);
            temp.AddRange(vel);
            temp.AddRange(scl);

            temp.AddRange(Encoding.Unicode.GetBytes(Username));

            return temp.ToArray();
        }

        #region Operators
        public static bool operator ==(CCC_Player p1, CCC_Player p2)
        {
            return p1.ID == p2.ID;
        }
        public static bool operator !=(CCC_Player p1, CCC_Player p2)
        {
            return p1.ID != p2.ID;
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            CCC_Player p = (CCC_Player)obj;
            if (p != null && p.ID == ID)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public override string ToString()
        {
            return String.Format("{0}[({1}, {2}, {3}), ({4}, {5}, {6}), ({7}, {8}, {9})]",
                Username, 
                Position.X, Position.Y, Position.Z,
                Rotation.X, Rotation.Y, Rotation.Z,
                Scale.X, Scale.Y, Scale.Z);
        }
        #endregion

    }
}
