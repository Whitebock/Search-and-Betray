using UnityEngine;
using System.Collections;

public class MainCameraMenager : MonoBehaviour
{
	/*
	 * Die Position der MainCamera wird nie direkt angesteuert. Die MainCamera folgt immer einem "Empty GameObject"
	 * und übernimmt dessen Position und Rotation.
	 * Dafür sorgt dieses Skript.
	 * Das Wechseln der Kameraansichten kann hier gesteuert werden.
	 */

	private MouseLook mouseLook;	// Refferrenz auf das "Umgucken-Skript"
	private int currentCam = 0;		// Ausgewählte Sicht

	Transform myCam;		// Eigene Ego-Sicht
	Transform[] otherCam;	// Spectator-Sichten (andere Spieler)
	
	public int CurrentCam
	{
		get { return currentCam; }
		set
		{
			// Ungültige Werte abfangen
			if (value < 1 || value > otherCam.Length)
			{
				// Wieder auf Ego-Sicht stellen
				mouseLook.PlayerPos();
				value = 0;
			}
			else
			{
				// MouseLook-Skript Bescheid sagen welche Objekte sie steuern soll
				mouseLook.tr_vertical = otherCam[value - 1].parent;
				mouseLook.tr_horizontal = otherCam[value - 1].parent;
			}
			currentCam = value;
		}
	}

	void Start()
	{
		// Initialisierungen
		mouseLook = GetComponent<MouseLook>();
		myCam = GameObject.Find("PlayerCameraPos").transform;
		UpdateCams();
	}

	void Update()
	{
		// Testzeile (View Toggeln)
		if (Input.GetKeyDown(KeyCode.H)) CurrentCam = CurrentCam + 1;

		// Kamera an Position "kleben"
		if (CurrentCam == 0) // Eigene Ego-Sicht
		{
			if (myCam != null)
			{
				transform.position = myCam.position;
				transform.rotation = myCam.rotation;
			}
		}
		else if (otherCam[currentCam - 1] != null) // Spectator 3rd-Person-Sicht
		{
			transform.position = otherCam[currentCam - 1].position;
			transform.rotation = otherCam[currentCam - 1].rotation;
		}
	}

	// Alle SpectatorKamerapositionen finden
	void UpdateCams()
	{
		GameObject[] x = GameObject.FindGameObjectsWithTag("OnlinePlayerCam");
		otherCam = new Transform[x.Length];
		for(int i = 0; i < otherCam.Length; i++) otherCam[i] = x[i].transform;
	}
}
