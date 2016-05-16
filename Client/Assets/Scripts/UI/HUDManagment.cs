using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

//Borucki

public class HUDManagment : MonoBehaviour 
{
	[Header("Player Information")]
	private static Text playerInfo;

	[Header("Player Status")]
	private static Image playerColor;
	private static Slider playerHealth;
	private static Slider playerArmor;

	[Header("Weapon Status")]
	private static Text weaponInfo;
	private static Text currentAmmo;
	private static Text maxAmmo;
	private static Image crosshair;

	[Header("Weapon Mode")]
	private static Text fireMode;

    [Header("Connection Status")]
	private static GameObject connectionPanel;
	private static Text connectionStatus;
	private static Image connectionImage;

	//Sprites
	private static Sprite connectionLost;
	private static Sprite connectionTimeOut;
	private static Sprite connectionNotConnected;


    /// <summary>
    /// Start this instance. Searches gameobjects for static attributes.
    /// </summary>
    void Awake()
	{
		//Bind static avaible objects
		playerInfo = GameObject.Find("Text_PlayerInfo").GetComponent<Text>();
		playerColor = GameObject.Find("User_Color").GetComponent<Image>();
		playerHealth = GameObject.Find("Slider_Health").GetComponent<Slider>();
		playerArmor = GameObject.Find("Slider_Armor").GetComponent<Slider>();
		weaponInfo = GameObject.Find("Text_WeaponInfo").GetComponent<Text>();
		currentAmmo = GameObject.Find("Text_CurrentAmmo").GetComponent<Text>();
		maxAmmo = GameObject.Find("Text_MaxAmmo").GetComponent<Text>();
		fireMode = GameObject.Find("Text_FireMode").GetComponent<Text>();
        connectionStatus = GameObject.Find("Text_ConnectionStatus").GetComponent<Text>();
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();

		//Search Resources
		connectionLost = Resources.Load<Sprite>("IconLost");
		connectionTimeOut = Resources.Load<Sprite>("IconTime");
		connectionNotConnected = Resources.Load<Sprite>("IconNo");

		//Setup 
		SetConnectionStatus(ConnectionStatus.Estabalished);
    }

	/// <summary>
	/// Sets the player info [Name or position. E.G. "Playername" or "Traitor".
	/// </summary>
	/// <param name="currentPlayerinfo">The player info you want to display next to the health and armor panel.</param>
	public static void SetPlayerInfo(string currentPlayerinfo)
	{
		playerInfo.text = currentPlayerinfo;
	}

	/// <summary>
	/// Sets the color of the player decoration on HUD. This is optional. E.G. for Traitor a red sign.
	/// </summary>
	/// <param name="color">The color that should be set.</param>
	public static void SetPlayerColor(Color color)
	{
		playerColor.color = color;
	}

	/// <summary>
	/// Sets on HUD the player health.
	/// </summary>
	/// <param name="health">The health that should be set.</param>
	public static void SetPlayerHealth(int health)
	{
		playerHealth.value = health;
	}

	/// <summary>
	/// Gets the current player health on HUD.
	/// </summary>
	/// <returns>The player health that is currently set.</returns>
	public static int GetPlayerHealth()
	{
		return int.Parse(playerHealth.value.ToString());
	}

	/// <summary>
	/// Sets the player armor on HUD.
	/// </summary>
	/// <param name="armor">The armor that should be set.</param>
	public static void SetPlayerArmor(int armor)
	{
		playerArmor.value = armor;
	}

	/// <summary>
	/// Gets the current player armor on HUD.
	/// </summary>
	/// <returns>The player armor that is currently set</returns>
	public static int GetPlayerArmor()
	{
		return int.Parse(playerArmor.value.ToString());
	}

