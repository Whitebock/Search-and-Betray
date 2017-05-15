using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class networkplayersetup : NetworkBehaviour {

	public Camera camera;

	public playerstats player;

	//spawn gui for local player
	public GameObject playerUi;

	//set references for later use
	public Text ammo;
	public Text health;
	public Text shield;
	public Text severchat;
	public Text mags;

	//reference for inagem crosshair
	public Image crosshair;

	public Canvas options;

	// Use this for initialization
	void Start () {

		if (!isLocalPlayer) {
			
			//disable stuff if not local player
			camera.enabled = false;
			gameObject.GetComponent<playerstats>().enabled = false;
			gameObject.GetComponent<AudioListener> ().enabled = false;


		} else {
			//reference to playerstats
			player = GetComponent<playerstats>();

			//instance player ui
			 Instantiate (playerUi, Vector3.zero, Quaternion.identity);

			//Get all texts / set references to ingamegui
			ammo =  GameObject.FindGameObjectWithTag ("ammo").GetComponent<Text>();
			mags =  GameObject.FindGameObjectWithTag ("mags").GetComponent<Text>();
			health =  GameObject.FindGameObjectWithTag ("health").GetComponent<Text>();
			severchat =  GameObject.FindGameObjectWithTag ("serverchat").GetComponent<Text>();
			shield = GameObject.FindGameObjectWithTag ("shield").GetComponent<Text>();
			crosshair  = GameObject.FindGameObjectWithTag ("crosshair").GetComponent<Image>();

			//extra options settings disable
			options =  GameObject.FindGameObjectWithTag ("options").GetComponent<Canvas>();
			options.enabled = false;

		}
	}

	public override void OnStartClient ()
	{
		base.OnStartClient ();

		player = GetComponent<playerstats>();

		//just for testing
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;

		player.networkid = GetComponent<NetworkIdentity>().netId.ToString();

		//register player to gamemanager
		GameManager.RegisterPlayer (player.networkid,gameObject.GetComponent<playerstats>());

		Debug.Log("registered player" +  player.networkid);

		//animation Sync
		GetComponent<NetworkAnimator> ().animator = GetComponent<Animator>();
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (0, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (1, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (2, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (3, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (4, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (5, true);
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (6, true);


	}

}
