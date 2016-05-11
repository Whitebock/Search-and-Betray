using UnityEngine;
using System.Collections;

public class ShiftMovement : MonoBehaviour
{
	/*
	 * Damit man nicht von Treppen abrutscht oder sie wie eine Rampe benutzt, weil der Collider nur eine ebene Fläche ist,
	 * kann man dieses Skript benutzen um den Spieler auf der Treppe zu fixieren als würde er auf einer Treppenstufe stehen.
	 * Solange der Spieler sich in dem Trigger befindet (der auf dem gleichen GameObject liegt wie das Skript), wird die Schwerkraft vorrübergehend
	 * abgeschaltet. Damit wird das Abrutschen des Spielers verhindert.
	 * Die "W,A,S,D"-Steuerung des Spielers wird an die Ausrichtung dieses GameObjects angepasst. Damit bewegt er sich auf der Treppe genau so wie auf dem Boden.
	 * Mit diesem Skript lassen sich auch Leitern einbauen.
	 */

	public bool canJump = true;								// Ob der Spieler vom Trigger abspringen kann
	private Movement movement;								// Referenz zum Bewegungs-Skript
	private Jumping jumping;								// Referenz zum Springen-Skript
	private int playerLayer;								// Layer auf dem der Spieler liegt

	void Start()
	{
		// Initialisierungen
		playerLayer = LayerMask.NameToLayer("Player");
		movement = GameObject.Find("Player").GetComponent<Movement>();
		jumping = movement.GetComponent<Jumping>();
		PlayerInfo.On_Player_Disabled += PlayerDisabled;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != playerLayer) return;	// Spieler herrausfiltern (mit "playerLayer")
		movement.ShiftMovement(transform);					// Bewegung des Spielers auf die neue Achse anpassen
		jumping.canJump = canJump;							// Springen blockieren (wenn eingestellt mit "canJump")
		PlayerInfo.On_Player_isGrounded += PlayerGrounded;	// Fall abfangen -> Spieler berührt wieder den Boden
		PlayerInfo.On_Inp_Jump += PlayerJumpedOff;			// Fall abfangen -> Spieler springt ab
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer != playerLayer) return;	// Spieler herrausfiltern (mit "playerLayer")
		movement.NormalMovement();							// Bewegung zurücksetzen
		jumping.canJump = false;							// Springen wieder erlauben
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;	// Fall nicht mehr abfangen -> Spieler berührt wieder den Boden
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;			// Fall nicht mehr abfangen -> Spieler springt ab
	}

	// Spieler hat den Einflussbereich verlassen weil er den Boden wieder berührt (z. B. bei Leitern)
	void PlayerGrounded(bool grounded)
	{
		if (!grounded) return;		// Checken ob der Spieler den Boden berührt oder nicht mehr berührt
		movement.NormalMovement();	// Bewegung zurücksetzen
		jumping.canJump = false;	// Springen wieder erlauben
	}

	// Spieler hat den Einflussbereich verlassen weil er abgesprungen ist
	void PlayerJumpedOff()
	{
		if (!canJump) return;		// Checken ob der Spieler abspringen darft (mit "canJump")
		movement.NormalMovement();	// Bewegung zurücksetzen
		jumping.canJump = false;	// Springen wieder erlauben
	}

	// Events abmelden wenn der Spieler inaktiv ist
	void PlayerDisabled()
	{
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;
	}

	void OnDisabled()
	{
		// Events abmelden wenn dieses Objekt/Skript inaktiv ist
		PlayerInfo.On_Player_Disabled -= PlayerDisabled;
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;
	}
}