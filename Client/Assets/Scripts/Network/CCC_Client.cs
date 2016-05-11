using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System;
using WhiteNet;
using WhiteNet.Client;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class CCC_Client
{
    #region Attributes
    private int port = 63001;
    private Client client;

    private static CCC_Client singleton;
    #endregion

    private struct DeserializedPlayer
    {
        public int ID, TeamID, Health, Armour;
        public bool Crouching;
        public Vector3 Position, Velocity, Scale;
        public Quaternion Rotation;
        public string Username;
        public DeserializedPlayer(byte[] serializedData)
        {
            ID = serializedData[0];
            TeamID = serializedData[1];
            Health = serializedData[2];
            Armour = serializedData[3];
            Crouching = BitConverter.ToBoolean(serializedData, 4);

            Vector3 pos = new Vector3();
            pos.x = BitConverter.ToSingle(serializedData, 5);
            pos.y = BitConverter.ToSingle(serializedData, 9);
            pos.z = BitConverter.ToSingle(serializedData, 13);
            Position = pos;
            
            Vector3 rot = new Vector3();
            rot.x = BitConverter.ToSingle(serializedData, 17);
            rot.y = BitConverter.ToSingle(serializedData, 21);
            rot.z = BitConverter.ToSingle(serializedData, 25);
            Rotation = Quaternion.Euler(rot);

            Vector3 vel = new Vector3();
            vel.x = BitConverter.ToSingle(serializedData, 29);
            vel.y = BitConverter.ToSingle(serializedData, 33);
            vel.z = BitConverter.ToSingle(serializedData, 37);
            Velocity = vel;

            Vector3 scl = new Vector3();
            scl.x = BitConverter.ToSingle(serializedData, 41);
            scl.y = BitConverter.ToSingle(serializedData, 45);
            scl.z = BitConverter.ToSingle(serializedData, 49);
            Scale = scl;

            byte[] ubytes = serializedData.Skip(53).ToArray();
            Username = Encoding.Unicode.GetString(ubytes);
        }
    }
    #region Properties
    public int Port
    {
        get { return port; }
        set { port = value; }
    }

    public static CCC_Client Instance
    {
        get
        {
            if (singleton == null)
                singleton = new CCC_Client();
            return singleton;
        }
    }
    #endregion

    #region Delegates

    public delegate void JoinEvent(int playerid, string playername);
    public delegate void SyncEvent(Dictionary<int, string> players);
    public delegate void UpdateEvent(int playerid, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 scale, bool crouching, int health);
    public delegate void ShootEvent(Vector3 position);

    #endregion

    #region Events

    public event JoinEvent OnPlayerJoin = delegate { };
    public event UpdateEvent OnPlayerUpdate = delegate { };
    public event SyncEvent OnSync = delegate { };
    public event ShootEvent OnPlayerShoot = delegate { };

    #endregion

    #region Constructors
    private CCC_Client()
    {
        Console.SetOut(new UnityTextWriter());
        Application.runInBackground = true;
        client = new Client();
        client.DataReceived += OnDataReceived;
    }

    #endregion

    private void OnDataReceived(byte[] data)
    {
        CCC_Packet packet = data;

        if (packet.Flag == CCC_Packet.Type.PLAYER_JOIN)
        {
            int playerid = packet.Data[0];
            string username = Encoding.Unicode.GetString(packet.Data.Skip(1).ToArray());

            OnPlayerJoin(playerid, username);
        }
        else if (packet.Flag == CCC_Packet.Type.PLAYER_UPDATE)
        {
            DeserializedPlayer d = new DeserializedPlayer(packet.Data);

            OnPlayerUpdate(d.ID, d.Position, d.Rotation, d.Velocity, d.Scale, d.Crouching, d.Health);
        }
        else if (packet.Flag == CCC_Packet.Type.PLAYER_SHOOT)
        {
            bool hit = BitConverter.ToBoolean(packet.Data, 0);
            Vector3 position = new Vector3();
            position.x = BitConverter.ToSingle(packet.Data, 1);
            position.y = BitConverter.ToSingle(packet.Data, 5);
            position.z = BitConverter.ToSingle(packet.Data, 9);
            Debug.Log("SHOT_RECIEVED");
            OnPlayerShoot(position);
        }
        else if (packet.Flag == CCC_Packet.Type.SYNC)
        {
            Dictionary<int, string> syncdata = new Dictionary<int, string>();
            int i = 0;
            try
            {
                do
                {
                    UInt16 length = BitConverter.ToUInt16(packet.Data, i);
                    i += 2;
                    byte[] serialized = packet.Data.Skip(i).Take(length).ToArray();
                    DeserializedPlayer d = new DeserializedPlayer(serialized);

                    syncdata.Add(d.ID, d.Username);
                    OnPlayerUpdate(d.ID, d.Position, d.Rotation, d.Velocity, d.Scale, d.Crouching, d.Health);

                    i += serialized.Length;

                } while (i < packet.Data.Length - 1);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            OnSync(syncdata);
        }
        else
        {
            Debug.Log(packet.Flag);
        }
    }

    #region Network Methodes
    public string[] GetInfo(IPAddress address)
    {
        if (GetProtocol(address) != CCC_Packet.Version)
        {
            throw new Exception("Protocol not supported");
        }

        // Connect to the server.
        client.Connect(address, Port);

        // Send Info Request.
        client.Send(new CCC_Packet(CCC_Packet.Type.INFO));

        // Wait for Response.
        CCC_Packet response = client.Read();

        // Disconnect.
        client.Disconnect();

        string temp = Encoding.Unicode.GetString(response.Data);
        return temp.Split(';');
    }

    public int GetProtocol(IPAddress address)
    {
        // Connect.
        client.Connect(address, Port);


        // Send Handshake.
        client.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE, CCC_Packet.Version));

        // Wait for response.
        CCC_Packet response = client.Read();

        // Disconnect.
        client.Disconnect();

        if (response.Flag != CCC_Packet.Type.HANDSHAKE_OK)
            return response.Data[0];
        return CCC_Packet.Version;


    }

    public void Connect(IPAddress address, string username)
    {
        if (GetProtocol(address) != CCC_Packet.Version)
        {
            throw new Exception("Protocol not supported");
        }

        // Connect to the server.
        client.Connect(address, Port);

        // Send Info Request.
        byte[] data = Encoding.Unicode.GetBytes(username);
        client.Send(new CCC_Packet(CCC_Packet.Type.LOGIN, data));

        // Wait for Response.
        CCC_Packet loginresponse = client.Read();

        switch (loginresponse.Flag)
        {
            case CCC_Packet.Type.USERNAME_TAKEN:
                client.Disconnect();
                throw new Exception("Username taken");

            case CCC_Packet.Type.USERNAME_INVALID:
                client.Disconnect();
                throw new Exception("Username invalid");

            case CCC_Packet.Type.GAME_FULL:
                client.Disconnect();
                throw new Exception("Game full");

            case CCC_Packet.Type.BLACKLIST:
                client.Disconnect();
                throw new Exception("You are banned from this server");

            case CCC_Packet.Type.WHITELIST:
                client.Disconnect();
                throw new Exception("You are not whitelisted on this server");
        }

        PlayerInfo.PlayerID = loginresponse.Data[0];
        PlayerInfo.PlayerName = Encoding.Unicode.GetString(loginresponse.Data.Skip(1).ToArray());
        client.BeginRead();
    }

    public void Disconnect()
    {
        client.EndRead();
        client.Send(new CCC_Packet(CCC_Packet.Type.LOGOUT));
        client.Disconnect();
    }
    #endregion

    #region Send Methodes
    public void SendTransform(Transform transform, Vector3 velocity)
    {
        CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_MOVE);

        List<byte> data = new List<byte>();
        data.AddRange(BitConverter.GetBytes(transform.position.x));
        data.AddRange(BitConverter.GetBytes(transform.position.y));
        data.AddRange(BitConverter.GetBytes(transform.position.z));

        data.AddRange(BitConverter.GetBytes(transform.rotation.eulerAngles.x));
        data.AddRange(BitConverter.GetBytes(transform.rotation.eulerAngles.y));
        data.AddRange(BitConverter.GetBytes(transform.rotation.eulerAngles.z));

        data.AddRange(BitConverter.GetBytes(velocity.x));
        data.AddRange(BitConverter.GetBytes(velocity.y));
        data.AddRange(BitConverter.GetBytes(velocity.z));

        data.AddRange(BitConverter.GetBytes(transform.localScale.x));
        data.AddRange(BitConverter.GetBytes(transform.localScale.y));
        data.AddRange(BitConverter.GetBytes(transform.localScale.z));

        packet.Data = data.ToArray();

        client.Send(packet);
    }

    public void SendCrouch(bool crouching)
    {
        CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_CROUCH);
        packet.Data = BitConverter.GetBytes(crouching);

        client.Send(packet);
    }

    public void SendShot(Vector3 position, int? playerid = null, int? amount = null)
    {
        if (playerid > 255)
            throw new ArgumentException("PlayerID is too large", "playerid");

        if (amount > 255)
            amount = byte.MaxValue;

        CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_SHOOT);
        List<byte> temp = new List<byte>();

        temp.Add(BitConverter.GetBytes(playerid.HasValue)[0]);
        temp.AddRange(BitConverter.GetBytes(position.x));
        temp.AddRange(BitConverter.GetBytes(position.y));
        temp.AddRange(BitConverter.GetBytes(position.z));

        if (playerid.HasValue)
        {
            temp.Add((byte)playerid);
            temp.Add((byte)amount);
        }
        packet.Data = temp.ToArray();

        client.Send(packet);
    }

    #endregion
}
