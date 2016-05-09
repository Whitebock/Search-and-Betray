using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NS_Model
{
    public class CCC_Packet
    {
        /// <summary>
        /// Version needs to be updated if there were changes
        /// to the protocol, to inform the client/server
        /// </summary>
        public static byte Version { get { return 6; } }

        /*
        ----------------------------------------------------------------------------------------------------------------
        | Direction |        Flag               | Data
        ----------------------------------------------------------------------------------------------------------------

        Handshake
        |   ->      | HANDSHAKE                 | version

        |   <-      | HANDSHAKE_OK              |
        |           | PROTOCOL_NOT_SUPPORTED    | version

        Info
        |   ->      | INFO                      |

        |   <-      | INFO_RESPONSE             | servername; game running; max players; array with player names

        Login
        |   ->      | LOGIN                     | username

        |   <-      | LOGIN_OK                  | id, username
        |           | USERNAME_TAKEN            |
        |           | USERNAME_INVALID          |
        |           | GAME_FULL                 |
        |           | BLACKLIST                 | time
        |           | WHITELIST                 |

        Game -> Player
        |   <-      | PLAYER_JOIN               | id, username
        |   <-      | SYNC_TABLE                | array with all objects
        |   <-      | PLAYER_UPDATE             | position, rotation, scale
        |   ->      | PLAYER_MOVE               | position, rotation, scale
        |   ->      | PLAYER_CROUTCH            | bool
        |   ->      | PLAYER_SEND_DAMAGE        | id, amount
        |   <-      | PLAYER_RECIEVE_DAMAGE     | amount

        Game -> Prop
        |   ->      | PROP_NEW                  | type; position
        |   <-      | PROP_CREATED              | id; type; position
        |   ->      | PROP_MOVE                 | id; position

        Chat
        |   ->      | SEND_MESSAGE              | text
        |   <-      | RECIEVE_MESSAGE           | id, text

        Logout
        |   ->      | LOGOUT                    |

        ----------------------------------------------------------------------------------------------------------------
        */

        public enum Type : byte
        {
            //Handshake
            HANDSHAKE = 20,
            HANDSHAKE_OK,
            PROTOCOL_NOT_SUPPORTED,

            //Info
            INFO = 40,
            INFO_RESPONSE,

            //Login
            LOGIN = 60,
            LOGIN_OK,
            USERNAME_TAKEN,
            USERNAME_INVALID,
            GAME_FULL,
            BLACKLIST,
            WHITELIST,

            //Game -> Player
            PLAYER_JOIN = 80,
            SYNC_TABLE,
            PLAYER_UPDATE,
            PLAYER_MOVE,
            PLAYER_CROUTCH,
            PLAYER_SEND_DAMAGE,
            PLAYER_RECIEVE_DAMAGE,

            //Game -> Prop
            PROP_NEW = 100,
            PROP_CREATED,
            PROP_MOVE,

            //Chat
            SEND_MESSAGE = 120,
            RECIEVE_MESSAGE,

            //Logout
            LOGOUT = 140
        }

        #region Properties
        public Type Flag { get; set; }
        public byte[] Data { get; set; }

        #endregion

        #region Constructors
        public CCC_Packet(Type flag, byte[] data)
        {
            Flag = flag;
            Data = data;
        }

        public CCC_Packet(Type flag, byte data)
        {
            Flag = flag;
            byte[] temp = new byte[1];
            temp[0] = data;
            Data = temp;
        }

        public CCC_Packet(Type flag)
        {
            Flag = flag;
            Data = new byte[0];
        }
        #endregion

        #region Basic Data Conversion 

        public string GetString(int offset = 0)
        {
            return Encoding.Unicode.GetString(Data.Skip(offset).ToArray());
        }

        #endregion

        #region Operators
        public static implicit operator byte[] (CCC_Packet packet)
        {
            List<byte> temp = new List<byte>();
            temp.Add((byte)packet.Flag);
            temp.AddRange(packet.Data);
            return temp.ToArray();
        }

        public static implicit operator CCC_Packet(byte[] packet)
        {
            if (packet.Length == 0)
            {
                return new CCC_Packet(Type.LOGOUT);
            }
            Type type = (Type)packet[0];
            byte[] data = packet.Skip(1).ToArray();
            CCC_Packet d = new CCC_Packet(type, data);
            return d;
        }
        #endregion

        #region ToString

        public override string ToString()
        {
            return String.Format("Packet[{0}][{1}]", Flag, Data);
        }

        public string ToBitString(bool markbytes = false)
        {
            BitArray bits = new BitArray(this);
            string output = "";
            for (int i = 1; i < bits.Length; i++)
            {
                output += bits[i] ? '1' : '0';
                if (markbytes && i % 8 == 0)
                {
                    output += '|';
                }
            }

            if (markbytes)
                output += " (" + bits.Length + "bit)";

            return output;
        }

        #endregion
    }
}
