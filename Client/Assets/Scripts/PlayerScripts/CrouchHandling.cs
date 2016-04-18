using UnityEngine;
using System.Collections;

public class CrouchHandling : MonoBehaviour
{
	/*
	 * Dises Skript kümmert sich um das Anpassen des Umweltcolliders beim Ducken und Aufstehen.
	 * Dazu wird vorher geprüft ob Aufstehen an der Position Möglich ist.
	 * Ein Event zum Ducken und Aufstehen wird auch abgefeuert.
	 */

	public float crouchHeight = 1.2f;		// Geduckte Höhe des Spielers
	private Bounds normalBounds;			// Originale Maße des Colliders merken
	private bool canStand;					// Ob der Spieler an der aktuellen Position stehen kann
	private CapsuleCollider envCollider;	// Refferenz auf den Umweltcollider (Spieler)

	public bool CanStand
	{ get { return canStand; } }

	void Start()
	{
		// Input einfangen
		PlayerInfo.On_Inp_Crouch += SetCrouch;

		// Initialisierung
		envCollider = GetComponent<CapsuleCollider>();
		normalBounds = envCollider.bounds;
		canStand = true;
	}

	void Update()
	{
		// Ob Ausftehen blockiert wird
		if (Physics.SphereCast(new Ray(transform.position, Vector3.up), normalBounds.size.x / 2, normalBounds.size.y - crouchHeight / 2 - normalBounds.size.x / 2 - 0.01f)) canStand = false;
		else canStand = true;

		// Spieler hinstellen sobald es möglich ist
		if (PlayerInfo.IsCrouching && !PlayerInfo.IsCrouchingInp && canStand) Stand();
	}

	void SetCrouch(bool isCrouchingInp)
	{
		if (isCrouchingInp && !PlayerInfo.IsCrouching) Crouch();	// Ducken-Event auslösen
		else if (PlayerInfo.IsCrouching && canStand) Stand();		// Aufstehen-Event auslösen
	}

	void Stand()
	{
		envCollider.height = normalBounds.size.y;	// Spieler aufrichten
		PlayerInfo.IsCrouching = false;				// Benachrichtigung das Spieler steht
	}

	void Crouch()
	{
		envCollider.height = crouchHeight;		// Spieler ducken lassen
		PlayerInfo.IsCrouching = true;			// Benachrichtigung das Spieler geduckt ist
	}
}
