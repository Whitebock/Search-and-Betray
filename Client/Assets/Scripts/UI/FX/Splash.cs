using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
}
