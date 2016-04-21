using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnlinePlayerUI_Manager : MonoBehaviour
{
	/*
	 * Dieses Skript positioniert alle UI-Elemente zwischen Kamera und dem Spieler dem die UI zugewiesen ist.
	 * Dabei bleibt der Abstand zwischen UI und der Kamera immer gleich, unabhängig von der Entfernung des Spielers.
	 */

	private Transform onlinePlayer, myCamera, myCameraPos;	// -onlinePlayer- ist der Spieler dem diese UI zugewiesen ist
	private OnlinePlayerInfo info;									// Die Daten die angezeigt werden können (z. B. "info.PlayerName")
	private Vector3 difference;								// Vector3 zwischen Kamera und diesem Spieler
	public bool visible;									// Ob der Spieler diesen Spieler sehen kann
	public float offset = 2f;								// Abstand zwischen der Kamera und der UI
	public RectTransform hoverField;						// Das Feld über welches man hovern muss, um die UI sichtbar zu machen.

	public Text playername;									// UI-Element: Spielername
	// public ...											// Weitere, zukünftige UI-Elemente

	public Vector3 Difference
	{ get { return difference; } }
	public bool Visible
	{ get { return visible; } }
	public Transform OnlinePlayer
	{
		get { return onlinePlayer; }
		set
		{
			onlinePlayer = value;

			// Spielerdaten zum Anzeigen zu verfügung stellen
			Info = value.GetComponent<OnlinePlayerInfo>();
			if (info != null)
			{
				// Objekt nach dem Spieler, zu dem es gehört, umbenennen
				transform.name = "PlayerUI_" + info.PlayerID + "_" + info.PlayerName;

				// Das Feld, über welches man hovern muss um die UI zu sehen, dem Spieler folgen lassen
				hoverField.SetParent(Info.transform.Find("Canvas"), false);
			}
			else Debug.Log("Das Übergebene Objekt stellt keine Spielerdaten zur Verfügung!");
		}
	}
	public OnlinePlayerInfo Info
	{
		get { return info; }
		set
		{
			info = value;
			UpdateValues(); // Sobald die Spielerdaten zur Verfügung stehen, die UI updaten
		}
	}

	void Start()
	{
		// Initialisierungen
		myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		myCameraPos = GameObject.Find("PlayerCameraPos").transform;
	}

	void Update()
	{
		// Sobald kein Spieler diesem Skript mehr zugewiesen ist, wird das Objekt zerstört
		if (onlinePlayer != null)
		{
			// Gesamte UI positionieren
			difference = onlinePlayer.position + myCameraPos.localPosition - myCamera.position;
			transform.position = myCamera.position + difference.normalized * offset;

			// Höhe des Spielernamens nach Entfernung anpassen
			playername.rectTransform.localPosition = new Vector3(0f, Mathf.Clamp(difference.magnitude, 0.5f, 20f) * -0.5f + 20f, 0f);

			// Sichtbarbeit zwischen diesem Spieler und der "MainCamera" prüfen
			RaycastHit x = new RaycastHit();
			Physics.Raycast(onlinePlayer.position, -difference + myCameraPos.localPosition, out x);
			try
			{
				if (x.transform.tag == "Player") visible = true;
				else visible = false;
			}
			catch
			{
				visible = false;
			}
		}
		else GameObject.Destroy(gameObject);
	}

	// Diese Methode holt sich alle Informationen aus dem Spieler um sie anzuzeigen
	public void UpdateValues()
	{
		if (info == null) return;

		// UI-Element: Spielername
		if (playername == null) Debug.Log("OnlinePlayerUI_Manager: Es wurde kein UI-Element für \"playername\" festgelegt!Objekt: \n" + transform.name);
		else playername.text = info.PlayerName;
	}
}