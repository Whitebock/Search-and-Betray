using UnityEngine;
using System.Collections;

public class FreeCameraMovement : MonoBehaviour
{
	/*
	 * Dises Skript steuert die freie Kamera sobald es aktiv ist.
	 */

	public float speed = 1f;						// Geschwindigkeit
	private float sideways, straight, vertical;		// Richtung
	private MouseLook mouseLook;					// Referenz auf das "Umgucken-Skript"

	void Start()
	{
		// Initialisierungen
		mouseLook = GetComponent<MouseLook>();

		// Bewegung zu Beginn deaktivieren
		enabled = false;
	}

	void Update()
	{
		// Input registrieren
		sideways = Input.GetAxisRaw("Horizontal");
		straight = Input.GetAxisRaw("Vertical");
		vertical = ToAxis(Input.GetButton("Jump"), Input.GetButton("Crouch"));

		// Diagonales Bewegen auf der Ebene
		if (sideways != 0 && straight != 0)
		{
			sideways *= 0.7071f;
			straight *= 0.7071f;
		}

		// Diagonales Bewegen in der Vertikalen
		if (sideways != 0 || straight != 0) vertical *= 0.7071f;

		// Kamera bewegen
		if (Input.GetButton("Fire2")) transform.Translate(new Vector3(sideways, vertical / 2f, straight) * speed * 2f);
		else transform.Translate(new Vector3(sideways, vertical / 2f, straight) * speed);
	}

	// Freie Kamera vorbereiten
	public void Initialise()
	{
		enabled = true;
		mouseLook.Tr_horizontal = transform;
		mouseLook.Tr_vertical = transform;
	}

	// Zwei Buttons als "Axis" zurückgeben
	private float ToAxis(bool pos, bool neg)
	{
		if (pos && !neg) return 1f;
		else if (!pos && neg) return -1f;
		else return 0f;
	}
}