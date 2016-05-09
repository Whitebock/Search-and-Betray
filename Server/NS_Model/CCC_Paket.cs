using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.NS_Model
{
    public class CCC_Packet
    {
        /// <summary>
        /// Version needs to be updated if there were changes
        /// to the protocol, to inform the client/server
        /// </summary>
        public static byte Version { get { return 7; } }

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
        |           | PLAYER_UPDATE             | player object

        |   ->      | PLAYER_MOVE               | position, rotation, velocity, scale
        |           | PLAYER_CROUCH             | bool
        |           | PLAYER_SHOOT              | [hitplayer] position [amount, playerid]
        |   <-      | PLAYER_SHOOT              | [hitplayer] position [amount, playerid]
        
        |   <-      | SYNC                      | all objects

        Game -> Prop
        |   ->      | PROP_NEW                  | type; position
        |           | PROP_MOVE                 | id; position, rotation

        |   <-      | PROP_CREATED              | id; type; position
        |           | PROP_UPDATE               | prop object
        |           | PROP_DELETE               | id

        Chat
        |   ->      | MESSAGE                   | text
        |   <-      | MESSAGE                   | id, text

        Logout
        |   ->      | LOGOUT                    |

        |   <-      | LOGOUT                    | id
        |           | TIMEOUT                   | id
        |           | KICK                      | id
        |           | BAN                       | id

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

            // Player
            PLAYER_JOIN = 80,
            PLAYER_UPDATE,
            PLAYER_MOVE,
            PLAYER_CROUCH,
            PLAYER_SHOOT,

            SYNC = 90,

            // Prop
            PROP_NEW = 100,
            PROP_CREATED,
            PROP_MOVE,
            PROP_DELETE,
            PROP_UPDATE,

            //Chat
            MESSAGE = 120,

            //Logout
            LOGOUT = 140,
            TIMEOUT,
            KICK,
            BAN
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
