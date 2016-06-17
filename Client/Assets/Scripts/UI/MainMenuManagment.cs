using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

//Borucki


public class MainMenuManagment : MonoBehaviour 
{
	/// ! Attributes 

	// MainMenu
	[Tooltip("List with all MenuTabs (Links buttons and their panels)")]
	[Header("Menu Tab List")]
	public MenuTab[] allMenuTabs;
	[Header("Information Area")]
	public Text ui_InfoDeviceInfo;
	public Text ui_InfoPlayerName;
	public Text ui_InfoClockHour;
	public Text ui_InfoClockSeperator;
	public Text ui_InfoClockMinute;
	[Header("Audio Sources")]
	public AudioSource ui_mainFX;
	public AudioSource ui_mainMusic;
	[Header("Sounds")]
	public AudioClip ui_failed;
	public AudioClip ui_success;
	public AudioClip ui_change;
	public AudioClip ui_wait;

	[Header("Menu Tabs")]
	[Header("Connection Tab")]
	// ConnectionTab
	public InputField ui_connectionIpAdress;
	public InputField ui_connectionPort;
	public Text ui_connectionTabInfo;

	[Header("Player Tab")]
	// PlayerTab
	public InputField ui_PlayerName;
	public Text ui_PlayerTabInfo;

	[Header("Preferences Tab")]
	//PreferencesTab
	public Dropdown ui_PreferencesResolution; 
	public Dropdown ui_PreferencesQuality;
	public Slider ui_PreferencesVolume;
	public Toggle ui_PreferencesFullscreen;

	private bool onChange = false;

	// Use this for initialization
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		SetDeviceInfo();
		SetPlayerInfo();
		Connect_SetValidationsOnInput();
		Player_SetValidationsOnInput();
		Player_PrepareUI();
		Preferences_PrepareUI();
		onChange = true;

