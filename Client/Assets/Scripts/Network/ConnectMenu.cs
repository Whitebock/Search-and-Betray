using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using WhiteNet;
using System;
using UnityEngine.SceneManagement;

public class ConnectMenu : MonoBehaviour {

    public InputField inputIP;
    public InputField inputPort;
    public InputField inputUsername;

    public Text labelServername;
    public Text labelPlayerCount;
    public Text labelVersion;

    public Text error;

    public Canvas connectMenu;
    public Canvas loginMenu;

    private CCC_Client client;

    void Start()
    {
        client = CCC_Client.CreateInstance();
        inputIP.text = IPUtils.GetLocalAddress().ToString();
        inputPort.text = client.Port.ToString();
        inputUsername.text = "";
        labelVersion.text = "v" + CCC_Packet.Version.ToString();
    }

    public void GetInfo()
    {
        client.Port = Int32.Parse(inputPort.text);
        IPAddress address = IPAddress.Parse(inputIP.text);
        string[] info;
        try
        {
            info = client.GetInfo(address);
        }
        catch (Exception e)
        {
            error.text = e.Message;
            return;
        }

        // Set Info.
        labelServername.text = info[0];


        labelPlayerCount.text = (info[3].Split(',').Length - 1) + "/" +  info[2] + " Players";

        // Switch Menus.
        connectMenu.gameObject.SetActive(false);
        loginMenu.gameObject.SetActive(true);
    }

    public void Connect()
    {
        error.text = "";
        client.Port = Int32.Parse(inputPort.text);
        IPAddress address = IPAddress.Parse(inputIP.text);

        try
        {
            client.Connect(address, inputUsername.text);
        }
        catch (Exception e)
        {
            error.text = e.Message;
            return;
        }

        SceneManager.LoadScene("MapTest");
    }
}
