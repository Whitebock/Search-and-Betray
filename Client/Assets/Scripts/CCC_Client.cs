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
    public CCC_Client()
    {
        client = new Client();
    }

    #region Network Methodes
    public string GetInfo(IPAddress address)
    {
        //Connect
        client.Connect(address, port);

        //Send Handshake
        client.Send(new CCC_Packet(CCC_Packet.Type.HANDSHAKE));

        //Wait for response
        CCC_Packet response = client.Read();

        if (response.Flag != CCC_Packet.Type.HANDSHAKE_OK)
        {
            throw new Exception("Protocol not supported");
        }

        //Send Info Request
        client.Send(new CCC_Packet(CCC_Packet.Type.INFO));

        //Wait for Response
        response = client.Read();

        return response.GetString();
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
