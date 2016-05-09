using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Invoke("LoadLevel",1);
	}
	
	// Update is called once per frame
	void LoadLevel () 
	{
		GameObject.Find("Loader").GetComponent<Loader>().LoadSpecialLevel("MainMenu");
	}

	[RuntimeInitializeOnLoadMethod]
	public void StartUp()
	{

		//Screen Resolution
		if (PlayerPrefs.HasKey("ResolutionWidth") && PlayerPrefs.HasKey("ResolutionHeight"))
		{
			Screen.SetResolution(PlayerPrefs.GetInt("ResolutionWidth"),PlayerPrefs.GetInt("ResolutionHeight"),Screen.fullScreen);
		}
		else
		{
			PlayerPrefs.SetInt("ResolutionWidth",Screen.currentResolution.width);
			PlayerPrefs.SetInt("ResolutionHeight",Screen.currentResolution.height);
		}

		//Screen Quality
		if (PlayerPrefs.HasKey("MainQuality")) 
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("MainQuality"));
		}
		else
		{
			PlayerPrefs.SetInt("MainQuality",QualitySettings.GetQualityLevel());
		}

		//Fullscreen
		if (PlayerPrefs.HasKey("Fullscreen")) 
		{
			Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetString("Fullscreen"));
		}
		else
		{
			Screen.fullScreen = true;
			PlayerPrefs.SetString("Fullscreen","true");
		}


	}
}
