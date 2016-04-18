using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class CCC_Packet
{
    /// <summary>
    /// Version needs to be updated if there were changes
    /// to the protocol, to inform the client/server
    /// </summary>
    public static byte Version
    {
        get { return 2; }
    }

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

    |   <-      | LOGIN_OK                  |
    |           | USERNAME_TAKEN            |
    |           | USERNAME_INVALID          |
    |           | GAME_FULL                 |
    |           | BLACKLIST                 | time
    |           | WHITELIST                 |

    Game -> Player
    |   <-      | PLAYER_TABLE              | array with all player objects (position, orientation, etc.)
    |   <-      | PLAYER_UPDATE             | position; rotation

    |   ->      | PLAYER_MOVE               | position; rotation
    |   ->      | PLAYER_JUMP               |
    |   ->      | PLAYER_CROUTCH            | bool
    |   ->      | PLAYER_DEAL_DAMAGE        | amount
    |   <-      | PLAYER_RECIEVE_DAMAGE     | amount

    Game -> Prop
    |   ->      | PROP_NEW                  | type; position
    |   <-      | PROP_CREATED              | id; type; position
    |   ->      | PROP_MOVE                 | id; position

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
        PLAYER_TABLE = 80,
        PLAYER_UPDATE,
        PLAYER_MOVE,
        PLAYER_JUMP,
        PLAYER_CROUTCH,
        PLAYER_DEAL_DAMAGE,
        PLAYER_RECIEVE_DAMAGE,

        //Game -> Prop
        PROP_NEW = 100,
        PROP_CREATED,
        PROP_MOVE,

        //Logout
        LOGOUT = 120
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

    #region Data Conversion 

    public string GetString()
    {
        return Encoding.UTF8.GetString(Data);
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
        Type type = (Type)packet[0];
        byte[] data = packet.Skip(1).ToArray();
        CCC_Packet d = new CCC_Packet(type, data);
        return d;
    }
    #endregion

    #region Overrides

    public override string ToString()
    {
        return String.Format("Packet[{0}][{1}]", Flag, Data);
    }

    #endregion
}

