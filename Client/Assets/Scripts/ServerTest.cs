using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using WhiteNet;
using System;

public class ServerTest : MonoBehaviour {

    public InputField inputIP;
    public InputField inputPort;
    public Text labelServername;

    private CCC_Client client;
    // Use this for initialization
    void Start()
    {
        client = new CCC_Client();
        inputIP.text = IPUtils.GetLocalAddress().ToString();
        inputPort.text = client.Port.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetInfo()
    {
        client.Port = Int32.Parse(inputPort.text);
        labelServername.text = client.GetInfo(IPAddress.Parse(inputIP.text));
    }
}
