using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System;
using WhiteNet;
using WhiteNet.Client;
using System.Text;
using System.Collections.Generic;

public class CCC_Client
{
    #region Attributes
    private int port = 63001;
    private Client client;

    private static CCC_Client singleton;
    #endregion

    #region Properties
    public int Port
    {
        get
        {
            return port;
        }

        set
        {
            port = value;
        }
    }

    #endregion

    #region Delegates

    public delegate void JoinEvent(int playerid, string playername);
    public delegate void UpdateEvent(int playerid, Vector3 position, Quaternion rotation, Vector3 scale);

    #endregion

    #region Events

    public event JoinEvent OnPlayerJoin = delegate { };
    public event UpdateEvent OnPlayerUpdate = delegate { };

    #endregion

    #region Constructors / Singleton
    public static CCC_Client CreateInstance()
    {
        if (singleton == null)
        {
            singleton = new CCC_Client();
        }
        return singleton;
    }
    private CCC_Client()
    {
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
            string username = packet.GetString(1);

            OnPlayerJoin(playerid, username);
        }
        else if(packet.Flag == CCC_Packet.Type.PLAYER_UPDATE)
        {
            int playerid = packet.Data[0];

            Vector3 position = new Vector3();
            position.x = BitConverter.ToSingle(packet.Data, 1);
            position.y = BitConverter.ToSingle(packet.Data, 5);
            position.z = BitConverter.ToSingle(packet.Data, 9);

            Vector3 rotatation = new Vector3();
            rotatation.x = BitConverter.ToSingle(packet.Data, 13);
            rotatation.y = BitConverter.ToSingle(packet.Data, 17);
            rotatation.z = BitConverter.ToSingle(packet.Data, 21);

            Vector3 scale = new Vector3();
            scale.x = BitConverter.ToSingle(packet.Data, 25);
            scale.y = BitConverter.ToSingle(packet.Data, 29);
            scale.z = BitConverter.ToSingle(packet.Data, 33);

            OnPlayerUpdate(playerid, position, Quaternion.Euler(rotatation), scale);
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

        string temp = response.GetString();
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
        

        client.BeginRead();
    }

    public void Disconnect()
    {
        client.Send(new CCC_Packet(CCC_Packet.Type.LOGOUT));
        client.EndRead();
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
        CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_CROUTCH);
        packet.Data = BitConverter.GetBytes(crouching);

        client.Send(packet);
    }

    public void SendDamage(int playerid, int amount)
    {
        if (playerid > 255)
            throw new ArgumentException("PlayerID is too large", "playerid");

        if (amount > 255)
            amount = byte.MaxValue;

        CCC_Packet packet = new CCC_Packet(CCC_Packet.Type.PLAYER_SEND_DAMAGE);
        packet.Data = new byte[2];
        packet.Data[0] = (byte)playerid;
        packet.Data[1] = (byte)amount;

        client.Send(packet);
    }

    #endregion
}
