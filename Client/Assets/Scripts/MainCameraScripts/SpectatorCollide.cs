using UnityEngine;
using System.Collections;

public class SpectatorCollide : MonoBehaviour
{
	/*
	 * Dises Skript bewegt das Object auf dem es liegt lokal auf der Z-Achse um -maxDistance- nach hinten.
	 * Dabei wird nach Hindernissen zwischen Objekt und dem ParrentObject gepüft.
	 * Die Höhe und Breite der MainCamera in Unity's Worldspace wird auch beachtet.
	 */
	public float maxDistance = 3f;		// Wie weit das Objekt vom ParrentObject weg ist, wenn kein Hindernis da ist
	private Camera myCam;				// Referenz auf die MainKamera

	void Start()
	{
		// Initialisierungen
		myCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	void Update()
	{
		// Object auf der lokalen Z-Achse positionieren
		RaycastHit hitInfo;
		if (Physics.BoxCast(transform.parent.position, CameraBox(), transform.parent.localToWorldMatrix * transform.localPosition, out hitInfo, transform.rotation, maxDistance, 1))
		{
			// Position noch vor das Hindernis setzen
			transform.localPosition = new Vector3 (0f, 0f, -hitInfo.distance);
		}
		else
		{
			// Position auf die Maximaldistanz zwischen Objekt und ParrentObjekt setzen
			transform.localPosition = new Vector3 (0f, 0f, -maxDistance);
		}
	}

	private Vector3 CameraBox()
	{
		// Die Größe der NearClipPlane von der MainCamera im Worldspace herrausfinden
		Vector3 size = myCam.ViewportToWorldPoint(new Vector3(1f, 1f, myCam.nearClipPlane)) - myCam.ViewportToWorldPoint(new Vector3(0f, 0f, myCam.nearClipPlane));

		// Die größe der NearClipPlane für den "Physics.BoxCast(...)" zurückgeben
		return new Vector3(size.x / 2, size.y / 2, 0.05f);
	}
}