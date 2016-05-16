using UnityEngine;
using System.Collections;

//Borucki


public class HUDTest : MonoBehaviour {

	[Header("Main HUD Test")]
	public bool HUDTestEnabled = false;

	[Header("Test connection status display mode")]
	public bool testConnectionDisplayMain = false;
	public ConnectionStatus connectionStatus;

	[Header("Test weapon display")]
	public bool testWeaponDisplayMain = false;
	public string weaponname;
	public FireMode fireMode;
	public int currentAmmo = 0;
	public int maxAmmo = 0;

	[Header("Test health display")]
	public bool testHealthDisplayMain = false;
	public string playerName;

	[Range(0, 100)]
	public int currentHealth = 100;
	[Range(0, 100)]
	public int currentAmour = 100;
	public Color playerColor;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (HUDTestEnabled) 
		{
			if (testConnectionDisplayMain) 
			{
				HUDManagment.SetConnectionStatus(connectionStatus);
			}
			if (testWeaponDisplayMain) 
			{
				HUDManagment.SetWeaponInfo(weaponname,currentAmmo,maxAmmo);
				HUDManagment.SetFireMode(fireMode);
			}
			if (testHealthDisplayMain) 
			{
				HUDManagment.SetPlayerInfo(playerName);
				HUDManagment.SetPlayerHealth(currentHealth);
				HUDManagment.SetPlayerArmor(currentAmour);
				HUDManagment.SetPlayerColor(playerColor);
			}

		}
	}
}