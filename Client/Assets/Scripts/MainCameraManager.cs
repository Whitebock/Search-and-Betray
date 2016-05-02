using UnityEngine;
using System.Collections;

public class MainCameraManager : MonoBehaviour
{
	/*
	 * Die Position der MainCamera wird nie direkt angesteuert. Die MainCamera folgt immer einem "Empty GameObject"
	 * und übernimmt dessen Position und Rotation.
	 * Dafür sorgt dieses Skript.
	 * Das Wechseln der Kameraansichten wird hier gesteuert.
	 * Mit dem Property -SpectatorMode- wird zwischen Game- und Spectatormodus gewechselt.
	 */

	private MouseLook mouseLook;			// Refferrenz auf das "Umgucken-Skript"
	private int currentCam = 0;				// Ausgewählte Sicht
	private Transform myCam;				// Eigene Ego-Sicht
	private Transform[] otherCam;			// 3rd-Person-Sichten (andere Spieler)
	private FreeCameraMovement movement;	// Referenz auf die "Freie Kamera"-Steuerung
	private bool spectatorMode = false;		// Kann zwischen den Views hin- und herschalten
	private bool notPressed = false;		// Hilfsvariable zum Umschalten der Kamera
	private int lastSpec = 1;				// Letzter Spieler dem zugeguckt wurde

	public bool SpectatorMode
	{
		get { return spectatorMode; }
		set
		{
			spectatorMode = value;
			if (!spectatorMode) CurrentCam = 0;		// Spectatormodus = AUS | Spieler aus dem Spectatormodus holen
			else CurrentCam = 1;					// Spectatormodus =  AN | Kamera direkt auf den ersten Spieler setzen
		}
	}

	public int CurrentCam
	{
		get { return currentCam; }
		set
		{
			if (value == -1)
			{
				movement.Initialise();			// Freie Kamerabewegung aktivieren
				lastSpec = currentCam;			// Letzten beobachteten Spieler merken
			}
			else movement.enabled = false;		// Freie Kamerabewegung sperren
			currentCam = SwitchCamera(value);
		}
	}

	void Start()
	{
		// Initialisierungen
		mouseLook = GetComponent<MouseLook>();
		myCam = GameObject.Find("PlayerCameraPos").transform;
		movement = GetComponent<FreeCameraMovement>();
		UpdateCams();
	}

	void Update()
	{
		// ----------------- Testzeile (Mit "H" in den Spectatormodus wechseln) -----------------
		if (Input.GetKeyDown(KeyCode.H)) SpectatorMode = !SpectatorMode;
		// --------------------------------------------------------------------------------------

		// Ansichten wechseln
		if (SpectatorMode)
		{
			// Input abfangen
			int switchPlayer = (int)Input.GetAxisRaw("Horizontal");						// Spieler wechseln mit "A" und "D"
			if (switchPlayer == 0) switchPlayer = (int)Input.GetAxisRaw("Vertical");	// Alternativ mit "W" und "S"
			bool switchFreeAnd3rd = Input.GetButtonDown("Fire1");						// Zwischen freie Kamera und 3rd-Person wechseln

			if (currentCam != -1)
			{
				if (switchFreeAnd3rd) CurrentCam = -1;	// Ansicht wechseln: 3rd-Person --> freie Kamera
				else
				{
					// 3rd-Person-Sicht wechseln (anderen Spieler beobachten)
					if (notPressed && switchPlayer != 0)
					{
						if (switchPlayer > 0) CurrentCam = NextCamera(true);
						else CurrentCam = NextCamera(false);
						notPressed = false;
					}
					else if (switchPlayer == 0) notPressed = true; // Erneutes Wechseln der Sicht verhindern, wenn die Taste dazu noch nicht losgelassen wurde
				}
			}
			else if (switchFreeAnd3rd) CurrentCam = lastSpec; // Ansicht wechseln: freie Kamera --> 3rd-Person
		}

		// MainKamera an Position "kleben"
		if (CurrentCam == 0) // Eigene Ego-Sicht
		{
			if (myCam == null || myCam.gameObject.activeSelf == false) CurrentCam = NextCamera(true);
			else
			{
				transform.position = myCam.position;
				transform.rotation = myCam.rotation;
			}
		}
		else if (Available(CurrentCam))	// 3rd-Person-Sicht
		{
			transform.position = otherCam[currentCam - 1].position;
			transform.rotation = otherCam[currentCam - 1].rotation;
		}
	}

	// Alle Spectatoransichten finden
	private void UpdateCams()
	{
		GameObject[] x = GameObject.FindGameObjectsWithTag("OnlinePlayerCam");
		otherCam = new Transform[x.Length];
		for(int i = 0; i < otherCam.Length; i++) otherCam[i] = x[i].transform;
	}

	// Gibt es diese Spectatoransicht?
	private bool Available(int cam)
	{
		try { return otherCam[cam - 1] != null; }
		catch { return false; }
	}

	// Ungültige Werte abfangen und dem "Umgucken-Skript" sagen, welche Objekte es steuern soll
	private int SwitchCamera(int cam)
	{
		if (cam == 0 || !SpectatorMode) // Wieder auf Ego-Sicht stellen
		{
			mouseLook.PlayerPos();
			return 0;
		}
		else if (cam == -1) return -1;
		else if (Available(cam)) // Auf 3rd-Person stellen, wenn der Spieler verfügbar ist
		{
			mouseLook.Tr_vertical = otherCam[cam - 1].parent;
			mouseLook.Tr_horizontal = otherCam[cam - 1].parent;
			return cam;
		}
		else // Wenn der Spieler nicht verfügbar ist, dann versuchen einen anderen zu finden
		{
			int x = NextCamera(true);
			if (x != -1)
			{
				mouseLook.Tr_vertical = otherCam[x - 1].parent;
				mouseLook.Tr_horizontal = otherCam[x - 1].parent;
			}
			return x;
		}
	}

	// Nächste verfügbare 3rd-Person-Sicht finden
	private int NextCamera(bool next)
	{
		int help = CurrentCam;
		for(int i = 1; i <= otherCam.Length; i++)
		{
			if (next) // nächste Position
			{
				help++;
				if (help > otherCam.Length || help < 1) help = 1;
			}
			else // vorherige Position
			{
				help--;
				if (help < 1 || help > otherCam.Length) help = otherCam.Length;
			}
			if (Available(help)) return help; // Position prüfen
		}

		// Freie Kamera zurückgeben, wenn kein anderer Spieler gefunden wurde
		return -1;
	}
}