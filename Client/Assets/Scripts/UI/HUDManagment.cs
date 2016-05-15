using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HUDManagment : MonoBehaviour 
{
	[Header("Player Information")]
	public static Text playerInfo;

	[Header("Player Status")]
	public static Image playerColor;
	public static Slider playerHealth;
	public static Slider playerArmor;

	[Header("Weapon Status")]
	public static Text weaponInfo;
	public static Text currentAmmo;
	public static Text maxAmmo;
    public static Image crosshair;

	[Header("Weapon Mode")]
	public static Text fireMode;

    [Header("Connection Status")]
    public static Text connectionStatus;


    /// <summary>
    /// Start this instance. Searches gameobjects for static attributes.
    /// </summary>
    void Start()
	{
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
		weaponInfo.text = currentWeaponinfo;
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

    public static void SetConnectionStatus(string status)
    {
        connectionStatus.text = status;
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
