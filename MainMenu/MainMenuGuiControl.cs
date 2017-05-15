using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;

public class MainMenuGuiControl : MonoBehaviour
{
	public Text localIp;
	public Text externalIp;
	public InputField serverIp;
	public InputField serverPort;

	//getting external IP
	public string GetExternalIp ()
	{
		string trimedIp;

		//send a request
		WebRequest request = WebRequest.Create ("http://checkip.dyndns.org");

		//get response from request
		WebResponse response = request.GetResponse ();

		//get response as stream for editing
		StreamReader reader = new StreamReader (response.GetResponseStream ());

		trimedIp = reader.ReadToEnd ();

		//regex pattern trimmer
		string Ip = Regex.Replace (trimedIp, "[^0-9.]", "");

		return Ip;
	}

	public string GetLocalIp ()
	{
		string ips = "";

		IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName ());

		foreach (var item in host.AddressList) {

			if (item.AddressFamily == AddressFamily.InterNetwork) {

				ips = item.ToString ();

			} else {
				Debug.Log ("no Lan Enabled!");
			}


		}

		return ips;

	}

	void Start()
	{
		//get ip and set to gui text
		localIp.text += GetLocalIp();

		 // externalIp.text += GetExternalIp();

		Debug.Log(CustomNetworkManager.singleton.networkAddress);
		
	}

	// Use this for initialization
	public void StartNetworkGame ()
	{
		CustomNetworkManager.singleton.StartHost();
			
	}
	
	// Update is called once per frame
	public void JoinNetworkGame ()
	{
		//get ip from boxes
		CustomNetworkManager.singleton.networkAddress = serverIp.text;
		CustomNetworkManager.singleton.networkPort = Convert.ToInt32(serverPort.text); 
		CustomNetworkManager.singleton.StartClient();
	}
}
