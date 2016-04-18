using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System;
using WhiteNet;
using WhiteNet.Client;

public class CCC_Client
{

    private int port = 63001;
    private Client client;

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

    public CCC_Client()
    {
        client = new Client();
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

    }

    public void Disconnect()
    {

    }
    #endregion

    #region Send Methodes
    public void SendTransform(Transform transform)
    {

    }

    public void SendJump()
    {

    }

    public void SendCrouch(bool crouching)
    {

    }

    public void SendDamage(GameObject player)
    {

    }

    #endregion
}
