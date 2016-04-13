using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using WhiteNet;

public class ServerTest : MonoBehaviour {

    public InputField inputIP;
    public Text labelServername;

    private CCC_Client client;
    // Use this for initialization
    void Start()
    {
        client = new CCC_Client();
        inputIP.text = IPUtils.GetLocalAddress().ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetInfo()
    {
        labelServername.text = client.GetInfo(IPAddress.Parse(inputIP.text));
    }
}
