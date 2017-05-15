using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager{

	public GameObject player;

	//for debug testing
	public override void OnServerConnect (NetworkConnection conn)
	{
		base.OnServerConnect (conn);
		Debug.Log(conn.address.ToString());
		Debug.Log(conn.connectionId.ToString());

	}


}
