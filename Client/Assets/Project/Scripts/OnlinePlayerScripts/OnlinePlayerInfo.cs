using UnityEngine;
using System.Collections;

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

	private AnimatorPlayer anim;					// Animationen
	private bool isGrounded;						// Ob spieler Bodenkontackt hat
	public DamageHandler bodyDamageHandler;			// Refferenz auf die Hitbox, die mit einer Brustpanzerung geschützt werden kann.

	// Testvariablen
	public Vector3 posOffset = new Vector3(30f, 0f, 0f);	// Wo der Spieler statt der echten Position sein soll (nur zum testen!)

	// Properties
	public int PlayerID
	{ get { return playerID; } }
	public string PlayerName
	{ get { return playerName; } }
	public bool IsCrouching
	{ get { return isCrouching; } }
	public bool IsGrounded
	{ get { return isGrounded; } }

	void Start()
	{
		// Objekt umbenennen
		transform.name += "_" + playerName;

		// Initialisierungen
		anim = new AnimatorPlayer(GetComponentInChildren<Animator>());

		// UI initialisieren (und ggf. Fehler abfangen)
		GameObject x = GameObject.Find("WorldspaceUIs");
		if (x != null) x.GetComponent<WorldspaceUI_Menager>().MakeUI(transform);
		else Debug.Log("Objekt \"WorldspaceUIs\" not found!");



		// ------------------ Simulierte Netzwerkschnittstelle ------------------
		Netzwerk_Simulator.NetzwerkStream += Stream;
		// ----------------------------------------------------------------------
	}

	// ------------------ Simulierte Netzwerkschnittstelle ------------------
	// Daten auspacken und nutzen
	void Stream(int sender, int empfaenger, PackageType typ, string info)
	{
		string[] msg = info.Split(new char[]{ ',' });
		float[] hilf = new float[msg.Length];
		for(int i = 0; i < msg.Length; i++) float.TryParse(msg[i], out hilf[i]);

		switch(typ)
		{
		case PackageType.Position:	// Positionsdaten
			transform.position = new Vector3(hilf[0] + posOffset.x, hilf[1] + posOffset.y, hilf[2] + posOffset.z);
			break;
		case PackageType.Rotation:	// Ausrichtung empfangen
			transform.rotation = new Quaternion(hilf[0], hilf[1], hilf[2], hilf[3]);
			break;
		case PackageType.Crouch:	// Ob Spieler geduckt ist
			if (info == "true") isCrouching = true;
			else isCrouching = false;
			break;
		case PackageType.Velocity:	// Ob Spieler geduckt ist
			anim.Speed_straight = Mathf.Lerp(anim.Speed_straight, hilf[2], 0.6f);
			anim.Speed_sideways = Mathf.Lerp(anim.Speed_sideways, hilf[0], 0.6f);
			break;
		case PackageType.Granade:
			anim.Throw();
			break;
		}
	}
	// ----------------------------------------------------------------------

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