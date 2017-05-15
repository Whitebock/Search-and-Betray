using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class playerstats : RigidbodyFirstPersonController
{

	//player varaibles / Server controlled
	[SyncVar]
	public int currentHealth;

	[SyncVar]
	public int shield;

	[SyncVar]
	public int ammocount;

	[SyncVar]
	public int mags;

	[SyncVar]
	public float movementspeed;

	[SyncVar]
	public int playerskilled;

	[SyncVar]
	public bool isdead;

	//other attributes
	public string networkid;
	public string networkplayername = "player";

	//for chat
	public string chatmessage;
	public Text chat;


	//animation
	public Animator myanim;

	public GameObject model;

	public GameObject knifeprefab;

	//audio stuff
	public AudioSource myaudio;

	//reference to network client setup script
	public networkplayersetup mynetworksetup;

	//reference to network commands to send to server
	public networkcommands mynetworkcommands;

	public bool isinmenu;

	public bool iscroutching;

	[SyncVar]
	public int grenades;

	[SyncVar]
	public bool hasweapon;

	// Use this for initialization
	void Start ()
	{
		if (!isLocalPlayer) {
			return;
		}

		//base stuff
		base.Start ();

		myanim = GetComponent<Animator>();

		chatmessage = "FUACK";

		myaudio = GetComponent<AudioSource> ();

		//get references
		mynetworksetup = GetComponent<networkplayersetup> ();
		mynetworkcommands = GetComponent<networkcommands> ();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer) {
			return;
		}

		//for ui
		mynetworksetup.ammo.text = ammocount.ToString ();
		mynetworksetup.health.text = "Hp: " + currentHealth.ToString ();
		mynetworksetup.shield.text = shield.ToString ();
		mynetworksetup.mags.text = mags.ToString();

		//input stuff


		//croutching
		if (Input.GetKey(KeyCode.LeftShift)) {
			myanim.SetBool("isCrouching",true);
		} else {
			myanim.SetBool("isCrouching",false);
		}


		//reloading
		if (Input.GetKeyDown (KeyCode.R)) {

			if (isinmenu) {
				return;
			}

			if (ammocount <= 30 && mags >= 1) {
				mynetworkcommands.CmdReload (networkid);
			}
		}

		//options
		if (Input.GetKeyDown (KeyCode.Escape)) {
			mynetworksetup.options.enabled = !mynetworksetup.options.enabled;
			isinmenu = !isinmenu;
		}

		// fireing and 0 ammo then reload
		if (Input.GetMouseButtonDown (0)) {

			if (isinmenu) {
				return;
			}

			myanim.SetBool("isFiring",true);

			if (ammocount <= 0) {
				if (mags >= 1) {
					mynetworkcommands.CmdReload (networkid);
				}
				Debug.Log("cant reload!");
			} else {
				mynetworkcommands.Fire ();
			}
		} else {
			myanim.SetBool("isFiring",false);
		}

		//throw weapon
		if (Input.GetKeyDown(KeyCode.G)) {
			
			 //mynetworkcommands.CmdThrowWeapon(networkid,);

		}

		//messern
		if (Input.GetKeyDown(KeyCode.E)) {



		}

		//throw grenade
		if (Input.GetKeyDown(KeyCode.F)) {
			
			myanim.SetTrigger("throws");
			GetComponent<NetworkAnimator> ().SetTrigger("throws");
			if (grenades >= 1) {
				mynetworkcommands.CmdThrowGrenade(networkid);
			}

		}

		Vector3 temp = transform.worldToLocalMatrix * Velocity;

		myanim.SetFloat("speed_straight" ,temp.z);
		myanim.SetFloat("speed_sideways" , temp.x);

			

		if (!isinmenu) {

		base.Update ();
	
		}
	}



	void FixedUpdate ()
	{
		if (!isLocalPlayer) {
			return;
		}

		if (isinmenu) {
			return;
		}

		myanim.SetBool("isGrounded", base.Grounded); 
		base.FixedUpdate ();
	}


	[ClientRpc]
	public void RpcDealDamage (int amount, string playername)
	{

		if (isdead) {
			return;
		}

		currentHealth -= amount;
		chatmessage = "player " + playername + " health now " + currentHealth;

		if (currentHealth <= 0) {
			Die ();
		}

		Debug.Log ("player " + playername + " health now " + currentHealth);

	}

	public void Die ()
	{
		isdead = true;

		currentHealth = 0;
		model.SetActive(false);

		StartCoroutine(RpcRespawn());
	}

	IEnumerator RpcRespawn ()
	{
		yield return new WaitForSeconds(5f);

		currentHealth = 100;
		transform.position = CustomNetworkManager.singleton.GetStartPosition().position;

		model.SetActive(true);
	}

}