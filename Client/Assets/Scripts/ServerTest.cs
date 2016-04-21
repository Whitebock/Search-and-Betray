using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using WhiteNet;
using System;

public class ServerTest : MonoBehaviour {

    public InputField inputIP;
    public InputField inputPort;
    public InputField inputUsername;
    public Text labelServername;
    public Text labelVersion;
    public Text labelPlayerCount;
    public Text labelGameRunning;

    private CCC_Client client;
    // Use this for initialization
    void Start()
    {
        client = new CCC_Client();
        inputIP.text = IPUtils.GetLocalAddress().ToString();
        inputPort.text = client.Port.ToString();
        inputUsername.text = "Testuser";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetInfo()
    {
        client.Port = Int32.Parse(inputPort.text);
        IPAddress address = IPAddress.Parse(inputIP.text);

        labelVersion.text = "v" + client.GetProtocol(address);

        string[] info = client.GetInfo(address);

        labelServername.text = info[0];
        labelGameRunning.text = info[1];
        labelPlayerCount.text = (info[3].Split(',').Length - 1) + "/" +  info[2];
    }

    public void Connect()
    {
        client.Port = Int32.Parse(inputPort.text);
        IPAddress address = IPAddress.Parse(inputIP.text);

        client.Connect(address, inputUsername.text);
    }
}
