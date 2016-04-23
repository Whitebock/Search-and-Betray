using UnityEngine;
using System.Collections;

public class ServerTest_Movement : MonoBehaviour {

    private CCC_Client client;

	void Start () {
        client = CCC_Client.CreateInstance();
        client.OnPlayerJoin += Client_OnPlayerJoin;
        client.OnPlayerUpdate += Client_OnPlayerUpdate;
        InvokeRepeating("UpdateTransform", 0, 0.2f);
	}

    private void Client_OnPlayerUpdate(int playerid, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        // Update the player object
    }

    private void Client_OnPlayerJoin(int playerid, string playername)
    {
        // Can't create player-gameobject in another thread
    }

    void Update () {
        
	}

    void UpdateTransform()
    {
        client.SendTransform(transform);
    }
}