        ui_connectionIpAdress.text = GetLocalAddress().ToString();
        ui_connectionPort.text = 63001 + "";
	}

    public static IPAddress GetLocalAddress(bool ipv6 = false)
    {
        IPAddress localhost = null;
        foreach (IPAddress ipaddress in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (ipaddress.AddressFamily == (ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork))
            {
                localhost = ipaddress;
                break;
            }
        }
        return localhost;
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update ()
	{
		SetClockInfo();
	}

	//Contains methods who are just reliable for menu-functions
	#region MainMenu

	/// <summary>
	/// Sets the device info: Like processor, memory and graphics.
	/// Puts them on the ui.
	/// </summary>
	private void SetDeviceInfo()
	{
		ui_InfoDeviceInfo.text = string.Format("{0}\n",SystemInfo.processorType);
		ui_InfoDeviceInfo.text += string.Format("{0} MB - RAM\n", SystemInfo.systemMemorySize);
		ui_InfoDeviceInfo.text += string.Format("{0}",SystemInfo.graphicsDeviceName);
	}

	/// <summary>
	/// Reads out the last PlayerName out of the PlayerPrefs.
	/// If no PlayerName was found, it will set the default ("Unkown")
	/// </summary>
	private void SetPlayerInfo()
	{
		if (PlayerPrefs.HasKey("PlayerName"))
		{
			ui_InfoPlayerName.text = PlayerPrefs.GetString("PlayerName");
		}
		else
		{
			PlayerPrefs.SetString("PlayerName","Unknown");
			ui_InfoPlayerName.text = PlayerPrefs.GetString("PlayerName");
		}
	}

	/// <summary>
	/// Sets the clock to actual time.
	/// </summary>
	private void SetClockInfo()
	{
		string seperator = "";

		if (DateTime.Now.Second %2 == 1) 
		{
			seperator = "\u202f:";
		}
				
		ui_InfoClockHour.text = DateTime.Now.Hour.ToString().PadLeft(2,'0');
		ui_InfoClockSeperator.text = seperator;
		ui_InfoClockMinute.text = DateTime.Now.Minute.ToString().PadLeft(2,'0');
	}

	/// <summary>
	/// Plaies the success Sound.
	/// </summary>
	private void PlaySuccessFX()
	{
		if (ui_success) 
		{
			ui_mainFX.clip = ui_success;
			ui_mainFX.Play();
		}
	}

	/// <summary>
	/// Plaies the failed Sound.
	/// </summary>
	private void PlayFailedFX()
	{
		if (ui_failed) 
		{
			ui_mainFX.clip = ui_failed;
			ui_mainFX.Play();
		}
	}

	/// <summary>
	/// Plaies the change Sound.
	/// </summary>
	private void PlayChangeFX()
	{
		if (ui_change) 
		{
			ui_mainFX.clip = ui_change;
			ui_mainFX.Play();
		}
	}

	/// <summary>
	/// Plaies overhanded AudioClip as FX-Sound.
	/// </summary>
	/// <param name="clip">AudioClip.</param>
	private void PlayClipFX(AudioClip clip)
	{
		if (clip) 
		{
			ui_mainFX.clip = clip;
			ui_mainFX.Play();
		}
	}

	#endregion

	//Contains methods who are reliable for an specific MenuTab
	#region Tabs

	#region ConnectTab
	/// <summary>
	/// Gets and prepares all Informations to connect to server.
	/// </summary>
	public void Connect_JoinGame()
	{
		ConnectionInformation serverToConnect = new ConnectionInformation();

		IPAddress ip = ValidateIPAdress(ui_connectionIpAdress.text);

		if (ip != null) 
		{
			int port;
			if (int.TryParse(ui_connectionPort.text,out port)) 
			{
				if (ValidatePort(port) == "OKAY") 
				{
					//[Obsolete]serverToConnect.ip = ip.ToString().Split(new char[] {'.'}, System.StringSplitOptions.RemoveEmptyEntries);
					serverToConnect.ip = ip;
					serverToConnect.port = int.Parse(ui_connectionPort.text);
					serverToConnect.name = (PlayerPrefs.GetString("PlayerName") == null) ? serverToConnect.name = "Unknown" : PlayerPrefs.GetString("PlayerName");

					ui_connectionTabInfo.text = "Attempting to connect";
					PlayClipFX(ui_wait);


					// Overhands to executing Procedures
					UIRespond connectionRespond = Execute_Connect_JoinGame(serverToConnect);

					//Displays messages if connection hasn't been estabelished
					ui_connectionTabInfo.text = connectionRespond.respondingTextForUI.ToUpper();
					if (connectionRespond.respondingSoundForUI) 
					{
						ui_mainFX.clip = connectionRespond.respondingSoundForUI;
						ui_mainFX.Play();
					}

				}
				else
				{
					ui_connectionTabInfo.text = ValidatePort(port);
					PlayFailedFX();
				}

			} 
			else 
			{
				ui_connectionTabInfo.text = "PORT INVALID";
				PlayFailedFX();
			}
		}
		else
		{
			ui_connectionTabInfo.text = "IP-ADRESS INVALID";
			PlayFailedFX();
		}


	}

	/// <summary>
	/// Sets the validations on inputfields.
	/// </summary>
	private void Connect_SetValidationsOnInput()
	{
		ui_connectionIpAdress.onValidateInput += delegate(string input, int charIndex, char addedChar) { return ValidateIPChar( addedChar ); };
		ui_connectionIpAdress.characterLimit = 15;
		ui_connectionPort.characterValidation = InputField.CharacterValidation.Integer;
		ui_connectionPort.characterLimit = 5;
	}

	#endregion

	#region PlayerTab

	private void Player_PrepareUI()
	{
		if (PlayerPrefs.GetString("PlayerName") != "")
		{
			ui_PlayerName.text = PlayerPrefs.GetString("PlayerName");
		}
		else
		{
			PlayerPrefs.SetString("PlayerName","Unknown");
			ui_PlayerName.text = PlayerPrefs.GetString("PlayerName");
		}
	}

	private void Player_SetValidationsOnInput()
	{
		ui_PlayerName.contentType = InputField.ContentType.Standard;
		ui_PlayerName.characterValidation = InputField.CharacterValidation.None;
		ui_PlayerName.characterLimit = 20;
	}

	public void Player_SaveInformation()
	{
		PlayerPrefs.SetString("PlayerName",ui_PlayerName.text);
		ui_InfoPlayerName.text = PlayerPrefs.GetString("PlayerName");
		ui_PlayerTabInfo.text = "SUCCESS";
		PlaySuccessFX();
	}

	#endregion

	#region Preferences

	private void Preferences_PrepareUI()
	{
		// Resolution
			// Declares and initates list for Dropdown
			System.Collections.Generic.List<Dropdown.OptionData> resolutionlist = new System.Collections.Generic.List<Dropdown.OptionData>();

			// Write accessable resolutions into list
			foreach (Resolution resolution in Screen.resolutions) 
			{
				resolutionlist.Add(new Dropdown.OptionData(string.Format("{0}x{1}",resolution.width,resolution.height)));
			}

			// Assigning list do Dropdown
			ui_PreferencesResolution.AddOptions(resolutionlist);

			// Selecting current resolution
			ui_PreferencesResolution.value = Array.FindIndex(Screen.resolutions,FindCurrentResolution);

		//Quality
			// Declares and initates list for Dropdown
			System.Collections.Generic.List<Dropdown.OptionData> qualitylist = new System.Collections.Generic.List<Dropdown.OptionData>();

			// Write accessable qualities into list
			foreach (string name in QualitySettings.names) 
			{
				qualitylist.Add(new Dropdown.OptionData(string.Format("{0}",name)));
			}

			// Assigning list do Dropdown
			ui_PreferencesQuality.AddOptions(qualitylist);

			// Selecting current quality level
			ui_PreferencesQuality.value = QualitySettings.GetQualityLevel();

		//Volume
			//Set Slider to current Volume
			ui_PreferencesVolume.value = PlayerPrefs.GetFloat("MainVolume");

		//Fullscreen
			//Sets Toggle to now condition
			ui_PreferencesFullscreen.isOn = Screen.fullScreen;


		}

	public void Preferences_SelectResolution(Int32 value)
	{
		if (onChange) 
		{
			Screen.SetResolution(Screen.resolutions[value].width,Screen.resolutions[value].width,Screen.fullScreen);
			PlayChangeFX();
			Debug.Log("Resolution :" + Screen.resolutions[value].width + "x" + Screen.resolutions[value].width);
		}
	}

	public void Preferences_SelectQuality(Int32 value)
	{
		if (onChange) 
		{
			QualitySettings.SetQualityLevel(value);
			PlayChangeFX();
			Debug.Log("Quality: " + QualitySettings.names[value]);
		}
	}

	public void Preferences_SelectVolume(Single value)
	{
		if (onChange) 
		{
			ChangeMenuVolume(ui_PreferencesVolume.value);
			PlayChangeFX();
		}
	}

	public void Preferences_ToggleFullscreen(bool full)
	{
		if (onChange) 
		{
			Screen.fullScreen = full;
			Debug.Log("Fullscreen: " + full);
			PlayChangeFX();
		}
	}

	public void Preferences_Save()
	{
		PlayerPrefs.SetInt("ResolutionWidth",Screen.resolutions[ui_PreferencesResolution.value].width);
		PlayerPrefs.SetInt("ResolutionHeight",Screen.resolutions[ui_PreferencesResolution.value].height);
		PlayerPrefs.SetInt("MainQuality",ui_PreferencesQuality.value);
		PlayerPrefs.SetFloat("MainVolume",ui_PreferencesVolume.value);
		PlayerPrefs.SetString("Fullscreen",ui_PreferencesFullscreen.isOn.ToString());
		PlayerPrefs.Save();
		PlaySuccessFX();
	}

	private static bool FindCurrentResolution(Resolution res)
	{
		if (res.height == Screen.currentResolution.height && res.width == Screen.currentResolution.width) 
		{
			return true;
		}
		else
		{
			return false;
		}
			

	}

	#endregion

	#region Universal

	[RuntimeInitializeOnLoadMethod]
	public void StartUp()
	{
		//Setting Volume
		if (PlayerPrefs.HasKey("MainVolume")) 
		{
			PlayerPrefs.SetFloat("MainVolume",1.0F);
			ChangeMenuVolume(1.0F);
		}
		else
		{
			ChangeMenuVolume(PlayerPrefs.GetFloat("MainVolume"));
		}
	}

	public void OpenTab()
	{
		CloseTabs();

		foreach (MenuTab tab in allMenuTabs) 
		{
			if (tab.tabButtonToActivate.name == EventSystem.current.currentSelectedGameObject.name) 
			{
				tab.panelToActivate.SetActive(true);
				if (tab.soundToPlay != null) 
				{
					ui_mainFX.clip = tab.soundToPlay;
					ui_mainFX.Play();
				}
			}
		}
	}

	public void CloseTabs()
	{
		foreach (GameObject tab in GameObject.FindGameObjectsWithTag("UITab")) 
		{
			tab.SetActive(false);
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ChangeMenuVolume(float volume)
	{
		ui_mainFX.volume = volume;
		ui_mainMusic.volume = volume;
	}

	#endregion

	#endregion

	//Contains help-methods who are just reliable for input validation methods
	#region Validation

	/// <summary>
	/// Validates the IP char.
	/// </summary>
	/// <returns>[char] The IP char if correct. Else returns empty.</returns>
	/// <param name="charToValidate">Char to validate.</param>
	private char ValidateIPChar(char charToValidate)
	{
		//Checks if a ip valid char is entered....
		if (Regex.IsMatch(charToValidate.ToString(),"[0-9]|\\."))
		{
			// ... returns if okay
			return charToValidate;
		}
		charToValidate = '\0';
		return charToValidate;


	}

	/// <summary>
	/// Validates the IP adress.
	/// </summary>
	/// <returns>[IPAdress] The IP adress if correct. Else returns null.</returns>
	/// <param name="iptovalidate">Iptovalidate.</param>
	private IPAddress ValidateIPAdress(string iptovalidate)
	{
		if (Regex.IsMatch(iptovalidate,"^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$")) 
		{
			return IPAddress.Parse(iptovalidate);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Validates the port.
	/// </summary>
	/// <returns>[String] The "OKAY" if correct. Otherwise an Error-Prompt</returns>
	/// <param name="port">Port.</param>
	private string ValidatePort(int port)
	{
		if (port < 0) 
		{
			return "PORT INVALID (NEGATIV)";
		}
		else if(port > 65535)
		{
			return "PORT INVALID (OUT OF RANGE)";
		}
		else
		{
			return "OKAY";
		}
	}
	#endregion

	//Have to be filled by the reliable teams
	#region Further actions [Methods who gets activated by overhanding functions]
	// ! ToDO ! // [] <- Your tasks
	// Methods for the transfer and execution of further actions [ Have to be completed by assigned team ]

	/// <summary>
	/// Execute the connection.
	/// </summary>
	/// <returns>[Please prepare UIRespond - e.x. FAILED BECAUSE...] - Status for UI.</returns>
	/// <param name="server">Server to connect.</param>
	UIRespond Execute_Connect_JoinGame(ConnectionInformation server)
	{
		//Here are your information:

		//server.ip; <- IP 
		//server.port; <- Port
		//server.name; <- Player Name

		CCC_Client client = CCC_Client.Instance;

		client.Port = server.port;

		try
		{
			client.Connect(server.ip, server.name);

		}
		catch (Exception e)
		{
			return new UIRespond(e.Message,ui_failed);
		}

		SceneManager.LoadScene("CandaharCity");
		return new UIRespond("CONNECTING");

	}

	#endregion
}

//Defines classes for menu-functions
#region Additional classes

/// <summary>
/// Menu tab.
/// </summary>
[System.Serializable]
public struct MenuTab
{
	[Tooltip("Choose button that should activate the panel.")]
	public Button tabButtonToActivate;
	[Tooltip("Choose panel that should get activated with clicking the button.")]
	public GameObject panelToActivate;
	[Tooltip("[Optional] That that should be played with activation.")]
	public AudioClip soundToPlay;

	/// <summary>
	/// Initializes a new instance of the <see cref="MenuTab"/> class.
	/// </summary>
	/// <param name="tabButtonToActivate">Tab button who activates this tab.</param>
	/// <param name="panelToActivate">Panel which should get activated.</param>
	/// <param name="soundToPlay">Sound to play by activation.</param>
	public MenuTab(Button tabButtonToActivate, GameObject panelToActivate, AudioClip soundToPlay = null)
	{
		this.tabButtonToActivate = tabButtonToActivate;
		this.panelToActivate = panelToActivate;
		this.soundToPlay = soundToPlay;
	}
}

/// <summary>
/// User interface respond.
/// </summary>
[System.Serializable]
public struct UIRespond
{
	public String respondingTextForUI;
	public AudioClip respondingSoundForUI;

	/// <summary>
	/// Initializes a new instance of the <see cref="UIRespond"/> class.
	/// </summary>
	/// <param name="respondingTextForUI">Responding text for UI.</param>
	/// <param name="respondingSoundForUI">[Optional]Responding sound for UI.</param>
	public UIRespond(String respondingTextForUI, AudioClip respondingSoundForUI = null)
	{
		this.respondingTextForUI = respondingTextForUI;
		this.respondingSoundForUI = respondingSoundForUI;
	}
}

[System.Serializable]
public struct ConnectionInformation
{
	public IPAddress ip;
	public int port;
	public string name;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConnectionInformation"/> struct.
	/// </summary>
	/// <param name="ip">IP to connect.</param>
	/// <param name="port">Port for connection.</param>
	/// <param name="name">Player name.</param>
	public ConnectionInformation(IPAddress ip, int port,string name)
	{
		this.ip = ip;
		this.port = port;
		this.name = name;
	}
}

#endregion




		