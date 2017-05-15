using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class networkcommands : NetworkBehaviour
{

	public GameObject Grenade;

	
	[Client]
	public void Fire ()
	{
		//syncro ammo
		CmdLowerAmmo (GetComponent<playerstats> ().networkid);

		RaycastHit hit;

		Ray to = Camera.main.ScreenPointToRay (Input.mousePosition);

		Debug.Log ("shots fired");

		if (Physics.Raycast (to, out hit)) {

			//send netid if object has a playerstats component 
			if (hit.transform.GetComponent<playerstats> () != null) {
				
				CmdFireMessage (hit.transform.GetComponent<playerstats> ().networkid);

			}
		}
	}

	[ClientRpc]
	public void RpcCreateBullethole(Vector3 position,Sprite bulletholeimage)
	{
		
		
	}


	[Command]
	public void CmdLowerAmmo (string playernetworkid)
	{

		//search playerlist from gamemager with same id then deal damage or something
		Debug.Log ("Ammo " + playernetworkid);

		//get player from list and do something
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);
		temp.ammocount--;

	}


	[Command]
	public void CmdAddAmmoClip (string playernetworkid, int magcount)
	{
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);
		temp.mags += magcount;
		
	}

	[Command]
	public void CmdAddGrenade (string playernetworkid)
	{
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);
		temp.grenades++;

	}


	[Command]
	public void CmdEquipWeapon (string playernetworkid, GameObject weapon)
	{
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);
		temp.hasweapon = true;
		GameObject x = Instantiate (weapon, Vector3.zero, Quaternion.identity) as GameObject;

	}

	[Command]
	public void CmdThrowWeapon (string playernetworkid, GameObject weapon)
	{
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);
		temp.hasweapon = false;

	}


	[Command]
	public void CmdFireMessage (string playernetworkid)
	{

		//search playerlist from gamemager with same id then deal damage or something
		Debug.Log ("Shot " + playernetworkid);

		//get player from list and do something
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);

		//dealdamge 20 to playername
		temp.RpcDealDamage (20, temp.networkplayername);

	}

	[Command]
	public void CmdReload (string playernetworkid)
	{
		//get player from list and do something
		playerstats temp = GameManager.GetPlayerstats (playernetworkid);

		if (temp.mags > 0) {
			temp.ammocount = 30;
			temp.mags--;
		}

		Debug.Log ("reloaded!");
	}

	[Command]
	public void CmdThrowGrenade (string playernetworkid)
	{
		

		Debug.Log ("Grenade Thrown!");
	}


	[ClientRpc]
	public void RpcGrenade (string[] playernetworkids)
	{
		foreach (var item in playernetworkids) {
			playerstats tempplayer = GameManager.GetPlayerstats (item);

			tempplayer.currentHealth -= 20;
		}

		
	}


}
