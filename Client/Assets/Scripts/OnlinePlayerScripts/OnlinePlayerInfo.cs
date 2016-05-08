using UnityEngine;
using System.Collections;
using Assets.Scripts.Network;

public class OnlinePlayerInfo : MonoBehaviour
{
	/*
	 * Alle Informationen über den Onlinemitspieler kommen hier an.
	 * Alle Skripte an diesem Spieler holen sich hier ihre Informationen her.
	 * Animationen werden auch hier gesteuert.
	 */

	// Felder
	private int playerID;							// Spieler-ID
	private string playerName = "Test-Spieler";		// Spielername
	private bool isCrouching;						// Ob Spieler gerade geduckt ist

	private OnlinePlayerUI_Manager uiManager;		// Spieler-UI
	private AnimatorPlayer anim;					// Animationen
	private bool isGrounded;						// Ob spieler Bodenkontackt hat
	public DamageHandler bodyDamageHandler;			// Refferenz auf die Hitbox, die mit einer Brustpanzerung geschützt werden kann.

	// ----- Testvariablen -----
	// Wo der Spieler statt der echten Position sein soll (für Testzwecke)
	public Vector3 posOffset = new Vector3(0f, 0f, 0f);
	// -------------------------

	// Properties
	public int PlayerID
	{ get { return playerID; } set { playerID = value; } }
	public string PlayerName
	{ get { return playerName; } set { playerName = value; if (uiManager) uiManager.UpdateValues(); } }
	public bool IsCrouching
	{ get { return isCrouching; } }
	public bool IsGrounded
	{ get { return isGrounded; } }

	void Start()
	{
		// Objekt umbenennen
		transform.name = "OnlinePlayer_" + playerName;

		// Animator initialisieren
		anim = new AnimatorPlayer(GetComponentInChildren<Animator>());

		// UI initialisieren (und ggf. Fehler abfangen)
		GameObject x = GameObject.Find("WorldspaceUIs");
		if (x != null) uiManager = x.GetComponent<WorldspaceUI_Manager>().MakeUI(transform);
		else Debug.LogError("GAME: Objekt \"WorldspaceUIs\" not found!\nOnline player information (such as nickname) will not be displayed.");

		// SpectatorKamera initialisieren
		GameObject.Find("MainCamera").GetComponent<MainCameraManager>().UpdateCams();

        // ---------------------- Netzwerkschnittstelle ----------------------
        CCC_Client.Instance.OnPlayerUpdate += OnPlayerUpdate;
		// -------------------------------------------------------------------
	}    

    // ---------------------- Netzwerkschnittstelle ----------------------
    private void OnPlayerUpdate(int playerid, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 scale)
    {
        if (PlayerID != playerid) return;

        Dispatcher.Instance.Invoke(delegate ()
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;

            anim.Speed_straight = Mathf.Lerp(anim.Speed_straight, velocity.z, 0.6f);
            anim.Speed_sideways = Mathf.Lerp(anim.Speed_sideways, velocity.x, 0.6f);
        });
    }
    void OnDestroy()
	{
        CCC_Client.Instance.OnPlayerUpdate -= OnPlayerUpdate;
    }
	// -------------------------------------------------------------------

	void Update()
	{
		/*/ ------- Geschwindigkeit anhand von Positionsdaten ermitteln (work in progress) -------
		Vector3 speed = Vector3.ClampMagnitude(transform.worldToLocalMatrix * (transform.position - lastPosition) / Time.deltaTime, 5f);
		Debug.DrawLine(new Vector3(31f, 2f, 0f), new Vector3(31f, 2f, 0f) + speed, Color.blue);
		//Debug.Log(speed.x + ", " + speed.z);
		lastPosition = transform.position;
		anim.Speed_straight = speed.z;
		anim.Speed_sideways = speed.x;*/
		// ---------------------------------------------------------------------------------------
		
		// Animationen
		anim.IsCrouching = isCrouching;
		anim.IsGrounded = isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.05f, 1);
	}

	// Brustpanzerung setzen
	public void SetBodyArmor(int armor)
	{
		if (bodyDamageHandler != null) bodyDamageHandler.Armor = armor;
	}
}