	/// <summary>
	/// Sets the weapon info.
	/// </summary>
	/// <param name="currentWeaponinfo">Current weaponinfo. The name of the weapon on HUD.</param>
	/// <param name="weaponCurrentAmmo">Weapon current ammo. Current Ammo of the player.</param>
	/// <param name="weaponMaxAmmo">Weapon max ammo. Maximal ammo that fits into weapon.</param>
	public static void SetWeaponInfo(string currentWeaponinfo, int weaponCurrentAmmo, int weaponMaxAmmo)
	{
		weaponInfo.text = currentWeaponinfo.ToUpper();
		currentAmmo.text = weaponCurrentAmmo.ToString();
		maxAmmo.text = weaponMaxAmmo.ToString();
	}

	/// <summary>
	/// Sets the fire mode that should be shown on HUD.
	/// </summary>
	/// <param name="firemode">Firemode as selectable from enum</param>
	public static void SetFireMode(FireMode firemode)
	{
		fireMode.text = Convert.ToString(firemode.ToString()[0].ToString());
	}

	/// <summary>
	/// Set the visibility of the HUD. Setup is "true"
	/// </summary>
	/// <param name="on">If set to <c>true</c> shows HUD.</param>
	public static void SetDisplayHUD(bool on)
	{
		if (on) 
		{
			GameObject.Find("HUD").GetComponent<CanvasGroup>().alpha = 1.0F;
		}
		else
		{
			GameObject.Find("HUD").GetComponent<CanvasGroup>().alpha = 0.0F;
		}
	}

    /// <summary>
    /// Method to turn crosshair on and off
    /// </summary>
    /// <param name="on">Indicates new status of crosshair</param>
    public static void SetCrosshair(bool on)
    {
        crosshair.enabled = on;
    }

	/// <summary>
	/// Sets the connection status on HUD.
	/// </summary>
	/// <param name="mainstatus">Select a status from ConnectionStatus-enum. Estabalished will deactivate the hud message.</param>
	/// <param name="statustext">Already provided by enum, but you can overwrite the message if neccessary</param>
	public static void SetConnectionStatus(ConnectionStatus mainstatus, string statustext = null)
    {
		//Bind additional objects
		connectionStatus = GameObject.Find("Text_ConnectionStatus").GetComponent<Text>();
		connectionImage = GameObject.Find("Image_ConnectionState").GetComponent<Image>();
		connectionPanel = GameObject.Find("Panel_ConnectionState");


		if (mainstatus != ConnectionStatus.Estabalished) 
		{
			switch (mainstatus) 
			{
			case ConnectionStatus.Lost:
				connectionImage.color = new Color(251,68,0,255);
				connectionImage.sprite = connectionLost;
				if (statustext != null) 
				{
					connectionStatus.text = statustext.ToUpper();
				} 
				else 
				{
					connectionStatus.text = "CONNECTION LOST";
				}
				connectionPanel.GetComponent<CanvasGroup>().alpha = 1.0F;
				break;
			case ConnectionStatus.TimeOut:
				connectionImage.sprite = connectionTimeOut;
				connectionImage.color = new Color(0,213,255,255);
				if (statustext != null) 
				{
					connectionStatus.text = statustext.ToUpper();
				} 
				else 
				{
					connectionStatus.text = "TIME OUT";
				}
				connectionPanel.GetComponent<CanvasGroup>().alpha = 1.0F;
				break;
			case ConnectionStatus.NotConnected:
				connectionImage.sprite = connectionNotConnected;
				connectionImage.color = new Color(255,0,0,255);
				if (statustext != null) 
				{
					connectionStatus.text = statustext.ToUpper();
				} 
				else 
				{
					connectionStatus.text = "NOT CONNECTED";
				}
				connectionPanel.GetComponent<CanvasGroup>().alpha = 1.0F;
				break;
			default:
				connectionPanel.GetComponent<CanvasGroup>().alpha = 0.0F;
				connectionStatus.text = string.Empty;
				connectionImage = null;
				break;
			}
		}
		else
		{
			connectionPanel.GetComponent<CanvasGroup>().alpha = 0.0F;
			connectionStatus.text = string.Empty;
			connectionImage = null;
		}
    }
}

/// <summary>
/// Fire mode.
/// </summary>
public enum FireMode
{
	Automatic = 'A',
	Single = 'S'
}

public enum ConnectionStatus
{
	Lost,
	TimeOut,
	NotConnected,
	Estabalished
}
